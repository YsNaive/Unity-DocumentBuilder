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
        }
        public static DocEditorWindow EditorInstance
        {
            get
            {
                m_editorInstance??= GetWindow<DocEditorWindow>("Document Editor");
                return m_editorInstance;
            }
        }
        static DocEditorWindow m_editorInstance;
        #endregion

        public SODocPage PageRoot;
        public DocBookVisual BookVisual;
        private void CreateGUI()
        {
            SODocPageEditor.OnCreateEditor += e =>
            {
                e.OnSaveData += tar =>
                {
                    BookVisual.MenuHandler.Selecting = BookVisual.MenuHandler.Selecting;
                };
                e.OnSubPagesChange += () =>
                {
                    BookVisual.MenuHandler.RootVisual.schedule.Execute(BookVisual.MenuHandler.Repaint);
                };
            };
            rootVisualElement.Clear();
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
                    Selection.activeObject = newVal.Target;
                };
                rootVisualElement.Add(BookVisual);
            });
            rootVisualElement.Add(pageRootSelector);
            pageRootSelector.value = DocEditorData.Instance.EditingDocPage;
        }
    }
}
