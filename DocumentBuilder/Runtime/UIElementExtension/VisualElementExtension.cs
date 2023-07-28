using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtension
{
    public static int UnityRTF(this StringBuilder str, int begIndex, int length, Color color)
    {
        string colorFormat = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
        str.Insert(begIndex + length,"</color>");
        str.Insert(begIndex,"</color>");
        return str.Length;
    }
    public static bool IsFocusedOnPanel(this VisualElement element)
    {
        if (element.panel == null) return false;
        return element.panel.focusController.focusedElement == element;
    } 
}
