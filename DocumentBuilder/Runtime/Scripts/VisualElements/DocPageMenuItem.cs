using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;

namespace NaiveAPI.DocumentBuilder
{
    public sealed class DocPageMenuItem : VisualElement
    {
        public SODocPage TargetPage => m_page;
        private SODocPage m_page;
        Image arrowImage, iconImage;
        TextElement nameText;
        public VisualElement SubItemContainer => m_subItemContainer;
        VisualElement m_subItemContainer;
        public bool IsOpen
        {
            get => SubItemContainer.style.display == DisplayStyle.Flex;
            set => SubItemContainer.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }
        public DocPageMenuItem(SODocPage page)
        {
            m_page = page;
            layoutInit();
            iconImage.style.backgroundImage = new StyleBackground(page.Icon);
            nameText.text = page.name;
            if (page.SubPages.Count != 0)
            {
                arrowImage.style.backgroundImage = new StyleBackground(SODocStyle.WhiteArrow);
                foreach (var p in page.SubPages)
                {
                    SubItemContainer.Add(new DocPageMenuItem(p));
                }
            }
        }
        private void layoutInit()
        {
            m_subItemContainer = new VisualElement();
            arrowImage = new Image();
            arrowImage.style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontgroundColor;
            arrowImage.style.scale = new Scale(new Vector3(.75f, .75f, .75f));
            iconImage = new Image();
            //iconImage.style.scale = new Scale(new Vector3(.85f, .85f, .85f));
            nameText = new DocTextElement();
            nameText.style.flexGrow = 1;
            nameText.style.textOverflow = TextOverflow.Ellipsis;
            var hor = new VisualElement();
            hor.style.flexDirection = FlexDirection.Row;
            var imgHor = new VisualElement();
            imgHor.style.flexDirection = FlexDirection.Row;
            imgHor.Add(arrowImage);
            imgHor.Add(iconImage);
            hor.Add(imgHor);
            hor.Add(nameText);
            Add(hor);
            Add(m_subItemContainer);

            nameText.RegisterCallback<GeometryChangedEvent>(e =>
            {
                if ((e.newRect.height - e.oldRect.height) < 0.5) return;
                arrowImage.style.width = e.newRect.height;
                arrowImage.style.height = e.newRect.height;
                iconImage.style.width = e.newRect.height;
                iconImage.style.height = e.newRect.height;
                m_subItemContainer.style.marginLeft = e.newRect.height/2f;
            });
            imgHor.RegisterCallback<PointerDownEvent>(e =>
            {
                IsOpen = !IsOpen;
                arrowImage.style.rotate = new Rotate(IsOpen ? 90 : 0);
            });
        }
    }

}