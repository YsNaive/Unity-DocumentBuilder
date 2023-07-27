using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
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
            if(BookVisual != null)
            {
                DocCache.Get().OpeningBookHierarchy = BookVisual.MenuHandler.GetState();
                DocCache.Save();
            }
        }
        bool viewAni = false;
        private void onCreateEditor(SODocPageEditor e)
        {
            inspector = e;
            if (BookVisual == null) return;
            IVisualElementScheduledItem scheduledItem = null;
            scheduledItem = rootVisualElement.schedule.Execute(() =>
            {
                if (e != null)
                    e.Save();
                else
                    scheduledItem.Pause();
            });
        }
        SODocPageEditor inspector;
        Vector2 pos = Vector2.zero;
        private void CreateGUI()
        {
            rootVisualElement.schedule.Execute(() =>
            {
                if (viewAni) return;
                if(BookVisual == null) return;
                if (BookVisual.MenuHandler == null) return;
                if (BookVisual.DisplayingPage !=null)
                    pos = BookVisual.DisplayingPage.scrollOffset;

                BookVisual.MenuHandler.Repaint();
                BookVisual.MenuHandler.Selecting = BookVisual.MenuHandler.Selecting;
            }).Every(500);
            Button playIntro = DocRuntime.NewButton("Play Intro", DocStyle.Current.HintColor, () =>
            {
                if (!viewAni)
                {
                    viewAni = true;
                    if (BookVisual.DisplayingPage != null)
                        BookVisual.DisplayingPage.PlayIntro(() => { viewAni = false; });
                }
            });
            Button playOuttro = DocRuntime.NewButton("Play Outtro", DocStyle.Current.HintColor, () =>
            {
                if (!viewAni)
                {
                    viewAni = true;
                    if (BookVisual.DisplayingPage != null)
                        BookVisual.DisplayingPage.PlayOuttro(() => { viewAni = false; });
                }
            });
            playIntro.style.position = Position.Absolute;
            playOuttro.style.position = Position.Absolute;
            playIntro.style.bottom = 25;
            playOuttro.style.bottom = 5;
            playIntro.style.right = 5;
            playOuttro.style.right = 5;
            playIntro.style.width = 80;
            playOuttro.style.width = 80;

            SODocPageEditor.OnCreateEditor += onCreateEditor;
            rootVisualElement.Clear();
            ObjectField pageRootSelector = new ObjectField("Root");
            pageRootSelector.objectType = typeof(SODocPage);
            pageRootSelector.RegisterValueChangedCallback(val =>
            {
                rootVisualElement.Clear();
                rootVisualElement.Add(playIntro);
                rootVisualElement.Add(playOuttro);
                rootVisualElement.Add(pageRootSelector);
                PageRoot = val.newValue as SODocPage;
                BookVisual = new DocBookVisual(PageRoot);
                BookVisual.MenuHandler.OnChangeSelect += (o, n) =>
                {
                    inspector?.Save();
                    if (o == null || ( o != n && BookVisual.MenuHandler.AddedVisual.Contains(o))) Selection.activeObject = n.Target;
                    if (BookVisual.DisplayingPage != null)
                        BookVisual.DisplayingPage.RegisterCallback<GeometryChangedEvent>(e =>
                        {
                            BookVisual.DisplayingPage.verticalScroller.highValue = float.MaxValue;
                            BookVisual.DisplayingPage.verticalScroller.value = pos.y;
                        });
                };
                BookVisual.DontPlayAnimation = true;
                rootVisualElement.Add(BookVisual);
                DocEditorData.Instance.EditingDocPage = (SODocPage)pageRootSelector.value;
                EditorUtility.SetDirty(DocEditorData.Instance);
            });
            rootVisualElement.Add(pageRootSelector);
            pageRootSelector.value = DocEditorData.Instance.EditingDocPage;

            rootVisualElement.Add(playIntro);
            rootVisualElement.Add(playOuttro);
            viewAni = false;
        }
    }
}
