using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;

namespace NaiveAPI.DocumentBuilder
{
    public class DSFoldout : Foldout
    {
        public new class UxmlFactory : UxmlFactory<DSFoldout, UxmlTraits> { }
        public new class UxmlTraits : Foldout.UxmlTraits { }
        public Toggle ToggleElement;
        public Image IconImage;
        public StyleBackground Icon
        {
            get => IconImage.style.backgroundImage;
            set
            {
                if(value.keyword == StyleKeyword.None)
                    IconImage.style.display = DisplayStyle.None;
                else
                    IconImage.style.display = DisplayStyle.Flex;
                IconImage.style.backgroundImage = value;
            }
        }
        public DSFoldout() {
            style.SetIS_Style(DocStyle.Current.MainTextStyle);
            contentContainer.style.minHeight = DocStyle.Current.LineHeight;
            contentContainer.style.paddingLeft = DocStyle.Current.MainTextSize;
            ToggleElement = this.Q<Toggle>();
            ToggleElement.style.ClearMarginPadding();
            ToggleElement.style.backgroundColor = DocStyle.Current.BackgroundColor * 0.65f;
            ToggleElement[0].focusable = false;
            var img = ToggleElement[0][0];
            img.style.backgroundImage = SODocStyle.WhiteArrow;
            img.style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontgroundColor;
            img.style.ClearMarginPadding();
            img.style.scale = new Scale(new Vector3(.7f, .7f, .7f));
            ToggleElement.RegisterValueChangedCallback(e =>
            {
                img.style.rotate = new Rotate(e.newValue ? 90 : 0);
            });
            schedule.Execute(() => { img.style.rotate = new Rotate(value ? 90 : 0); });

            var parent = ToggleElement[0];
            IconImage = new Image();
            IconImage.style.scale = new Scale(new Vector3(.85f, .85f, .85f));
            IconImage.style.display = DisplayStyle.None;
            parent.RegisterCallback<GeometryChangedEvent>(e =>
            {
                IconImage.style.width = e.newRect.height;
                IconImage.style.height = e.newRect.height;
            });
            parent.Insert(1, IconImage);
        }
        public DSFoldout(string text) : this()
        {
            this.text = text;
            ToggleElement[0].Q<Label>().style.marginLeft = DocStyle.Current.MainTextSize / 2f;
        }
        public DSFoldout(StyleBackground icon) : this() { Icon = icon; }
        public DSFoldout(string text, StyleBackground icon) : this(text) { Icon = icon; }

    }
}