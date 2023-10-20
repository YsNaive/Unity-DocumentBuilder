using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocVisualDebugWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/DocumentBuilder/DocVisual Debugger", priority = 2)]
        public static void ShowWindow()
        {
            GetWindow<DocVisualDebugWindow>("DocVisual Debugger");
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
            dataText = new TextElement();
            dataText.text = " Data";
            dataText.style.borderBottomColor = Color.gray;
            dataText.style.borderBottomWidth = 2f;
            dataText.style.marginTop = 30;

            docComponent = new DocComponent();
            ScrollView scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            scrollView.contentContainer.style.minHeight = Length.Percent(100);
            scrollView.Add(createView());
            rootVisualElement.Add(scrollView);
        }

        TextElement editText, viewText, dataText;
        bool forceUpdate = false;
        private VisualElement createView()
        {
            VisualElement root = new VisualElement();
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.style.paddingLeft = 10;
            root.style.paddingRight = 10;
            root.style.flexGrow = 1;
            root.Add(editText);
            var comField = DocRuntime.NewEmpty();
            comField.Add(DocEditor.CreateComponentField(docComponent,true));
            root.Add(comField);
            root.schedule.Execute(() =>
            {
                if (this.forceUpdate)
                {
                    var ve = root.Q<DocVisual>();
                    int i = root.IndexOf(ve);
                    if (i == -1) return;
                    root.Remove(ve);
                    root.Insert(i, DocRuntime.CreateDocVisual(docComponent));
                }
            }).Every(250);
            var viewbar = DocRuntime.NewEmptyHorizontal();
            Toggle forceUpdate = new Toggle();
            forceUpdate.RegisterValueChangedCallback(e => { this.forceUpdate = e.newValue; });
            viewText.style.width = Length.Percent(50);
            forceUpdate.style.width = Length.Percent(25);
            forceUpdate.text = "Force Update";
            Button repaint = DocRuntime.NewButton("Repaint", () =>
            {
                var ve = root.Q<DocVisual>();
                int i = root.IndexOf(ve);
                if (ve != null)
                    root.Remove(ve);
                root.Insert(i, DocRuntime.CreateDocVisual(docComponent));
            });
            repaint.style.width = Length.Percent(25);
            viewbar.Add(viewText);
            viewbar.Add(forceUpdate);
            viewbar.Add(repaint);
            viewbar.style.marginTop = 30;
            viewbar.style.borderBottomColor = Color.gray;
            viewbar.style.borderBottomWidth = 2f;
            root.Add(viewbar);
            root.Add(DocRuntime.CreateDocVisual(docComponent));
            root.Add(dataText);
            root.Add(new IMGUIContainer(() =>
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField("ID: "+docComponent.VisualID);
                EditorGUILayout.LabelField("Text:");
                foreach(var str in docComponent.TextData)
                    EditorGUILayout.TextArea(str);
                EditorGUILayout.LabelField("Json:");
                GUI.skin.textArea.wordWrap = true;
                EditorGUILayout.TextArea(docComponent.JsonData);
                GUI.skin.textArea.wordWrap = false;
                EditorGUILayout.LabelField("Objs:");
                foreach (var obj in docComponent.ObjsData)
                {
                    EditorGUILayout.ObjectField(obj, typeof(Object), false);
                }
                EditorGUILayout.LabelField($"AniSettings Intro: {docComponent.IntroType}, {docComponent.IntroTime}    Outtro: {docComponent.OuttroType}, {docComponent.OuttroTime}");
                EditorGUILayout.LabelField($"Version: {docComponent.VisualVersion}");
                EditorGUI.EndDisabledGroup();
            }));
            return root;
        }
    }
}