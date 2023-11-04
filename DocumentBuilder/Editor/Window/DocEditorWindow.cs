using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditorWindow : EditorWindow
    {
        #region get window
        [MenuItem("Tools/NaiveAPI/DocumentBuilder/Document Editor", priority = 1)]
        public static void ShowWindow()
        {
            m_editorInstance = GetWindow<DocEditorWindow>("Document Editor");
        }
        public static DocEditorWindow Instance
        {
            get
            {
                m_editorInstance ??= GetWindow<DocEditorWindow>("Document Editor");
                return m_editorInstance;
            }
        }
        static DocEditorWindow m_editorInstance;
        #endregion

        class Data
        {
            public float SplitPercent1 = 30;
            public float SplitPercent2 = 70;
            public float SplitPercent3 = 30;
        }
        Data cacheData;

        ObjectField rootPageSelector;
        VisualElement leftContainer, rightTopContainer, rightBottomContainer;
        VisualElement editorContainer;
        DocPageMenu pageMenu;
        DSButton createPageBtn, deletePageBtn, openOnInspectorBtn;
        private void OnEnable()
        {
            cacheData = JsonUtility.FromJson<Data>(DocCache.LoadData("DocumentEditorWindowCache.json"));
            cacheData ??= new();
            if (rootPageSelector != null)
                rootPageSelector.value = DocEditorData.Instance.EditingDocPage;
        }
        private void OnDisable()
        {
            if (splitView != null)
            {
                cacheData.SplitPercent1 = splitView.SplitPercent;
                cacheData.SplitPercent2 = splitView2.SplitPercent;
                cacheData.SplitPercent3 = splitView3.SplitPercent;
            }
            DocCache.SaveData("DocumentEditorWindowCache.json", JsonUtility.ToJson(cacheData));
        }
        private void CreateGUI()
        {
            initLayout();
            initRootPageSelector();
            initPageMenu();

            rootPageSelector.value = DocEditorData.Instance.EditingDocPage;
        }
        SplitView splitView, splitView2, splitView3;
        void initLayout()
        {
            rootVisualElement.style.backgroundColor = DocStyle.Current.BackgroundColor;
            leftContainer = new VisualElement();
            rightBottomContainer = new VisualElement();
            editorContainer = new DSScrollView() { verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible};
            rightTopContainer = new VisualElement();
            rightTopContainer.style.SetIS_Style(ISPadding.Pixel((int)(DocStyle.Current.MainTextSize / 2f)));
            rootPageSelector = new ObjectField() { objectType = typeof(SODocPage) };
            menuItemDragHintLine = new VisualElement();
            menuItemDragHintLine.style.height = 2f;
            menuItemDragHintLine.style.width = Length.Percent(100);
            menuItemDragHintLine.style.backgroundColor = DocStyle.Current.SubFrontgroundColor;
            menuItemDragHintLine.style.position = Position.Absolute;

            createPageBtn = new DSButton("Create Page", DocStyle.Current.SuccessColor);
            createPageBtn.clicked += createPage;
            deletePageBtn = new DSButton("Delete Page", DocStyle.Current.DangerColor);
            deletePageBtn.clicked += deletePage;
            openOnInspectorBtn = new DSButton("Open on Inspector", DocStyle.Current.HintColor);
            openOnInspectorBtn.clicked += openOnInspector;

            leftContainer.Add(rootPageSelector);
            rightTopContainer.Add(createPageBtn);
            rightTopContainer.Add(deletePageBtn);
            rightTopContainer.Add(openOnInspectorBtn);

            splitView = new SplitView(cacheData.SplitPercent1);
            splitView2 = new SplitView(cacheData.SplitPercent2);
            splitView3 = new SplitView(FlexDirection.Column, cacheData.SplitPercent3);
            splitView.Add(leftContainer);
            splitView.Add(splitView2);
            splitView2.Add(editorContainer);
            splitView2.Add(splitView3);
            splitView3.Add(rightTopContainer);
            splitView3.Add(rightBottomContainer);
            rootVisualElement.Add(splitView);

            emptySelectingWarning = DocVisual.Create(DocDescription.CreateComponent
            ("Please Select a Page first", DocDescription.DescriptionType.Warning));
            emptySelectingWarning.style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize));
        }

        void initRootPageSelector()
        {
            rootPageSelector.RegisterValueChangedCallback(e =>
            {
                if (pageMenu != null)
                    if (leftContainer.Contains(pageMenu))
                        leftContainer.Remove(pageMenu);
                editorContainer.Clear();
                initPageMenu();
                DocEditorData.Instance.EditingDocPage = (SODocPage)e.newValue;
                EditorUtility.SetDirty(DocEditorData.Instance);
            });
        }
        DocPageVisual preview;
        void rescale()
        {
            if (preview == null) return;
            if (rightBottomContainer.worldBound.width >= editorContainer.worldBound.width) return;
            var scale = rightBottomContainer.worldBound.width / editorContainer.worldBound.width;
            preview.style.minWidth = editorContainer.worldBound.width;
            preview.style.width = editorContainer.worldBound.width;
            var height = rightBottomContainer.worldBound.height / scale;
            preview.style.minHeight = rightBottomContainer.worldBound.height / scale;
            preview.style.height = rightBottomContainer.worldBound.height / scale;
            preview.style.scale = new Scale(new Vector3(scale, scale, scale));
            preview.style.transformOrigin = new TransformOrigin(0, 0);
        }
        void initPreview(SODocPage page)
        {
            preview = new DocPageVisual(page);
            preview.schedule.Execute(() =>
            {
                preview.Repaint();
            }).Every(1000);
            rightBottomContainer.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                rescale();
            });
            rescale();
        }
        void initPageMenu()
        {
            if (rootPageSelector.value == null) return;
            DocPageMenuItem lastSelect = null;
            if (pageMenu != null && leftContainer.Contains(pageMenu))
            {
                leftContainer.Remove(pageMenu);
                lastSelect = pageMenu.Selecting;
            }
            pageMenu = new DocPageMenu((SODocPage)rootPageSelector.value);
            pageMenu.EnableEmptySelecting = true;
            pageMenu.EnableDisplayingRootChange = false;
            pageMenu.EnableAutoHierarchySave = true;

            pageMenu.OnSelected += e =>
            {
                editorContainer.Clear();
                Editor orgEditor = null;
                Editor.CreateCachedEditor(e.TargetPage, typeof(SODocPageEditor),ref orgEditor);
                var editor = orgEditor as SODocPageEditor;
                var editorVisual = editor.CreateInspectorGUI();
                editor.IconField.RegisterValueChangedCallback(e =>
                {
                    pageMenu.Selecting.Repaint();
                });
                editorContainer.Add(editorVisual);
                rightBottomContainer.Clear();
                initPreview(e.TargetPage);
                rightBottomContainer.Add(preview);
                if (Selection.activeObject == e.TargetPage)
                    Selection.activeObject = null;
            };
            leftContainer.Add(pageMenu);
            pageMenu.Selecting = lastSelect;
            initPageMenuExtend();
        }
        void initPageMenuExtend()
        {
            foreach (var item in pageMenu.RootMenuItem.MenuItems())
                item.TitleContainer.Add(createMenuItemDrager(item));
        }

        DocVisual emptySelectingWarning;
        void createPage()
        {
            editorContainer.Clear();
            if (pageMenu.Selecting == null)
            {
                editorContainer.Add(emptySelectingWarning);
                return;
            }
            pageMenu.Selecting.IsOpen = true;
            pageMenu.SaveStateHierarchy();
            var preSelect = pageMenu.Selecting;
            editorContainer.Add(new DocPageCreator(pageMenu.Selecting.TargetPage, val =>
            {
                if (val != null)
                {
                    initPageMenu();
                }
                editorContainer.Clear();
                if (val != null)
                    pageMenu.TrySelect(val);
                else
                    pageMenu.Selecting = preSelect;
            }));
        }
        void deletePage()
        {
            editorContainer.Clear();
            if (pageMenu.Selecting == null)
            {
                editorContainer.Add(emptySelectingWarning);
                return;
            }
            if(pageMenu.Selecting == pageMenu.RootMenuItem)
            {
                editorContainer.Add(DocVisual.Create(DocDescription.CreateComponent(
                    "<b>You can not delete the root page</b>", DocDescription.DescriptionType.Warning)));
                return;
            }
            editorContainer.Add(new DocPageDeleter(pageMenu.Selecting.TargetPage, e =>
            {
                if (e.isDelete)
                {
                    if (pageMenu.Selecting != null)
                        pageMenu.Selecting.ParentMenuItem.TargetPage.SubPages.Remove(pageMenu.Selecting.TargetPage);
                    initPageMenu();
                    Debug.Log($"Delete {e.deletedPage.Length} Pages and {e.deletedFolder.Length} Folders");
                }
                editorContainer.Clear();
            }));
        }
        void openOnInspector()
        {
            if(pageMenu.Selecting != null)
            {
                editorContainer.Clear();
                Selection.activeObject = pageMenu.Selecting.TargetPage;
                var cur = pageMenu.Selecting;
                while(cur !=null)
                {
                    cur.RepaintStyle(DocPageMenuItem.StyleType.None);
                    cur = cur.ParentMenuItem;
                }
            }
        }

        VisualElement menuItemDragHintLine;
        VisualElement createMenuItemDrager(DocPageMenuItem item)
        {
            var moveBtn = new DSLabel("=");
            moveBtn.style.unityTextAlign = TextAnchor.MiddleCenter;
            moveBtn.style.opacity = 0.85f;
            moveBtn.style.backgroundColor = DocStyle.Current.HintColor;
            moveBtn.style.width = DocStyle.Current.LineHeight;
            moveBtn.style.height = DocStyle.Current.LineHeight;
            moveBtn.style.position = Position.Absolute;
            moveBtn.style.right = 3;
            moveBtn.style.scale = new Scale(new Vector3(.9f, .9f, .9f));
            var manipulator = new CapturePointerManipulator();
            var posOffset = Vector2.zero;
            var stateBefore = false;
            int insertPlace = -1;

            manipulator.OnEnable += () =>
            {
                posOffset = leftContainer.worldBound.position;
                leftContainer.Add(menuItemDragHintLine);
                item.style.opacity = .5f;
                stateBefore = item.IsOpen;
                item.IsOpen = false;
                menuItemDragHintLine.style.width = 0;
            };

            manipulator.ActiveMoveEvent += (evt) =>
            {
                insertPlace = -1;
                if (pageMenu == null) return;
                if(pageMenu.Hovering == null) return;
                if (pageMenu.Hovering == item) return;
                if (pageMenu.Hovering == pageMenu.RootMenuItem) return;

                var targetHeight = pageMenu.Hovering.TitleContainer.worldBound.height;
                var targetWidth = pageMenu.Hovering.TitleContainer.worldBound.width;
                var targetWorldPos = pageMenu.Hovering.TitleContainer.worldBound.position;
                var mouseOnTargetLocalPos = (Vector2)evt.position - targetWorldPos;
                if (mouseOnTargetLocalPos.y < targetHeight * .3f)
                    insertPlace = 0; // Before
                else if (mouseOnTargetLocalPos.y < targetHeight * .7f)
                    insertPlace = 1; // Inside
                else if (mouseOnTargetLocalPos.y < targetHeight * 1.05f)
                    insertPlace = 2; // Next
                else
                    insertPlace = -1; // Out of Range

                var leftOffset = 0f;
                if(pageMenu.Hovering.parent != null)
                {
                    leftOffset = pageMenu.Hovering.parent.worldBound.xMin - posOffset.x;
                }
                if(insertPlace == 0)
                {
                    menuItemDragHintLine.style.top = targetWorldPos.y - posOffset.y;
                    menuItemDragHintLine.style.left = leftOffset;
                    menuItemDragHintLine.style.width = targetWidth;
                }
                else if(insertPlace == 1)
                {
                    menuItemDragHintLine.style.top = targetWorldPos.y - posOffset.y + targetHeight;
                    var indent = DocStyle.Current.LineHeight.Value * 2f;
                    menuItemDragHintLine.style.left = leftOffset + indent;
                    menuItemDragHintLine.style.width = targetWidth - indent;
                }
                else if (insertPlace == 2)
                {
                    menuItemDragHintLine.style.top = targetWorldPos.y - posOffset.y + targetHeight;
                    menuItemDragHintLine.style.left = leftOffset;
                    menuItemDragHintLine.style.width = targetWidth;
                }
                else
                {
                    menuItemDragHintLine.style.width = 0;
                }
            };

            manipulator.OnDisable += () =>
            {
                if (leftContainer == menuItemDragHintLine.parent)
                    leftContainer.Remove(menuItemDragHintLine);
                item.style.opacity = 1f;
                item.IsOpen = stateBefore;

                if(insertPlace != -1)
                {
                    var pageToMove = item.TargetPage;
                    var toPage = pageMenu.Hovering.ParentMenuItem.TargetPage;
                    item.ParentMenuItem.TargetPage.SubPages.Remove(pageToMove);
                    if (insertPlace == 0)
                    {
                        var parentList = pageMenu.Hovering.ParentMenuItem.TargetPage.SubPages;
                        parentList.Insert(parentList.IndexOf(pageMenu.Hovering.TargetPage), pageToMove);
                    }
                    else if(insertPlace == 1)
                    {
                        pageMenu.Hovering.TargetPage.SubPages.Add(pageToMove);
                        toPage = pageMenu.Hovering.TargetPage;
                    }
                    else if(insertPlace == 2)
                    {
                        var parentList = pageMenu.Hovering.ParentMenuItem.TargetPage.SubPages;
                        parentList.Insert(parentList.IndexOf(pageMenu.Hovering.TargetPage) + 1, pageToMove);
                    }

                    DocPageEditorUtils.MovePageAsset(pageToMove, DocPageEditorUtils.ValidSubPageFolderPath(toPage));
                    pageMenu?.RepaintMenuHierarchy();
                    initPageMenuExtend();
                }
            };
            moveBtn.AddManipulator(manipulator);
            return moveBtn;
        }
    }
}
