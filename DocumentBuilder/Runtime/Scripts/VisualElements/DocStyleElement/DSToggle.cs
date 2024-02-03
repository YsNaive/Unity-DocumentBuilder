using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSToggle : Toggle
    {
        public DSToggle() :base()
        {
            style.minHeight = DocStyle.Current.LineHeight;
            style.SetIS_Style(DocStyle.Current.MainTextStyle);
            var checkMark = this.Q("unity-checkmark");
            checkMark.style.SetIS_Style(new ISBorder(DocStyle.Current.SubBackgroundColor, 1f));
            checkMark.style.backgroundColor = DocStyle.Current.InputFieldStyle.Background.ImageTint;
            checkMark.style.unityBackgroundImageTintColor = DocStyle.Current.MainText.Color;
            checkMark.style.width = DocStyle.Current.LineHeight;
            checkMark.style.height = DocStyle.Current.LineHeight;
            checkMark.style.scale = new Scale(new Vector3(.7f, .7f, .7f));
        }
        public DSToggle(string label) : this()
        {
            this.label = label;
            this.labelElement.style.SetIS_Style(DocStyle.Current.MainTextStyle);
            this.labelElement.style.minWidth = DocStyle.Current.LabelWidth;
            this.labelElement.style.width = DocStyle.Current.LabelWidth;
        }
    }
}
