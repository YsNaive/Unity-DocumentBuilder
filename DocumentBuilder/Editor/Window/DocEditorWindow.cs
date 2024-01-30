using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditorWindow : EditorWindow, IHasCustomMenu
    {
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            GUIContent content = new GUIContent("Tutorial");
            menu.AddItem(content, false, beginTutorial);
        }

        class Data
        {
            public float SplitPercent1 = 30;
            public float SplitPercent2 = 70;
            public float SplitPercent3 = 30;
            public bool isFirstOpen = true;
        }
        Data cacheData;

        ObjectField rootPageSelector;
        VisualElement leftContainer, rightTopContainer, rightBottomContainer;
        VisualElement editorContainer;
        DocPageMenu pageMenu;
        DSButton createPageBtn, deletePageBtn;
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

            if (cacheData.isFirstOpen)
                beginTutorial();
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

            leftContainer.Add(rootPageSelector);
            rightTopContainer.Add(createPageBtn);
            rightTopContainer.Add(deletePageBtn);

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
                var editor = Editor.CreateEditor(e.TargetPage) as SODocPageEditor;
                var editorVisual = editor.CreateInspectorGUI();
                var space = new VisualElement();
                space.style.marginBottom = Screen.height / 2.5f;
                editorVisual.Add(space);
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
            EditorUtility.SetDirty(preSelect.TargetPage);
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
                    EditorUtility.SetDirty(pageToMove);
                    pageMenu?.RepaintMenuHierarchy();
                    initPageMenuExtend();
                }
            };
            moveBtn.AddManipulator(manipulator);
            return moveBtn;
        }

        #region Tutorial
        void beginTutorial()
        {
            rootPageSelector.value = null;
            leftContainer.SetEnabled(false);
            editorContainer.SetEnabled(false);
            rightTopContainer.SetEnabled(false);

            var container = _tutorialCoverMask();
            var title = new DSLabel("Welcome to DocumentEditor");
            title.style.unityTextAlign = TextAnchor.MiddleCenter;
            var subtitle = new DSTextElement("Do you need tutorial ?");
            subtitle.style.unityTextAlign = TextAnchor.MiddleCenter;
            var btnWidth = DocStyle.Current.MainTextSize * 10;
            var startBtn = new DSButton("Let's GO", DocStyle.Current.SuccessColor, () =>
            { rootVisualElement.Remove(container); _tutorialCreateBookRoot(); });
            startBtn.style.width = btnWidth;
            var skipBtn = new DSButton("Skip", DocStyle.Current.DangerColor, () =>
            {
                rootVisualElement.Remove(container);
                leftContainer.SetEnabled(true);
                editorContainer.SetEnabled(true);
                rightTopContainer.SetEnabled(true);
                rightBottomContainer.SetEnabled(true);
                cacheData.isFirstOpen = false;
            });
            skipBtn.style.width = btnWidth;
            var buttons = new DSHorizontal();
            buttons.Add(skipBtn);
            buttons.Add(startBtn);
            buttons.style.justifyContent = Justify.Center;
            buttons.style.marginTop = DocStyle.Current.LineHeight;
            buttons.style.marginBottom = DocStyle.Current.LineHeight;
            container.Add(title);
            container.Add(subtitle);
            container.Add(buttons);

            rootVisualElement.Add(container);
        }
        VisualElement _tutorialCoverMask()
        {
            var container = new VisualElement();
            container.style.width = Length.Percent(100);
            container.style.height = Length.Percent(100);
            container.style.position = Position.Absolute;
            container.style.justifyContent = Justify.Center;
            container.style.alignItems = Align.Center;
            container.style.backgroundColor = Color.black * 0.45f;
            return container;
        }
        VisualElement _tutorialMsgContainer(string msg)
        {
            var arrow = new VisualElement();
            arrow.style.SetIS_Style(DocStyle.Current.ArrowIcon);
            arrow.style.marginLeft = StyleKeyword.Auto;
            arrow.style.marginRight = StyleKeyword.Auto;
            arrow.style.rotate = new Rotate(-90);
            var selectMsgContainer = new VisualElement();
            selectMsgContainer.Add(arrow);
            selectMsgContainer.style.opacity = 0f;
            selectMsgContainer.Add(DocDescription.Create(msg, DocDescription.DescriptionType.Hint));
            return selectMsgContainer;
        }
        void _tutorialCreateBookRoot()
        {
            editorContainer.SetEnabled(true);
            leftContainer.SetEnabled(true);
            var page = AssetDatabase.LoadAssetAtPath<SODocPage>(AssetDatabase.GUIDToAssetPath("39afb8bcc6cde314baadbe99f5d69e4b"));
            var pageVisual = new DocPageVisual(page);
            editorContainer.Add(pageVisual);

            var msg = _tutorialMsgContainer("Select your DocPage as BookRoot here.");
            leftContainer.Add(msg);
            var animation = new VisualElementAnimation.GotoTransformPositionBackAndForth(msg, 0.75f, new Vector2(0, 7));

            pageVisual.panel.visualTree.schedule.Execute(() =>
            {
                msg.Fade(1, 750);
                animation.Start();
            }).ExecuteLater(2000);

            EventCallback<ChangeEvent<Object>> next = null;
            VisualElement msg2 = _tutorialMsgContainer("Click here to select editing page.");
            var animation2 = new VisualElementAnimation.GotoTransformPositionBackAndForth(msg2, 0.75f, new Vector2(0, 7));
            msg2.Fade(1, 750);
            next = evt =>
            {
                if (leftContainer.Contains(msg))
                {
                    leftContainer.Remove(msg);
                    animation.Stop();
                }
                pageMenu.RootMenuItem.Add(msg2);
                animation2.Stop();
                animation2.Start();
                Action<DocPageMenuItem> next2 = null;
                next2 = evt =>
                {
                    animation2.Stop();
                    pageMenu.RootMenuItem.Remove(msg2);
                    _tutorialEditContent();
                    rootPageSelector.SetEnabled(false);
                    rootPageSelector.UnregisterCallback(next);
                    pageMenu.OnSelected -= next2;
                }; pageMenu.OnSelected += next2;

            };  rootPageSelector.RegisterValueChangedCallback(next);
        }

        void _tutorialEditContent()
        {
            leftContainer.SetEnabled(false);
            var field = editorContainer.Q<DocComponentsField>();
            var msg = _tutorialMsgContainer("Click here to add new content.");
            field.Add(msg);
            var animation = new VisualElementAnimation.GotoTransformPositionBackAndForth(msg, 0.75f, new Vector2(0, 7));
            animation.Start();
            msg.Fade(1, 750);
            editorContainer[0][0].SetEnabled(false);
            var readMoreBtn = new DSButton("Read More", () =>
            {
                var container = _tutorialCoverMask();
                var page = AssetDatabase.LoadAssetAtPath<SODocPage>(AssetDatabase.GUIDToAssetPath("a08211148927d0f46a8f15c2cc30f33d"));
                var pageVisual = new DocPageVisual(page);
                container.Add(pageVisual);
                var backBtn = new DSButton("Back",DocStyle.Current.HintColor, () =>
                {
                    rootVisualElement.Remove(container);
                });
                pageVisual.style.marginLeft = Length.Percent(18);
                pageVisual.style.width = Length.Percent(60);
                backBtn.style.marginLeft = Length.Percent(18);
                backBtn.style.width = Length.Percent(60);
                pageVisual.style.backgroundColor = DocStyle.Current.BackgroundColor;
                container.Add(backBtn);
                rootVisualElement.Add(container);
            });

            var nextBtn = new DSButton("Next", DocStyle.Current.SuccessColor, () =>
            {
                field.Remove(msg);
                _tutorialAddPage();
            });

            Action created = null; created = () =>
            {
                msg[0].style.rotate = new Rotate(90);
                var hor = new DSHorizontal();
                hor.Add(msg[1]);
                hor.Add(readMoreBtn);
                hor.Add(nextBtn);
                hor[0].style.flexGrow = 1f;
                msg.Insert(0,hor);
                msg.Q<DSTextElement>().text = "Modify setting of Component here";
                field.Insert(field.childCount-3, msg);

                msg.Fade(1, 750);
                field.Q<Button>().clicked -= created;
            };
            field.Q<Button>().clicked+= created;  
        }

        void _tutorialAddPage()
        {
            leftContainer.SetEnabled(false);
            editorContainer.SetEnabled(false);
            rightTopContainer.SetEnabled(true);
            rightTopContainer[1].SetEnabled(false);
            var msg = _tutorialMsgContainer("Let add new page !");
            msg.style.position = Position.Absolute;
            msg.style.top = rightTopContainer[0].worldBound.height + 5;
            msg.style.left = DocStyle.Current.LineHeight;
            rightTopContainer.Add( msg);
            var animation = new VisualElementAnimation.GotoTransformPositionBackAndForth(msg, 0.75f, new Vector2(0, 7));
            animation.Start();
            msg.Fade(1, 750);
            var addBtn = (Button)rightTopContainer[0];
            Action addCallback = null;
            addCallback = () =>
            {
                if(rightTopContainer.Contains(msg))
                    rightTopContainer.Remove(msg);
                addBtn.SetEnabled(false);
                var btnHor = editorContainer[0][editorContainer[0].childCount - 1];
                editorContainer.SetEnabled(true);
                btnHor[1].SetEnabled(false);
                Action<SODocPage> callback = null;
                var creater = editorContainer.Q<DocPageCreator>();
                callback = (val) =>
                {

                    editorContainer.Clear();
                    var container = _tutorialCoverMask();
                    var title = DocLabel.Create("Congratulations !");
                    title.style.unityTextAlign = TextAnchor.MiddleCenter;
                    var subtitle = DocDescription.Create("Now you know the basic of DocumentEditor. There still are many thing you can try, enjoy !");
                    subtitle.style.unityTextAlign = TextAnchor.MiddleCenter;
                    container.Add(title);
                    container.Add(subtitle);
                    container.Add(new DSButton("Start Editing", () =>
                    {
                        leftContainer.SetEnabled(true);
                        editorContainer.SetEnabled(true);
                        rootPageSelector.SetEnabled(true);
                        rightTopContainer.SetEnabled(true);
                        rightTopContainer[0].SetEnabled(true);
                        rightTopContainer[1].SetEnabled(true);
                        rootVisualElement.Remove(container);
                        cacheData.isFirstOpen = false;
                    }));
                    container[2].style.width = Length.Percent(25);
                    container[2].style.marginTop = DocStyle.Current.LineHeight;
                    container[2].style.marginBottom = DocStyle.Current.LineHeight;
                    rootVisualElement.Add(container);
                    creater.callback -= callback;
                };  creater.callback += callback;
                addBtn.clicked -= addCallback;
            };  addBtn.clicked += addCallback;
        }
        #endregion
    }
}
