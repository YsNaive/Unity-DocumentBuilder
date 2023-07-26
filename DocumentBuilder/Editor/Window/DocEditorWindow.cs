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
        public static DocEditorWindow Instance
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
        private void OnEnable()
        {
            SODocPageEditor.OnCreateEditor -= onCreateEditor;
            SODocPageEditor.OnCreateEditor += onCreateEditor;
        }
        private void OnDisable()
        {
            DocCache.Get().OpeningBookHierarchy = BookVisual.MenuHandler.GetState();
            DocCache.Save();
        }
        private void onCreateEditor(SODocPageEditor e)
        {
            if (BookVisual == null) return;
            e.OnSubPagesChange += () =>
            {
                BookVisual.MenuHandler.RootVisual.schedule.Execute(BookVisual.MenuHandler.Repaint);
            };
            e.OnSaveData += (page) =>
            {
                var find = BookVisual.MenuHandler.AddedVisual.Find(m => { return m.Target == Selection.activeObject; });
                if (find != null)
                    BookVisual.MenuHandler.Selecting = find;
            };
        }
        public static void RepaintMenu()
        {
            Instance.rootVisualElement.schedule.Execute(() =>
            {
                Instance.BookVisual.MenuHandler.Repaint();
            }).ExecuteLater(50);
        }
        private void CreateGUI()
        {
            SODocPageEditor.OnCreateEditor += onCreateEditor;
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
                    if (BookVisual.MenuHandler.AddedVisual.Contains(oldVal) || oldVal == null)
                        Selection.activeObject = newVal.Target;
                };
                rootVisualElement.Add(BookVisual);
                DocEditorData.Instance.EditingDocPage = (SODocPage)pageRootSelector.value;
                EditorUtility.SetDirty(DocEditorData.Instance);
            });
            rootVisualElement.Add(pageRootSelector);
            pageRootSelector.value = DocEditorData.Instance.EditingDocPage;
        }
    }
}
