using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

namespace NaiveAPI_Editor.DocumentBuilder
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
            dataText = new TextElement();
            dataText.text = " Data";
            dataText.style.borderBottomColor = Color.gray;
            dataText.style.borderBottomWidth = 2f;
            dataText.style.marginTop = 30;

            docComponent = new DocComponent();
            ScrollView scrollView = new ScrollView();
            scrollView.Add(createView());
            rootVisualElement.Add(scrollView);
        }

        TextElement editText, viewText, dataText;
        private VisualElement createView()
        {
            VisualElement root = new VisualElement();
            root.style.paddingLeft = 10;
            root.style.paddingRight = 10;
            root.Add(editText);
            root.Add(DocEditor.CreateEditVisual(docComponent));
            Button repaint = new Button();
            repaint.text = "Repaint";
            repaint.style.width = 80;
            repaint.clicked += () =>
            {
                root.RemoveAt(4);
                root.Insert(4,DocRuntime.CreateVisual(docComponent));
            };
            root.Add(viewText);
            root.Add(repaint);
            root.Add(DocRuntime.CreateVisual(docComponent));
            root.Add(dataText);
            root.Add(new IMGUIContainer(() =>
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField("ID: "+docComponent.VisualID);
                EditorGUILayout.LabelField("Text:");
                foreach(var str in docComponent.TextData)
                    EditorGUILayout.TextArea(str);
                EditorGUILayout.LabelField("Json:");
                EditorGUILayout.TextArea(docComponent.JsonData);
                EditorGUILayout.LabelField("Objs:");
                foreach (var obj in docComponent.ObjsData)
                {
                    EditorGUILayout.ObjectField(obj, typeof(Object), false);
                }
                EditorGUI.EndDisabledGroup();
            }));
            return root;
        }
    }
}