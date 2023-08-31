using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public static class VisualElementExtension
    {
        public static int HtmlRTF(this StringBuilder str, int begIndex, int length, Color color)
        {
            string colorFormat = $"<font color=#{ColorUtility.ToHtmlStringRGB(color)}>";
            str.Insert(begIndex + length, "</font>");
            str.Insert(begIndex, colorFormat);
            return colorFormat.Length + 7;
        }
        public static int UnityRTF(this StringBuilder str, int begIndex, int length, Color color)
        {
            string colorFormat = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
            str.Insert(begIndex + length, "</color>");
            str.Insert(begIndex, colorFormat);
            return colorFormat.Length + 8;
        }
        public static int UnityRTF(this StringBuilder str, int begIndex, int length, FontStyle font)
        {
            string format = "";
            string endFormat = "";
            if (font == FontStyle.Bold)
            {
                format = "<b>";
                endFormat = "</b>";
            }
            if (font == FontStyle.Italic)
            {
                format = "<i>";
                endFormat = "</i>";
            }
            if (font == FontStyle.BoldAndItalic)
            {
                format = "<b><i>";
                endFormat = "</i></b>";
            }
            str.Insert(begIndex + length, endFormat);
            str.Insert(begIndex, format);
            return format.Length * 2 + 1;
        }
        public static bool IsFocusedOnPanel(this VisualElement element)
        {
            if (element.panel == null) return false;
            return element.panel.focusController.focusedElement == element;
        }
    }
}