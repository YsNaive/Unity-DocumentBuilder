using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
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
                if (m_isOpen != value)
                {
                    if (value)
                        foreach (var child in SubMenuVisual)
                            Add(child);
                    else
                        foreach (var child in SubMenuVisual)
                            Remove(child);
                }
                m_isOpen = value;
            }
        }
        private bool m_isOpen = true;
        public List<VisualElement> SubMenuVisual = new List<VisualElement>();
        public PageMenuHandler MenuHandler;
        public PageMenuVisual(SODocPage page, PageMenuHandler menuHandler)
        {
            Target = page;
            MenuHandler ??= menuHandler;
            MenuHandler.Root ??= page;
            MenuHandler.RootVisual ??= this;
            MenuHandler.Selecting ??= this;
            MenuHandler.AddedPages.Add(page);
            VisualElement icon = new VisualElement();
            TextElement name = new TextElement();
            name.text = Target.name;
            name.style.borderBottomColor = DocStyle.Current.FrontGroundColor;
            name.style.SetIS_Style(DocStyle.Current.MainText);
            name.style.SetIS_Style(ISMargin.None);
            name.style.whiteSpace = WhiteSpace.NoWrap;
            icon.style.SetIS_Style(ISMargin.None);
            if(Target.Icon != null) { icon.style.backgroundImage =  Target.Icon; }
            name.RegisterCallback<GeometryChangedEvent>(e =>
            {
                if(e.newRect.height != e.oldRect.height)
                {
                    icon.style.height = e.newRect.height;
                    icon.style.width = e.newRect.height;
                    icon.style.marginLeft = -5- e.newRect.height;
                    style.marginLeft = e.newRect.height + 5;
                    icon.style.position = Position.Absolute;
                }
            });
            name.RegisterCallback<PointerDownEvent>(e =>
            {
                repaintUnselect(MenuHandler.Selecting);
                repaintSelect(this);
                MenuHandler.Selecting = this;
            });


            Add(icon);
            Add(name);
            foreach(var subPage in Target.SubPages)
            {
                Debug.Log(MenuHandler.AddedPages.Count);
                if (!MenuHandler.AddedPages.Contains(subPage))
                {
                    var ve = new PageMenuVisual(subPage, menuHandler);
                    SubMenuVisual.Add(ve);
                    Add(ve);
                    MenuHandler.AddedPages.Add(subPage);
                }
            }

            icon.RegisterCallback<MouseDownEvent>(e => { IsOpen = !IsOpen; });
        }

        void repaintSelect(VisualElement visualElement)
        {
            visualElement.Q<TextElement>().style.borderBottomWidth = 3;
        }
        void repaintUnselect(VisualElement visualElement)
        {
            visualElement.Q<TextElement>().style.borderBottomWidth = 0;
        }
    }

}