using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditorWindow : EditorWindow
    {
        #region get window
        [MenuItem("Tools/NaiveAPI/DocumentBuilder/Document Editor")]
        public static void ShowWindow()
        {
            m_editorInstance = GetWindow<DocEditorWindow>("Document Editor");
            m_InspectorInstance = GetWindow<DocInspectorWindow>("Document Inspector");
        }
        public static DocEditorWindow EditorInstance
        {
            get
            {
                m_editorInstance??= GetWindow<DocEditorWindow>("Document Editor");
                return m_editorInstance;
            }
        }
        public static DocInspectorWindow InspectorInstance
        {
            get
            {
                m_InspectorInstance ??= GetWindow<DocInspectorWindow>("Document Inspector");
                return m_InspectorInstance;
            }
        }
        static DocEditorWindow m_editorInstance;
        static DocInspectorWindow m_InspectorInstance;
        #endregion

        public SODocPage PageRoot;
        public DocBookVisual BookVisual;
        private void CreateGUI()
        {
            ObjectField pageRootSelector = new ObjectField("Root");
            pageRootSelector.objectType = typeof(SODocPage);
            pageRootSelector.RegisterValueChangedCallback(val =>
            {
                rootVisualElement.Clear();
                rootVisualElement.Add(pageRootSelector);
                PageRoot = val.newValue as SODocPage;
                BookVisual = new DocBookVisual(PageRoot);
                BookVisual.MenuHandler.OnChangeSelect += (oldVal, newVal) =>
                {
                    InspectorInstance.rootVisualElement.Clear();
                    ScrollView scrollView = new ScrollView();
                    SODocPageEditor editor = (SODocPageEditor)Editor.CreateEditor(newVal.Target);
                    editor.OnSaveData += tar =>
                    {
                        BookVisual.MenuHandler.Selecting = BookVisual.MenuHandler.Selecting;
                    };
                    scrollView.Add(editor.CreateInspectorGUI());
                    InspectorInstance.rootVisualElement.Add(scrollView);
                };
                rootVisualElement.Add(BookVisual);
            });
            rootVisualElement.Add(pageRootSelector);
            pageRootSelector.value = DocEditorData.instance.EditingDocPage;
        }

        private void OnDisable()
        {
            m_editorInstance = null;
            if(m_InspectorInstance != null)
                m_InspectorInstance.Close();
        }
        public static void TryClose()
        {
            m_InspectorInstance = null;
            if (m_editorInstance != null)
                m_editorInstance.Close();
        }
    }
    public class DocInspectorWindow : EditorWindow
    {
        private void OnDisable()
        {
            DocEditorWindow.TryClose();
        }
    }
}
