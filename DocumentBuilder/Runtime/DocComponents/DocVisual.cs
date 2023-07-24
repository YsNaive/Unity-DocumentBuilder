using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DocVisual : VisualElement
{
    public virtual string VisualID { get; }
    public DocComponent Target;
    public float Width = -1.0f;
    /// <summary>
    /// Call after Target is set
    /// </summary>
    public abstract void Repaint();
}
