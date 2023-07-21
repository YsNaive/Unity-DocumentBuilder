using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

namespace NaiveAPI_Editor.window
{
    public class DocVisualDebugWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/Debug/DocVisual")]
        public static void ShowWindow()
        {
            GetWindow<DocVisualDebugWindow>("Debug DocVisual");
        }
        private float previousWidth;
        private void OnEnable()
        {
            previousWidth = position.width;
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if ((docEditView != null) && (previousWidth != position.width))
            {
                rootVisualElement.Remove(docEditView);
                docEditView = null;
                docEditView = createView();
                rootVisualElement.Add(docEditView);
                previousWidth = position.width;
            }
        }

        VisualElement docEditView = null;
        DocEditVisual editView;
        DocComponent docComponent;
        private void CreateGUI()
        {
            editText = new TextElement();
            editText.text = " Edit Mode";
            editText.style.borderBottomColor = Color.gray;
            editText.style.borderBottomWidth = 2f;
            viewText = new TextElement();
            viewText.text = " View Mode";
            viewText.style.borderBottomColor = Color.gray;
            viewText.style.borderBottomWidth = 2f;
            viewText.style.marginTop = 30;

            docComponent = new DocComponent();
            docEditView = createView();
            rootVisualElement.Add(docEditView);
        }

        TextElement editText, viewText;
        private VisualElement createView()
        {
            VisualElement root = new VisualElement();
            root.style.paddingLeft = 10;
            root.Add(editText);
            editView = new DocEditVisual(docComponent, (int)position.width-20);
            root.Add(editView);
            root.Add(viewText);
            var btn = new Button();
            btn.clicked += () =>
            {
                docComponent.FromVisual(editView[1]);
                var ve = docComponent.CreateViewGUI((int)position.width);
                ve.style.marginLeft = 10;
                rootVisualElement[0].RemoveAt(4);
                rootVisualElement[0].Add(ve);
            };
            btn.text = "Repaint";
            btn.style.width = 100;
            root.Add(btn);
            root.Add(docComponent.CreateViewGUI((int)position.width));
            return root;
        }
    }
}