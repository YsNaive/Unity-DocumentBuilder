using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtension
{
    public static bool IsFocusedOnPanel(this VisualElement element)
    {
        if (element.panel == null) return false;
        return element.panel.focusController.focusedElement == element;
    } 
}
