using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocBookVisual : VisualElement
    {
        PageMenuVisual menuVisual;
        public PageMenuHandler MenuHandler = new PageMenuHandler();
        VisualElement divLineBar = new VisualElement();
        ScrollView menuScrollView = new ScrollView();
        public DocPageVisual DisplayingPage;
        float menuWidthPercent;
        float widthSpace = 15;
        public bool DontPlayAnimation = false;
        public bool IsPlayingAinmation { get; private set; }
        private bool isChangingWidth = false;
        public VisualElement ChapterMenu;
        public DocBookVisual(SODocPage rootPage)
        {
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            menuWidthPercent = DocCache.Get().DocMenuWidth;
            menuWidthPercent=Mathf.Clamp(menuWidthPercent, 0.01f, 1f);
            if (rootPage == null) return;
            style.SetIS_Style(ISFlex.Horizontal);
            divLineBar.style.width = 5;
            divLineBar.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            divLineBar.RegisterCallback<PointerDownEvent>(e =>{isChangingWidth = true;});
            RegisterCallback<PointerUpEvent>(e =>{isChangingWidth = false;});
            RegisterCallback<PointerLeaveEvent>(e => { isChangingWidth = false; });
            RegisterCallback<PointerMoveEvent>(e =>
            {
                if (isChangingWidth)
                {
                    menuWidthPercent = e.position.x/ layout.width;
                    menuWidthPercent = Mathf.Clamp(menuWidthPercent, 0.01f, 1f);
                    DocCache.Get().DocMenuWidth = menuWidthPercent;
                    setMenuWidth();
                }
            });
            

            MenuHandler.Root = rootPage;
            menuVisual = new PageMenuVisual(rootPage, MenuHandler);
            if (rootPage == MenuHandler.Root)
                menuVisual.style.SetIS_Style(ISMargin.Pixel(10));
            else
                menuVisual.style.SetIS_Style(ISMargin.None);
            menuVisual.style.SetIS_Style(ISPadding.None);
            MenuHandler.OnChangeSelect += (oldVal, newVal) =>
            {
                var newPage = new DocPageVisual(newVal.Target);
                newPage.style.SetIS_Style(ISMargin.None);
                newPage.style.width = (layout.width - widthSpace) * (1f - menuWidthPercent);
                newPage.style.position = Position.Relative;
                if (DontPlayAnimation)
                {
                    if (DisplayingPage != null)
                        Remove(DisplayingPage);
                    DisplayingPage = newPage;
                    Add(DisplayingPage);
                }
                else if (DisplayingPage != null)
                {
                    IsPlayingAinmation = true;
                    MenuHandler.LockSelect = true;
                    DisplayingPage.PlayOuttro(() =>
                    {
                        Remove(DisplayingPage);
                        DisplayingPage = newPage;
                        Add(DisplayingPage);
                        DisplayingPage.PlayIntro(() =>
                        {
                            IsPlayingAinmation = false;
                            MenuHandler.LockSelect = false;
                        });
                    });
                }
                else
                {
                    DisplayingPage = newPage;
                    Add(DisplayingPage);
                    IsPlayingAinmation = true;
                    MenuHandler.LockSelect = true;
                    DisplayingPage.PlayIntro(() =>
                    {
                        IsPlayingAinmation = false;
                        MenuHandler.LockSelect = false;
                    });
                }

                if(DisplayingPage != null)
                {
                    if(Contains(ChapterMenu))
                    Remove(ChapterMenu);
                    ChapterMenu = createChapterMenu();
                    Add(ChapterMenu);
                }
            };
            MenuHandler.SetState(DocCache.Get().OpeningBookHierarchy);
            menuScrollView.Add(menuVisual);
            menuScrollView.mode = ScrollViewMode.VerticalAndHorizontal;
            Add(menuScrollView);
            Add(divLineBar);

            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (e.oldRect.width != e.newRect.width)
                {
                    setMenuWidth();
                }
                if (e.oldRect.height != e.newRect.height)
                {
                    style.height = Screen.height;
                }
            });
        }
        void setMenuWidth()
        {
            menuScrollView.style.width = (layout.width - widthSpace) * menuWidthPercent;
            if (DisplayingPage != null)
            {
                DisplayingPage.style.width = (layout.width - widthSpace) * (1f - menuWidthPercent);
            }
        }

        VisualElement createChapterMenu()
        {
            var ve = new VisualElement();
            ve.style.position = Position.Absolute;
            ve.style.alignItems = Align.FlexStart;
            ve.style.right = 5;
            int displayLevel = -1;
            int lastLevel = 0;
            int comIndex=0;
            foreach (var com in DisplayingPage.Target.Components)
            {
                if (com.VisualID == "1")
                {
                    if (com.TextData.Count != 0)
                    {
                        var text = (DocRuntime.NewTextElement("- " + com.TextData[0]));
                        ve.Add(text);
                        DocLabel.Data data = JsonUtility.FromJson<DocLabel.Data>(com.JsonData);
                        data ??= new DocLabel.Data();

                        if(data.Level < lastLevel)
                        {
                            displayLevel = 0;
                        }
                        else if(data.Level > lastLevel)
                        {
                            displayLevel++;
                        }
                        lastLevel = data.Level;
                        text.style.marginLeft = displayLevel * DocStyle.Current.MainTextSize;
                        int localI = comIndex;
                        text.RegisterCallback<PointerDownEvent>(e =>
                        {
                            DisplayingPage.ScrollTo(DisplayingPage[localI]);
                            DisplayingPage[localI].Highlight(50, DocStyle.Current.SuccessTextColor);
                        });
                    }
                }
                comIndex++;
            }
            return ve;
        }
    }
}
