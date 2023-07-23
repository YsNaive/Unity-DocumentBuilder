using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DocVisual
{
    public abstract string DisplayName { get; }
    public DocComponent Target => target;
    private DocComponent target;
    public void SetTarget(DocComponent target)
    {
        this.target = target;
    }
    public abstract VisualElement CreateEditGUI(int width); 
    public abstract VisualElement CreateViewGUI(int width);
    public abstract DocComponent Save(VisualElement visualElement);
    public virtual string ToMarkdown(VisualElement visualElement) { return ""; }
}

