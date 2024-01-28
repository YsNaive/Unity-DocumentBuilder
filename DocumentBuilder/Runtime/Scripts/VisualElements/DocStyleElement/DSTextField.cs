using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;
namespace NaiveAPI.DocumentBuilder
{
    public class DSTextField : TextField
    {
        public new class UxmlFactory : UxmlFactory<DSTextField, UxmlTraits> { }
        public new class UxmlTraits : TextField.UxmlTraits { }
        public VisualElement InputFieldElement => inputFieldElement;
        VisualElement inputFieldElement;
        public DSTextField()
        {
            style.ClearMarginPadding();
            style.minHeight = DocStyle.Current.LineHeight;
            style.SetIS_Style(DocStyle.Current.MainTextStyle);
            inputFieldElement = this[0];
            inputFieldElement.style.SetIS_Style(DocStyle.Current.InputFieldStyle);
        }
        public new string label
        {
            get => base.label;
            set
            {
                base.label = value;
                labelElement.style.SetIS_Style(DocStyle.Current.MainTextStyle);
                labelElement.style.width = DocStyle.Current.LabelWidth;
                labelElement.style.minWidth = DocStyle.Current.LabelWidth;
            }
        }
        public DSTextField(string label) : this()
        {
            this.label = label;
        }
        public DSTextField(string label, EventCallback<ChangeEvent<string>> changeCallback) : this(label)
        {
            this.RegisterValueChangedCallback(changeCallback);
        }
    }
}
