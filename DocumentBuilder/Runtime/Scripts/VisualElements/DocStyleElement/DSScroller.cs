using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSScroller : Scroller
    {
        public new class UxmlFactory : UxmlFactory<DSScroller, UxmlTraits> { }
        public new class UxmlTraits : Scroller.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ApplyStyle((Scroller)ve);
            }
        }
        public DSScroller()
        {
            ApplyStyle(this);
        }
        public new SliderDirection direction
        {
            get => base.direction;
            set
            {
                base.direction = value;
                ApplyDirection(this);
            }
        }
        public static void ApplyStyle(Scroller scroller)
        {
            ISBorder border = new ISBorder();
            scroller.slider.style.marginTop = 0;
            scroller.slider.style.marginBottom = 0;
            scroller.style.ClearMarginPadding();
            scroller.style.borderLeftWidth = 0;
            scroller.style.borderTopWidth = 0;
            scroller.highButton.style.display = DisplayStyle.None;
            scroller.lowButton.style.display = DisplayStyle.None;
            scroller.contentContainer.style.backgroundColor = Color.clear;
            var dragContainer = scroller.Q("unity-tracker");
            dragContainer.style.backgroundColor = new Color(0, 0, 0, 0.1f);
            dragContainer.style.SetIS_Style(border);
            scroller.Q("unity-dragger").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            ApplyDirection(scroller);
        }
        public static void ApplyDirection(Scroller scroller)
        {
            var drag = scroller.Q("unity-dragger");
            foreach (var ve in scroller.slider.contentContainer.Children())
            {
                if (scroller.direction == SliderDirection.Horizontal)
                {
                    ve.style.height = DocStyle.Current.ScrollerWidth;
                    ve.style.width = StyleKeyword.Auto;
                }
                else
                {
                    ve.style.height = StyleKeyword.Auto;
                    ve.style.width = DocStyle.Current.ScrollerWidth;
                }
                ve.style.backgroundColor = Color.clear;
                ve.style.ClearMarginPadding();
            }
            if (scroller.direction == SliderDirection.Horizontal)
            {
                scroller.slider.style.height = DocStyle.Current.ScrollerWidth;
                scroller.style.height = DocStyle.Current.ScrollerWidth;
                scroller.slider.style.width = StyleKeyword.Auto;
                scroller.style.width = StyleKeyword.Auto;

                drag.style.height = Length.Percent(80);
                drag.style.width = Length.Percent(25);
                drag.style.top = Length.Percent(10);
                drag.style.left = StyleKeyword.Auto;
            }
            else
            {
                scroller.slider.style.width = DocStyle.Current.ScrollerWidth;
                scroller.style.width = DocStyle.Current.ScrollerWidth;
                scroller.slider.style.height = StyleKeyword.Auto;
                scroller.style.height = StyleKeyword.Auto;

                drag.style.height = Length.Percent(25);
                drag.style.width = Length.Percent(80);
                drag.style.top = StyleKeyword.Auto;
                drag.style.left = Length.Percent(10);
            }
        }
    }
}