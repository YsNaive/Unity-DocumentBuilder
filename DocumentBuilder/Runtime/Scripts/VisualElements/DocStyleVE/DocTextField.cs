using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;
namespace NaiveAPI.DocumentBuilder
{
    public class DocTextField : TextField
    {
        public new class UxmlFactory : UxmlFactory<DocTextField, UxmlTraits> { }
        public new class UxmlTraits : TextField.UxmlTraits { }
        public DocTextField()
        {
            style.ClearMarginPadding();
            style.minHeight = DocStyle.Current.LineHeight;
            style.SetIS_Style(DocStyle.Current.MainTextStyle);
            this[0].style.paddingLeft = DocStyle.Current.MainTextSize / 2f;
            this[0].style.SetIS_Style(DocStyle.Current.InputFieldStyle);
        }
        public DocTextField(string label) : this()
        {
            this.label = label;
        }
        public DocTextField(string label, EventCallback<ChangeEvent<string>> changeCallback) : this(label)
        {
            this.RegisterValueChangedCallback(changeCallback);
        }
    }
}
