using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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

        ObjectField rootPageSelector;
        VisualElement leftContainer, rightContainer;
        DSScrollView editorContainer;
        DocPageMenu pageMenu;
        DSToggle forceRepaintToggle;
        private void OnEnable()
        {
            if (rootPageSelector != null)
                rootPageSelector.value = DocEditorData.Instance.EditingDocPage;
        }
        private void OnDisable()
        {
        }
        private void CreateGUI()
        {
            initLayout();
            initRootPageSelector();
            initPageMenu();

            rootPageSelector.value = DocEditorData.Instance.EditingDocPage;
        }

        void initLayout()
        {
            rootVisualElement.style.backgroundColor = DocStyle.Current.BackgroundColor;
            leftContainer = new VisualElement();
            editorContainer = new DSScrollView();
            rightContainer = new VisualElement();
            rootPageSelector = new ObjectField() { objectType = typeof(SODocPage) };
            forceRepaintToggle = new DSToggle("Force Repaint") { value = false};
            menuItemDragHintLine = new VisualElement();
            menuItemDragHintLine.style.height = 2f;
            menuItemDragHintLine.style.width = Length.Percent(100);
            menuItemDragHintLine.style.backgroundColor = DocStyle.Current.SubFrontgroundColor;
            menuItemDragHintLine.style.position = Position.Absolute;

            leftContainer.Add(rootPageSelector);
            rightContainer.Add(forceRepaintToggle);

            var splitView = new SplitView(30);
            var splitView2 = new SplitView(FlexDirection.Row,75);
            splitView.Add(leftContainer);
            splitView.Add(splitView2);
            splitView2.Add(editorContainer);
            splitView2.Add(rightContainer);
            rootVisualElement.Add(splitView);
        }

        void initRootPageSelector()
        {
            rootPageSelector.RegisterValueChangedCallback(e =>
            {
                if(pageMenu!=null)
                    leftContainer.Remove(pageMenu);
                editorContainer.Clear();
                initPageMenu();
                DocEditorData.Instance.EditingDocPage = (SODocPage)e.newValue;
            });
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
                var editor = (SODocPageEditor)Editor.CreateEditor(e.TargetPage);
                var editorVisual = editor.CreateInspectorGUI();
                editor.IconField.RegisterValueChangedCallback(e =>
                {
                    pageMenu.Selecting.Repaint();
                });
                editorContainer.Add(editorVisual);
            };
            leftContainer.Add(pageMenu);
            pageMenu.Selecting = lastSelect;
            initPageMenuExtend();
        }
        void initPageMenuExtend()
        {
            foreach (var item in pageMenu.RootMenuItem.MenuItems())
            {
                item.TitleContainer.Add(createMenuItemDrager(item));

                var createBtn = DocRuntime.NewButton("+",DocStyle.Current.SuccessColor);
                createBtn.style.width = DocStyle.Current.LineHeight;
                createBtn.style.height = DocStyle.Current.LineHeight;
                createBtn.style.position = Position.Absolute;
                createBtn.style.right = 6 + DocStyle.Current.LineHeight.Value;
                createBtn.clicked += () => { createNewPagePopup(item); };
                item.TitleContainer.Add(createBtn);

                var deleteBtn = DocRuntime.NewButton("-", DocStyle.Current.DangerColor);
                deleteBtn.style.width = DocStyle.Current.LineHeight;
                deleteBtn.style.height = DocStyle.Current.LineHeight;
                deleteBtn.style.position = Position.Absolute;
                deleteBtn.style.right = 4;
                deleteBtn.clicked += () => { deletePagePopup(item, item.ParentMenuItem); };
                item.TitleContainer.Add(deleteBtn);
            }
        }
        void createNewPagePopup(DocPageMenuItem parentMenuItem)
        {
            editorContainer.Clear();
            parentMenuItem.IsOpen = true;
            pageMenu.SaveStateHierarchy();
            editorContainer.Add(new DocPageCreator(parentMenuItem.TargetPage, val =>
            {
                if (val != null)
                {
                    initPageMenu();
                }
                editorContainer.Clear();
                pageMenu.TrySelect(val);
            }));
        }
        void deletePagePopup(DocPageMenuItem deleteItem, DocPageMenuItem parentItem)
        {
            editorContainer.Clear();
            editorContainer.Add(new DocPageDeleter(deleteItem.TargetPage, e =>
            {
                if (e.isDelete)
                {
                    if(parentItem != null)
                        parentItem.TargetPage.SubPages.Remove(deleteItem.TargetPage);
                    initPageMenu();
                    Debug.Log($"Delete {e.deletedPage.Length} Pages and {e.deletedFolder.Length} Folders");
                }
                editorContainer.Clear();
            }));
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
            moveBtn.style.right = 7 + DocStyle.Current.LineHeight.Value * 2f;
            moveBtn.style.scale = new Scale(new Vector3(.9f, .9f, .9f));
            var manipulator = new CapturePointerManipulator();
            var posOffset = Vector2.zero;
            var stateBefore = false;
            int insertPlace = -1;

            manipulator.PointerDownEvent += e =>
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

            manipulator.PointerUpEvent += (evt) =>
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
