using NaiveAPI_UI;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class StringDropdown : CustomDropdown<string>
    {
        public static Color EvenItemColor;
        public static Color OddItemColor;
        public static Color ItemHighlightColor;
        public static Color ItemTextColor;
        static StringDropdown()
        {
            Func<Color, Color> invert = color =>
            {
                float h, s, v;
                Color.RGBToHSV(color, out h, out s, out v);
                v = 1f - v;
                return Color.HSVToRGB(h, s, v);
            };

            EvenItemColor = invert(DocStyle.Current.SubBackgroundColor);
            OddItemColor = invert(DocStyle.Current.BackgroundColor);
            ItemTextColor = invert(DocStyle.Current.MainText.Color);
            ItemHighlightColor = invert(DocStyle.Current.FrontgroundColor);

            DocStyle.OnStyleChanged += style =>
            {
                EvenItemColor = invert(DocStyle.Current.SubBackgroundColor);
                OddItemColor = invert(DocStyle.Current.BackgroundColor);
                ItemTextColor = invert(DocStyle.Current.MainText.Color);
                ItemHighlightColor = invert(DocStyle.Current.FrontgroundColor);
            };
        }
        public StringDropdown(string label = "") : base(label) { }
        public override VisualElement CreatePopupElement()
        {
            TextElement ve = new DocTextElement(Value);
            ve.style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            ve.style.unityBackgroundImageTintColor = Color.clear;
            ve.style.SetIS_Style(new ISBorder(DocStyle.Current.BackgroundColor, DocStyle.Current.MainTextSize / 6f));
            return ve;
        }

        public override VisualElement CreateSelectableItem(string choice, int i)
        {
            TextElement ve = new DocTextElement(choice);
            ve.style.color = ItemTextColor;
            if (i % 2 == 1)
                ve.style.backgroundColor = OddItemColor;
            else
                ve.style.backgroundColor = EvenItemColor;
            ISBorder clear = new ISBorder(Color.clear, 3);
            ISBorder highlight = new ISBorder(ItemHighlightColor, 3);
            ve.style.ClearMarginPadding();
            ve.style.SetIS_Style(clear);
            ve.style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize/4));
            ve.RegisterCallback<PointerEnterEvent>(e => { ve.style.SetIS_Style(highlight); });
            ve.RegisterCallback<PointerLeaveEvent>(e => { ve.style.SetIS_Style(clear); });
            return ve;
        }

        public override void OnSelectChanged(string newValue)
        {
            ((TextElement)PopupElement).text = newValue;
        }
    }
}