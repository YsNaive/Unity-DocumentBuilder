using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocPage))]
    public class SODocPageEditor : Editor
    {
        public static event Action<SODocPageEditor> OnCreateEditor;
        public static event Action<SODocPage> OnCreateNewPage;
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
        VisualElement contents;
        VisualElement header;
        Button addComponent;
        VisualElement addAndDeleteBar;
        Button addPage;
        CheckButton deletePage;
        ObjectField icon;
        public override VisualElement CreateInspectorGUI()
        {
            #region mod bar
            root = new VisualElement();
            root.style.SetIS_Style(ISPadding.Pixel(10));
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            header = new VisualElement();
            Target = target as SODocPage;
            contents = new VisualElement();
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
            saveBtn.text = "Save";
            saveBtn.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            editMode.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            viewMode.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            defuMode.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            saveBtn.style.backgroundColor = DocStyle.Current.SuccessColor;

            bar.Add(editMode);
            bar.Add(viewMode);
            bar.Add(defuMode);
            bar.Add(saveBtn);
            bar.style.height = 20;
            root.Add(bar);

            #endregion

            #region header bar
            addAndDeleteBar = new VisualElement();
            addAndDeleteBar.style.ClearMarginPadding();
            addAndDeleteBar.style.SetIS_Style(ISFlex.Horizontal);
            addPage = new Button();
            addPage.style.ClearMarginPadding();
            addPage.text = "Add New Page";
            addPage.clicked += () =>
            {
                newPageBtn();
            };
            addPage.style.marginBottom = 5;
            addPage.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            addPage.style.width = Length.Percent(75);
            addPage.style.height = 20;
            deletePage = new CheckButton();
            deletePage.style.height = 20;
            deletePage.style.ClearMarginPadding();
            deletePage.text = "Delete Page";
            deletePage.style.width = Length.Percent(24);
            deletePage.MainBtn.style.backgroundColor = DocStyle.Current.DangerColor;
            deletePage.ConfirmColor = DocStyle.Current.DangerColor;
            deletePage.CancelColor = DocStyle.Current.SuccessColor;
            deletePage.Confirm += () =>
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Target));
                AssetDatabase.Refresh();
                if (DocEditorWindow.Instance != null)
                    DocEditorWindow.RepaintMenu();
            };
            addAndDeleteBar.Add(addPage);
            addAndDeleteBar.Add(deletePage);
            header.Add(addAndDeleteBar);

            icon = new ObjectField();
            icon.label = "icon";
            icon.objectType = typeof(Texture2D);
            icon[0].style.minWidth = 60;
            icon[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            icon.value = Target.Icon;
            icon.RegisterValueChangedCallback(value =>
            {
                Target.Icon = (Texture2D)value.newValue;
                DocEditorWindow.RepaintMenu();
            });
            header.Add(icon);
            header.Add(new IMGUIContainer(() => {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SubPages"));
                if (EditorGUI.EndChangeCheck()) { OnSubPagesChange?.Invoke(); serializedObject.ApplyModifiedProperties(); }
            }));
            header.style.marginBottom = 20;
            root.Add(header);
            #endregion

            #region mod actiom
            editMode.clicked += () =>
            {
                Save();
                isEditMode = true;
                root.Insert(1, header);
                contents.Clear();
                contents.Add(createEdit());
            };
            viewMode.clicked += () =>
            {
                Save();
                isEditMode = false;
                if (root.Contains(header)) { root.Remove(header); }
                contents.Clear();
                contents.Add(createView());
            };
            defuMode.clicked += () =>
            {
                Save();
                isEditMode = false;
                if (root.Contains(header)) { root.Remove(header); }
                contents.Clear();
                contents.Add(new IMGUIContainer(() => { DrawDefaultInspector(); }));
            };
            saveBtn.clicked += Save;
            root.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.ctrlKey && e.keyCode == KeyCode.S) { Save(); }
            });
            #endregion

            contents.Add(createEdit());
            addComponent = new Button();
            addComponent.text = "Add";
            addComponent.clicked += () => { EditRoot.Add(createUnit(new DocComponent())); };
            addComponent.style.marginTop = 10;
            contents.Add(addComponent);
            root.Add(contents);
            return root;
        }
        private void OnDisable() { Save(); }

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
                    for (int j = 0; j < EditRoot.childCount; j++)
                    {
                        if (i == j)
                            EditRoot[j].style.borderTopWidth = 5;
                        else
                            EditRoot[j].style.borderTopWidth = 0;
                    }
                    dragPosition.Top.Value.Value = e.localMousePosition.y + 1 - header.layout.yMax;
                    dragPosition.Left.Value.Value = e.localMousePosition.x + 1;
                    dragingTarget.style.SetIS_Style(dragPosition);
                }
            });
            return EditRoot;
        }
        VisualElement createView()
        {
            return new DocPageVisual(Target);
        }

        public event Action<SODocPage> OnSaveData;
        public event Action OnSubPagesChange;
        public void Save()
        {
            if (Target == null) return;

            if (Target.SubPages != null)
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
            if (isEditMode)
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
            var docEdit = DocEditor.CreateEditVisual(doc);
            unit.style.borderTopColor = new Color(.75f, .75f, 1f, 0.5f);
            toolBar.style.SetIS_Style(ISFlex.Horizontal);
            toolBar.Add(insertBtn(unit));
            toolBar.Add(dragBtn(unit));
            toolBar.Add(deleteBtn(unit));
            toolBar.style.marginTop = 10;
            unit.Add(toolBar);
            unit.Add(docEdit);
            unit.style.borderTopWidth = 3;
            unit.style.borderTopColor = DocStyle.Current.SubBackgroundColor;
            unit.style.marginBottom = 10;
            return unit;
        }
        void newPageBtn()
        {
            var addPageParent = addPage.parent.parent;
            var bar = addPage.parent;
            int addPageIndex = addPageParent.IndexOf(bar);
            addPageParent.RemoveAt(addPageIndex);
            TextField inputName = new TextField();
            inputName.label = "Page Name";
            inputName[0].style.minWidth = 50;
            inputName[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            inputName.style.width = Length.Percent(69);
            inputName.style.ClearMarginPadding();
            Button create = new Button();
            create.text = "Create";
            create.style.width = Length.Percent(15);
            create.style.ClearMarginPadding();
            create.style.backgroundColor = DocStyle.Current.SuccessColor;
            Button cancel = new Button();
            cancel.text = "Cancel";
            cancel.style.width = Length.Percent(15);
            cancel.style.ClearMarginPadding();
            cancel.style.backgroundColor = DocStyle.Current.DangerColor;
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            root.Add(inputName);
            root.Add(create);
            root.Add(cancel);
            root.style.marginBottom = 7;
            header.Insert(addPageIndex,root);
            create.clicked += () =>
            {
                string path = AssetDatabase.GetAssetPath(Target);
                path = path.Substring(0, path.LastIndexOf('/'));
                string folder = $"{Target.name}SubPages";
                if (!AssetDatabase.IsValidFolder(path + '/' + folder))
                    AssetDatabase.CreateFolder(path, folder);
                var asset = CreateInstance<SODocPage>();
                asset.name = inputName.value;
                AssetDatabase.CreateAsset(asset, path + '/' + folder + $"/{asset.name}.asset");
                AssetDatabase.Refresh();
                var sp = serializedObject.FindProperty("SubPages");
                sp.InsertArrayElementAtIndex(sp.arraySize);
                sp = sp.GetArrayElementAtIndex(sp.arraySize - 1);
                sp.objectReferenceValue = asset;
                serializedObject.ApplyModifiedProperties();
                Target.SubPages.Add(asset);
                header.Remove(root);

                OnCreateNewPage?.Invoke(asset);
                OnSubPagesChange?.Invoke();
                addPageParent.Insert(addPageIndex, bar);
            };
            cancel.clicked += () =>
            {
                header.Remove(root);
                addPageParent.Insert(addPageIndex, bar);
            };
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
        CheckButton deleteBtn(VisualElement unit)
        {
            CheckButton button = new CheckButton();
            button.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            button.text = "Delete";
            button.Confirm += () =>
            {
                EditRoot.RemoveAt(EditRoot.IndexOf(unit));
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
                EditRoot.style.height = sumHeight;
            };
            return button;
        }
        void endDraging(MouseDownEvent e)
        {
            isDraging = false;
            root.style.height = Length.Percent(100);
            EditRoot.style.height = Length.Percent(100);
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
            float pos = dragPosition.Top.Value.Value - header.layout.yMax;
            foreach (var ve in EditRoot.Children())
            {
                if (ve.layout.y > pos)
                    break;
                i++;
            }
            return i;
        }
    }
}