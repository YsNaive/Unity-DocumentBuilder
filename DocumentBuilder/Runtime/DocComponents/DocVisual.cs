using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DocVisual : VisualElement
{
    public DocVisual()
    {
        RegisterCallback<GeometryChangedEvent>(e =>
        {
            if (e.oldRect.width != e.newRect.width)
                OnWidthChanged?.Invoke(e.newRect.width);
            if (e.oldRect.height != e.newRect.height)
                OnHeightChanged?.Invoke(e.newRect.height);
        });
    }
    public abstract string VisualID { get; }
    public Action<float> OnHeightChanged;
    public Action<float> OnWidthChanged;
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
