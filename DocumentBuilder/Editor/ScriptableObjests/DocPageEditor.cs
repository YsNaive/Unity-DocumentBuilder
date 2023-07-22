using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.editor
{
    [CustomEditor(typeof(SODocPage))]
    public class DocPageEditor : Editor
    {
        SODocPage Target;
        VisualElement root;
        VisualElement docVisual;
        int width;
        public override VisualElement CreateInspectorGUI()
        {
            Target = target as SODocPage;
            root = new VisualElement();
            root.style.width = new Length(100, LengthUnit.Percent);
            root.RegisterCallback<GeometryChangedEvent>(layout);
            return root;
        }
        void layout(GeometryChangedEvent e)
        {
            root.Add(new IMGUIContainer(() =>
            {
                var pages = serializedObject.FindProperty("SubPages");
                EditorGUILayout.PropertyField(pages);
            }));
            docVisual = new VisualElement();
            width = (int)root.layout.width;
            foreach (var doc in Target.Components)
            {
                docVisual.Add(createEditBlock(doc));
            }
            Button newDoc = new  Button();
            newDoc.clicked += () =>
            {
                var doc = new DocComponent();
                docVisual.Add(createEditBlock(doc));
                Target.Components.Add(doc);
            };
            newDoc.text = "add component";
            newDoc.style.marginTop = 20;
            newDoc.style.width = 120;
            root.Add(docVisual);
            root.Add(newDoc);
            root.UnregisterCallback<GeometryChangedEvent>(layout);
        }

        private void OnDisable() { save(); }
        void save()
        {
            if (root == null) return;
            int i = 0;
            foreach(var ve in docVisual.Children()) 
            {
                Target.Components[i].FromVisual(ve.Q<DocEditVisual>()[1]);
                i++;
            }
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }

        VisualElement createEditBlock(DocComponent component)
        {
            VisualElement root = new VisualElement();
            VisualElement toolBar = new VisualElement();
            toolBar.style.SetIS_Style(ISFlex.Horizontal);
            toolBar.style.marginTop = 15;
            toolBar.Add(createInsertBtn(root));
            toolBar.Add(createMoveUpBtn(root));
            toolBar.Add(createMoveDownBtn(root));
            toolBar.Add(createDeleteBtn(root));
            var ve = new DocEditVisual(component, width);
            root.Add(toolBar);
            root.Add(ve);
            return root;
        }

        VisualElement createInsertBtn(VisualElement root)
        {
            Button insertBtn = new Button();
            insertBtn.text = "->Insert";
            insertBtn.clicked += () =>
            {
                int i = root.parent.IndexOf(root);
                DocComponent doc = new DocComponent();
                root.parent.Insert(i, createEditBlock(doc));
                Target.Components.Insert(i, doc);
            };
            return insertBtn;
        }
        VisualElement createMoveUpBtn(VisualElement root)
        {
            Button moveUpBtn = new Button();
            moveUpBtn.text = "¡ô";
            moveUpBtn.clicked += () =>
            {
                int i = root.parent.IndexOf(root);
                if (i == 0) return;
                root.parent.Insert(i - 1, root);
                var temp = Target.Components[i];
                Target.Components[i] = Target.Components[i - 1];
                Target.Components[i - 1] = temp;
            };
            return moveUpBtn;
        }
        VisualElement createMoveDownBtn(VisualElement root)
        {
            Button moveDownBtn = new Button();
            moveDownBtn.text = "¡õ";
            moveDownBtn.clicked += () =>
            {
                int i = root.parent.IndexOf(root);
                if (i == (root.parent.childCount-1)) return;
                root.parent.Insert(i + 1, root);
                var temp = Target.Components[i];
                Target.Components[i] = Target.Components[i + 1];
                Target.Components[i + 1] = temp;
            };
            return moveDownBtn;
        }
        VisualElement createDeleteBtn(VisualElement root)
        {
            Button deleteBtn = new Button();
            deleteBtn.text = "delete";
            deleteBtn.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            deleteBtn.style.backgroundColor = new Color(.5f, .3f, .3f);
            deleteBtn.clicked += () =>
            {
                int i = root.parent.IndexOf(root);
                root.parent.RemoveAt(i);
                Target.Components.RemoveAt(i);
            };
            return deleteBtn;
        }
    }
}
