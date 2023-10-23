using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;
namespace NaiveAPI.DocumentBuilder
{
    public class DSTextElement : TextElement
    {
        public new class UxmlFactory : UxmlFactory<DSTextElement, UxmlTraits> { }
        public new class UxmlTraits : TextElement.UxmlTraits { }
        public DSTextElement()
        {
            style.whiteSpace = WhiteSpace.Normal;
            style.minHeight = DocStyle.Current.LineHeight;
            style.SetIS_Style(DocStyle.Current.MainTextStyle);
        }
        public DSTextElement(string text) : this()
        {
            this.text = text;
        }
    }
}
