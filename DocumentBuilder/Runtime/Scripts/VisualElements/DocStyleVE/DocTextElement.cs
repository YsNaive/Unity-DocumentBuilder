using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;
namespace NaiveAPI.DocumentBuilder
{
    public class DocTextElement : TextElement
    {
        public new class UxmlFactory : UxmlFactory<DocTextElement, UxmlTraits>
        {
        }
        public new class UxmlTraits : TextElement.UxmlTraits
        {

        }
        public DocTextElement()
        {
            style.whiteSpace = WhiteSpace.Normal;
            style.minHeight = DocStyle.Current.LineHeight;
            style.SetIS_Style(DocStyle.Current.MainTextStyle);
        }
        public DocTextElement(string text) : this()
        {
            this.text = text;
        }
    }
}
