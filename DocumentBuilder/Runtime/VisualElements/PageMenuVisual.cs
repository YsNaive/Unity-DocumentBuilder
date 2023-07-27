using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class PageMenuVisual : VisualElement
    {
        public SODocPage Target;
        public bool IsOpen
        {
            get => m_isOpen;
            set
            {
                if (value == m_isOpen) return;
                if (value)
                    foreach (var child in SubMenuVisual)
                        Add(child);
                else
                    foreach (var child in SubMenuVisual)
                        Remove(child);
                openState.style.rotate = new Rotate(value?90:0);
                openState.visible = Target.SubPages.Count != 0;
                m_isOpen = value;
            }
        }
        private bool m_isOpen = true;
        
        public List<VisualElement> SubMenuVisual = new List<VisualElement>();
        public PageMenuHandler MenuHandler;
        VisualElement openState;
        public PageMenuVisual(SODocPage page, PageMenuHandler menuHandler)
        {
            Target = page;
            MenuHandler ??= menuHandler;
            MenuHandler.Root ??= page;
            MenuHandler.RootVisual ??= this;
            MenuHandler.AddedPages.Add(page);
            MenuHandler.AddedVisual.Add(this);
            VisualElement icon = new VisualElement();
            openState = new VisualElement();
            TextElement name = new TextElement();
            name.text = Target.name;
            name.style.borderBottomColor = DocStyle.Current.FrontGroundColor;
            name.style.SetIS_Style(DocStyle.Current.MainText);
            name.style.SetIS_Style(ISMargin.None);
            name.style.whiteSpace = WhiteSpace.NoWrap;
            icon.style.ClearMarginPadding();
            openState.style.ClearMarginPadding();
            openState.style.backgroundImage = DocStyle.WhiteArrow;
            if (Target.Icon != null) { icon.style.backgroundImage =  Target.Icon; }
            name.RegisterCallback<GeometryChangedEvent>(e =>
            {
                if(e.newRect.height != e.oldRect.height)
                {
                    float nameMargin = 0;
                    if(Target.Icon != null)
                    {
                        icon.style.height = e.newRect.height;
                        icon.style.width = e.newRect.height;
                        icon.style.position = Position.Absolute;
                        name.style.marginLeft = 2 * e.newRect.height;
                        name.style.marginLeft = e.newRect.height;
                        Add(icon);
                    }
                    nameMargin += e.newRect.height;
                    if (Target.SubPages.Count > 0)
                    {
                        openState.style.height = e.newRect.height;
                        openState.style.width = e.newRect.height;
                        openState.style.position = Position.Absolute;
                        openState.style.marginLeft = e.newRect.height+3;
                        openState.style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontGroundColor;
                        openState.RegisterCallback<MouseDownEvent>(e => { 
                            IsOpen = !IsOpen; 
                        });
                        Add(openState);
                        nameMargin += e.newRect.height+3;
                    }
                    style.marginLeft = e.newRect.height + 3;
                    name.style.marginLeft = nameMargin+3;
                    if (MenuHandler.RootVisual == this)
                    {
                        style.marginLeft = nameMargin - 2*e.newRect.height;
                    }
                }
            });
            name.RegisterCallback<PointerDownEvent>(e => { MenuHandler.Selecting = this; });

            if (MenuHandler.RootVisual == this)
            {
                MenuHandler.OnChangeSelect += (oldval, newval) =>
                {
                    if (oldval != null)
                        oldval.RepaintUnselect();
                    newval.RepaintSelect();
                };
            }

            Add(name);
            foreach(var subPage in Target.SubPages)
            {
                if (subPage == null) continue;
                    if (!MenuHandler.AddedPages.Contains(subPage))
                {
                    var ve = new PageMenuVisual(subPage, menuHandler);
                    SubMenuVisual.Add(ve);
                    Add(ve);
                }
            }
            IsOpen = false;
        }

        public void RepaintSelect()
        {
            var ve = this.Q<TextElement>();
            Color c = DocStyle.Current.SuccessColor;
            c.a = 0.5f;
            if (ve != null)
                ve.style.backgroundColor = c;
        }
        public void RepaintUnselect()
        {
            var ve = this.Q<TextElement>();
            if (ve != null)
                ve.style.backgroundColor = Color.clear;
        }

    }

}