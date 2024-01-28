using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace NaiveAPI.DocumentBuilder
{
    public sealed class DocPageMenuItem : VisualElement
    {
        public enum InitMode
        {
            Single,
            Tree,
        }
        public enum StyleType
        {
            None,
            Hover,
            Selected,
            ChildSelected,
        }
        public SODocPage TargetPage => m_page;
        public VisualElement SubItemContainer => m_subItemContainer;
        public VisualElement TitleContainer => m_titleContainer;
        public TextElement TitleText => nameText;
        public DocPageMenuItem ParentMenuItem => m_parentItem;

        public StyleType CurrentStyle
        {
            get => m_CurrentStyle;
            set
            {
                RepaintStyle(value);
                m_CurrentStyle = value;
            }
        }
        SODocPage m_page;
        VisualElement m_subItemContainer;
        VisualElement m_titleContainer;
        DocPageMenuItem m_parentItem;
        Image arrowImage, iconImage;
        TextElement nameText;
        StyleType m_CurrentStyle = StyleType.None;
        InitMode m_InitMode;
        public bool IsOpen
        {
            get => SubItemContainer.style.display == DisplayStyle.Flex;
            set
            {
                if(IsOpen != value)
                    InvertOpenState();
            }
        }
        public event Action OnStateChanged;
        public DocPageMenuItem(SODocPage page, InitMode mode = InitMode.Tree)
        {
            m_page = page;
            m_InitMode = mode;
            layoutInit();
            Repaint();
            if (mode == InitMode.Tree)
            {
                if (page.SubPages.Count != 0)
                    arrowImage.style.backgroundImage = DocStyle.Current.ArrowIcon.Background.StyleBackground;
                foreach (var p in page.SubPages)
                {
                    var newItem = new DocPageMenuItem(p);
                    newItem.m_parentItem = this;
                    SubItemContainer.Add(newItem);
                }
            }
        }
        private void layoutInit()
        {
            iconImage = new Image();
            //iconImage.style.scale = new Scale(new Vector3(.85f, .85f, .85f));
            nameText = new DSTextElement();
            nameText.style.flexGrow = 1;
            nameText.style.whiteSpace = WhiteSpace.NoWrap;
            nameText.style.marginLeft = DocStyle.Current.MainTextSize / 2f;
            m_titleContainer = new VisualElement();
            m_titleContainer.style.flexDirection = FlexDirection.Row;
            var imgHor = new VisualElement();
            imgHor.style.flexDirection = FlexDirection.Row;
            imgHor.Add(iconImage);
            m_titleContainer.Add(imgHor);
            m_titleContainer.Add(nameText);
            Add(m_titleContainer);

            if (m_InitMode == InitMode.Tree)
            {
                arrowImage = new Image();
                arrowImage.style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontgroundColor;
                arrowImage.style.scale = new Scale(new Vector3(.75f, .75f, .75f));
                arrowImage.style.opacity = .45f;
                imgHor.Insert(0, arrowImage);
                m_subItemContainer = new VisualElement();
                m_subItemContainer.style.display = DisplayStyle.None;
                Add(m_subItemContainer);
            }

            nameText.RegisterCallback<GeometryChangedEvent>(e =>
            {
                if ((e.newRect.height - e.oldRect.height) < 0.5) return;
                iconImage.style.width = e.newRect.height;
                iconImage.style.height = e.newRect.height;
                if (m_InitMode == InitMode.Tree)
                {
                    arrowImage.style.width = e.newRect.height;
                    arrowImage.style.height = e.newRect.height;
                    m_subItemContainer.style.marginLeft = e.newRect.height;
                }
            });
            imgHor.RegisterCallback<PointerDownEvent>(e => { InvertOpenState(); });
        }
        public bool InvertOpenState()
        {
            if (m_InitMode != InitMode.Tree) return false;
            bool newState = !IsOpen;
            arrowImage.style.rotate = new Rotate(newState ? 90 : 0);
            arrowImage.style.opacity = newState ? 1f : .45f;
            SubItemContainer.style.display = newState ? DisplayStyle.Flex : DisplayStyle.None;

            OnStateChanged?.Invoke();
            return newState;
        }
        public IEnumerable<DocPageMenuItem> MenuItems()
        {
            Queue<DocPageMenuItem> queue = new ();
            queue.Enqueue(this);
            DocPageMenuItem current;
            while (queue.TryDequeue(out current))
            {
                yield return current;
                foreach(DocPageMenuItem item in current.SubItemContainer.Children())
                    queue.Enqueue(item);
            }
        }
        public IEnumerable<(DocPageMenuItem item, string path)> MenuItemsAndPath()
        {
            Queue<(DocPageMenuItem item, string path)> queue = new();
            queue.Enqueue((this,"R"));
            (DocPageMenuItem item, string path) current;
            while (queue.TryDequeue(out current))
            {
                yield return current;
                int i = 0;
                foreach (DocPageMenuItem item in current.item.SubItemContainer.Children())
                {
                    queue.Enqueue((item, current.path + i.ToString()));
                    i++;
                }
            }
        }
        public IEnumerable<DocPageMenuItem> ParentMenuItems()
        {
            var current = ParentMenuItem;
            while (current!=null)
            {
                yield return current;
                current = current.ParentMenuItem;
            }
        }
        public void Repaint()
        {
            iconImage.style.backgroundImage = new StyleBackground(m_page.Icon);
            nameText.text = m_page.name;
        }
        public void RepaintStyle(StyleType state)
        {
            if (state == StyleType.None)
            {
                TitleContainer.style.backgroundColor = Color.clear;
                return;
            }
            if (state == StyleType.Hover)
            {
                var color = DocStyle.Current.HintColor;
                color.a = 0.25f;
                TitleContainer.style.backgroundColor = color;
                return;
            }
            if (state == StyleType.Selected)
            {
                var color = DocStyle.Current.SuccessColor;
                color.a = 0.75f;
                TitleContainer.style.backgroundColor = color;
                return;
            }
            if(state == StyleType.ChildSelected)
            {
                var color = DocStyle.Current.SuccessColor;
                color.a = 0.25f;
                TitleContainer.style.backgroundColor = color;
                return;
            }
        }
    }

}