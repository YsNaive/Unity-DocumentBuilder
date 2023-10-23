using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;
namespace NaiveAPI.DocumentBuilder
{
    public class DSLabel : Label
    {
        public new class UxmlFactory : UxmlFactory<DSLabel, UxmlTraits> { }
        public new class UxmlTraits : Label.UxmlTraits { }
        public DSLabel()
        {
            style.whiteSpace = WhiteSpace.Normal;
            style.minHeight = DocStyle.Current.LineHeight;
            style.SetIS_Style(DocStyle.Current.LabelText);
        }
        public DSLabel(string text) : this()
        {
            this.text = text;
        }
    }
}
