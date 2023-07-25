using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocPage))]
    public class SODocPageEditor : Editor
    {
        public static event Action<SODocPageEditor> OnCreateEditor;
        private void OnEnable()
        {
            OnCreateEditor?.Invoke(this);
        }
        SODocPage Target;
        VisualElement root;
        bool isEditMode = true;
        bool isDraging = false;
        VisualElement dragingTarget;
        ISPosition dragPosition;
        ScrollView scrollView;
        public override VisualElement CreateInspectorGUI()
        {
            Target = target as SODocPage;
            scrollView = new ScrollView();
            root = new VisualElement();
            root.style.SetIS_Style(ISPadding.Pixel(10));
            clickMask = new VisualElement();
            clickMask.style.SetIS_Style(ISSize.Percent(100, 100));
            clickMask.style.position = Position.Absolute;
            VisualElement bar = new VisualElement();
            bar.style.SetIS_Style(ISFlex.Horizontal);
            bar.style.marginBottom = 10;
            Button editMode = new Button();
            Button viewMode = new Button();
            Button defuMode = new Button();
            Button saveBtn = new Button();
            editMode.text = "Edit Layout";
            viewMode.text = "View Layout";
            defuMode.text = "Inspector";
            saveBtn.text  = "Save";
            saveBtn.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            bar.Add(editMode);
            bar.Add(viewMode);
            bar.Add(defuMode);
            bar.Add(saveBtn);
            bar.style.height = 20;
            root.Add(bar);
            root.Add(new IMGUIContainer(() => { 
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SubPages"));
                if (EditorGUI.EndChangeCheck()) { OnSubPagesChange?.Invoke(); serializedObject.ApplyModifiedProperties(); }
            }));
            editMode.clicked += () =>
            {
                Save();
                isEditMode = true;
                scrollView.Clear();
                scrollView.Add(createEdit());
            };
            viewMode.clicked += () =>
            {
                Save();
                isEditMode = false;
                scrollView.Clear();
                scrollView.Add(createView());
            };
            defuMode.clicked += () =>
            {
                Save();
                isEditMode = false;
                scrollView.Clear();
                scrollView.Add(new IMGUIContainer(() => { DrawDefaultInspector(); }));
            };
            saveBtn.clicked += Save;
            root.RegisterCallback<KeyDownEvent>(e =>
            {
                if(e.ctrlKey && e.keyCode==KeyCode.S) { Save(); }
            });
            scrollView.Add(createEdit());
            scrollView.mode = ScrollViewMode.Vertical;
            Button addNew = new Button();
            addNew.text = "Add";
            addNew.clicked += () => { EditRoot.Add(createUnit(new DocComponent())); };
            addNew.style.marginTop = 10;
            scrollView.Add(addNew);
            root.Add(scrollView);
            return root;
        }
        private void OnDisable(){ Save(); }

        public VisualElement EditRoot;
        VisualElement clickMask;
        VisualElement createEdit()
        {
            dragPosition = new ISPosition() { Left = ISStyleLength.Pixel(0), Top = ISStyleLength.Pixel(0) };
            dragPosition.Position = Position.Absolute;
            EditRoot = new VisualElement();
            EditRoot.style.height = Length.Percent(100);
            foreach (var doc in Target.Components)
            {
                EditRoot.Add(createUnit(doc));
            }
            clickMask.RegisterCallback<MouseMoveEvent>((e) =>
            {
                if (isDraging)
                {
                    int i = calDragingIndex();
                    for(int j=0;j<EditRoot.childCount;j++)
                    {
                        if (i == j)
                            EditRoot[j].style.borderTopWidth = 5;
                        else
                            EditRoot[j].style.borderTopWidth = 0;
                    }
                    dragPosition.Top.Value.Value = e.localMousePosition.y+1;
                    dragPosition.Left.Value.Value = e.localMousePosition.x+1;
                    dragingTarget.style.SetIS_Style(dragPosition);
                }
            });
            return EditRoot;
        }
        VisualElement createView()
        {
            VisualElement root = new VisualElement();
            foreach (var doc in Target.Components)
            {
                root.Add(DocRuntime.CreateVisual(doc));
            }
            return root;
        }

        public event Action<SODocPage> OnSaveData;
        public event Action OnSubPagesChange;
        public void Save()
        {
            if (Target == null) return;
            
            if(Target.SubPages != null)
            {
                for (int i = 0; i < Target.SubPages.Count; i++)
                {
                    if (Target.SubPages[i] == null)
                    {
                        Target.SubPages.RemoveAt(i);
                        i--;
                    }
                }
            }
            if(isEditMode)
            {
                List<DocComponent> newComponents = new List<DocComponent>();
                if (EditRoot == null) return;
                foreach (var visual in EditRoot.Children())
                {
                    var edit = visual.Q<DocEditVisual>();
                    if (edit == null) continue;
                    if (edit.Target == null) continue;
                    newComponents.Add(edit.Target);
                }
                Target.Components = newComponents;
            }
            EditorUtility.SetDirty(target);
            OnSaveData?.Invoke(Target);
        }

        VisualElement createUnit(DocComponent doc)
        {
            VisualElement unit = new VisualElement();
            VisualElement toolBar = new VisualElement();
            unit.style.borderTopColor = new Color(.75f, .75f, 1f, 0.5f);
            toolBar.style.marginTop = 10;
            toolBar.style.SetIS_Style(ISFlex.Horizontal);
            toolBar.Add(insertBtn(unit));
            toolBar.Add(upBtn(unit));
            toolBar.Add(downBtn(unit));
            toolBar.Add(dragBtn(unit));
            toolBar.Add(deleteBtn(unit));   
            unit.Add(toolBar);
            unit.Add(DocEditor.CreateEditVisual(doc));
            
            return unit;
        }

        Button insertBtn(VisualElement unit)
        {
            Button button = new Button();
            button.text = "Insert ->";
            button.clicked += () =>
            {
                int index = EditRoot.IndexOf(unit);
                EditRoot.Insert(index, createUnit(new DocComponent()));
            };
            return button;
        }
        Button deleteBtn(VisualElement unit)
        {
            Button button = new Button();
            button.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            button.text = "delete";
            button.clicked += () =>
            {
                EditRoot.RemoveAt(EditRoot.IndexOf(unit));
            };
            return button;
        }
        Button upBtn(VisualElement unit)
        {
            Button button = new Button();
            button.text = "¡¶";
            button.clicked += () =>
            {
                int i = EditRoot.IndexOf(unit);
                if(i>0)
                    EditRoot.Insert(i-1, unit);
            };
            return button;
        }
        Button downBtn(VisualElement unit)
        {
            Button button = new Button();
            button.text = "¡¿";
            button.clicked += () =>
            {
                int i = EditRoot.IndexOf(unit);
                if (i < unit.parent.childCount-1)
                    EditRoot.Insert(i + 1, unit);
            };
            return button;
        }
        Button dragBtn(VisualElement unit)
        {
            Button button = new Button();
            button.text = "Drag";
            button.clicked += () =>
            {
                isDraging = true;
                dragingTarget = unit;
                EditRoot.Add(unit);
                root.Add(clickMask);
                clickMask.RegisterCallback<MouseDownEvent>(endDraging);
                float sumHeight = 0;
                foreach (var ve in EditRoot.Children()) { sumHeight += ve.layout.height; }
                sumHeight += 400;
                root.style.height = sumHeight;
            };
            return button;
        }
        void endDraging(MouseDownEvent e)
        {
            isDraging = false;
            root.style.height = Length.Percent(100);
            dragingTarget.style.SetIS_Style(new ISPosition());
            EditRoot.Insert(calDragingIndex(), dragingTarget);
            dragingTarget = null;
            for (int j = 0; j < EditRoot.childCount; j++)
            {
                EditRoot[j].style.borderTopWidth = 0;
            }
            root.Remove(clickMask);
            clickMask.UnregisterCallback<MouseDownEvent>(endDraging);
            Save();
        }
        int calDragingIndex()
        {
            int i = 0;
            foreach (var ve in EditRoot.Children())
            {
                if (ve.layout.yMax > dragPosition.Top.Value.Value)
                    break;
                i++;
            }
            return i;
        }
    }
}