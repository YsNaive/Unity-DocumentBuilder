using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageMenuVisual : VisualElement
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
        public DocPageMenuVisual(SODocPage page, PageMenuHandler menuHandler)
        {
            Target = page;
            MenuHandler ??= menuHandler;
            MenuHandler.Root ??= page;
            MenuHandler.RootVisual ??= this;
            VisualElement icon = new VisualElement();
            TextElement name = new TextElement();
            name.text = Target.name;
            name.style.SetIS_Style(DocStyle.Current.MainText);
            name.style.SetIS_Style(ISMargin.None);
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
                    name.style.position = Position.Absolute;
                }
            });
            Add(icon);
            Add(name);
            foreach(var subPage in Target.SubPages)
            {
                var ve = new DocPageMenuVisual(subPage, menuHandler);
                SubMenuVisual.Add(ve);
                Add(ve);
            }

            icon.RegisterCallback<MouseDownEvent>(e => { IsOpen = !IsOpen; });
        }

    }

}