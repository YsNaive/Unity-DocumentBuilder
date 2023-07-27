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
        public DocPageVisual DisplayingPage;
        float menuWidthPercent = 0.3f;
        float widthSpace = 15;
        public bool DontPlayAnimation = false;
        public bool IsPlayingAinmation { get; private set; }
        public DocBookVisual(SODocPage rootPage)
        {
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            if (rootPage == null) return;
            style.SetIS_Style(ISFlex.Horizontal);
            divLineBar.style.width = 5;
            divLineBar.style.backgroundColor = DocStyle.Current.SubBackgroundColor;

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
                    return;
                }
                if (DisplayingPage != null)
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
            };
            MenuHandler.SetState(DocCache.Get().OpeningBookHierarchy);
            ScrollView menuScrollView = new ScrollView();
            menuScrollView.Add(menuVisual);
            menuScrollView.mode = ScrollViewMode.VerticalAndHorizontal;
            Add(menuScrollView);
            Add(divLineBar);

            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (e.oldRect.width != e.newRect.width)
                {
                    menuScrollView.style.width =( e.newRect.width- widthSpace) * menuWidthPercent;
                    if (DisplayingPage != null)
                    {
                        DisplayingPage.style.width = (e.newRect.width - widthSpace) * (1f- menuWidthPercent);
                    }
                }
                if (e.oldRect.height != e.newRect.height)
                {
                    style.height = Screen.height;
                }
            });
        }
    }
}
