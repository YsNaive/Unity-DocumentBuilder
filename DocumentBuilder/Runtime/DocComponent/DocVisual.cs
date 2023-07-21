using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DocVisual
{
    public abstract string DisplayName { get; }
    public abstract VisualElement CreateEditGUI(DocComponent docComponent, int width); 
    public abstract VisualElement CreateViewGUI(DocComponent docComponent, int width);
    public abstract DocComponent SaveTo(VisualElement visualElement, DocComponent docComponent);
    public virtual string ToMarkdown(VisualElement visualElement) { return ""; }
}

