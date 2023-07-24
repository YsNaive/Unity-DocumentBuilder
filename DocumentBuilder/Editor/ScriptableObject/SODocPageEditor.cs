using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocPage))]
    public class SODocPageEditor : Editor
    {
        SODocPage Target;
        VisualElement root;
        bool isEditMode = true;
        public override VisualElement CreateInspectorGUI()
        {
            Target = target as SODocPage;
            root = new VisualElement();
            root.style.SetIS_Style(ISPadding.Pixel(10));
            VisualElement bar = new VisualElement();
            bar.style.SetIS_Style(ISFlex.Horizontal);
            bar.style.marginBottom = 10;
            Button editMode = new Button();
            Button viewMode = new Button();
            Button defuMode = new Button();
            editMode.text = "Edit Layout";
            viewMode.text = "View Layout";
            defuMode.text = "Default Layout";
            bar.Add(editMode);
            bar.Add(viewMode);
            bar.Add(defuMode);
            root.Add(bar);

            editMode.clicked += () =>
            {
                save();
                isEditMode = true;
                root.RemoveAt(1);
                root.Add(createEdit());
            };
            viewMode.clicked += () =>
            {
                save();
                isEditMode = false;
                root.RemoveAt(1);
                root.Add(createView());
            };
            defuMode.clicked += () =>
            {
                save();
                isEditMode = false;
                root.RemoveAt(1);
                root.Add(new IMGUIContainer(() => { DrawDefaultInspector(); }));
            };

            root.Add(createEdit());
            return root;
        }
        private void OnDisable(){ save(); }

        VisualElement editRoot;
        VisualElement createEdit()
        {
            editRoot = new VisualElement();
            foreach(var doc in Target.Components)
            {
                editRoot.Add(createUnit(doc));
            }
            return editRoot;
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

        void save()
        {
            if(isEditMode)
            {
                List<DocComponent> newComponents = new List<DocComponent>();
                foreach (var visual in root[1].Children())
                {
                    var edit = visual.Q<DocEditVisual>();
                    if (edit == null) continue;
                    if (edit.Target == null) continue;
                    newComponents.Add(edit.Target);
                }
                Target.Components = newComponents;
            }
            EditorUtility.SetDirty(target);
        }

        VisualElement createUnit(DocComponent doc)
        {
            VisualElement unit = new VisualElement();
            VisualElement toolBar = new VisualElement();
            toolBar.style.marginTop = 10;
            toolBar.style.SetIS_Style(ISFlex.Horizontal);
            toolBar.Add(insertBtn(unit));
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
                int index = editRoot.IndexOf(unit);
                editRoot.Insert(index, createUnit(new DocComponent()));
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
                editRoot.RemoveAt(editRoot.IndexOf(unit));
            };
            return button;
        }
    }
}