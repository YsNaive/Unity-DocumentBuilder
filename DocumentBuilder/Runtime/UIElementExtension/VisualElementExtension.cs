using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtension
{
    public static void UnityRTF(this string str, int begIndex, int length, Color color)
    {
        string colorFormat = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
        str.Insert(begIndex + length,"</color>");
        str.Insert(begIndex,"</color>");
    }
    public static bool IsFocusedOnPanel(this VisualElement element)
    {
        if (element.panel == null) return false;
        return element.panel.focusController.focusedElement == element;
    } 
}
