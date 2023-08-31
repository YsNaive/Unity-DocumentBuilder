using NaiveAPI_UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class StringDropdown : CustomDropdown<string>
    {
        public StringDropdown(string label = "") : base(label) { }
        public override VisualElement CreatePopupElement()
        {
            TextElement ve = DocRuntime.NewTextElement(Value);
            ve.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            return ve;
        }

        public override VisualElement CreateSelectableItem(string choice, int i)
        {
            TextElement ve = DocRuntime.NewTextElement(choice);
            if (i % 2 == 1)
                ve.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            ISBorder clear = new ISBorder(Color.clear, 3);
            ISBorder highlight = new ISBorder(DocStyle.Current.SubFrontgroundColor, 3);
            ve.style.ClearMarginPadding();
            ve.style.SetIS_Style(clear);
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