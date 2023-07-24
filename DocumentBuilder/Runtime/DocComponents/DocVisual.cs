using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DocVisual : VisualElement
{
    public abstract string VisualID { get; }
    public float Width = -1.0f;
    public DocComponent Target => m_target;

    private DocComponent m_target;
    public void SetTarget(DocComponent target)
    {
        m_target = target;
        OnCreateGUI();
    }
    /// <summary>
    /// Call after Target is set
    /// </summary>
    public abstract void OnCreateGUI();
}
