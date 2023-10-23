using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;
namespace NaiveAPI.DocumentBuilder
{
    public class DSScrollView : ScrollView
    {
        public new class UxmlFactory : UxmlFactory<DSScrollView, UxmlTraits> { }
        public new class UxmlTraits : ScrollView.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ScrollView scroller = (ScrollView)ve;
                ApplyStyle(scroller);
            }
        }
        public DSScrollView()
        {
            contentContainer.style.width = StyleKeyword.Auto;
            style.minHeight = DocStyle.Current.LineHeight;
            ApplyStyle(this);
        }
        public static void ApplyStyle(ScrollView scrollView)
        {
            scrollView.style.ClearMarginPadding();
            DSScroller.ApplyStyle(scrollView.verticalScroller);
            DSScroller.ApplyStyle(scrollView.horizontalScroller);
        }
    }

}