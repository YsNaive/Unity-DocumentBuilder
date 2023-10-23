using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSDropdown : NaiveAPI_UI.CustomDropdown<string>
    {
        protected override VisualElement createMenuItem(string choice)
        {
            var text = new DSTextElement(choice);
            text.text = choice;

            text.RegisterCallback<PointerEnterEvent>(evt => { text.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
            text.RegisterCallback<PointerLeaveEvent>(evt => { text.style.backgroundColor = DocStyle.Current.BackgroundColor; });
            text.style.paddingLeft = DocStyle.Current.MainTextSize;
            text.style.height = DocStyle.Current.LineHeight;
            text.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
            text.style.borderBottomWidth = 1.5f;
            return text;
        }

        protected override VisualElement invokeDropdownElement => field;

        public DSDropdown(string label = "")
        {
            this.label.text = label;
        }
        DSTextElement label,field;
        protected override void initLabelAndField()
        {
            var hor = DocRuntime.NewEmptyHorizontal();
            label = new DSTextElement("");
            label.style.width = DocStyle.Current.LabelWidth;
            field = new DSTextElement("N/A");
            field.style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            field.style.flexGrow = 1f;
            hor.Add(label);
            hor.Add(field);
            Add(hor);
            var arrow = new Image();
            arrow.style.rotate = new Rotate(90);
            arrow.style.backgroundImage = new StyleBackground(SODocStyle.WhiteArrow);
            arrow.style.unityBackgroundImageTintColor = DocStyle.Current.FrontgroundColor;
            arrow.style.width = DocStyle.Current.LineHeight;
            arrow.style.height = DocStyle.Current.LineHeight;
            arrow.style.position = Position.Absolute;
            arrow.style.right = DocStyle.Current.MainTextSize/2f;
            field.Add(arrow);

        }

        protected override VisualElement createPopupContainer()
        {
            var scView = new VisualElement();
            scView.style.backgroundColor = DocStyle.Current.BackgroundColor;
            scView.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 2.5f));
            return scView;
        }

        protected override void onValueChanged()
        {
            field.text = value;
        }
    }

}