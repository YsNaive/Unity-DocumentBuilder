using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
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
    public Action<Action> IntroAnimation;
    public Action<Action> OuttroAnimation;
    public DocComponent Target => m_target;

    private DocComponent m_target;
    public void SetTarget(DocComponent target)
    {
        m_target = target;
        OnCreateGUI();
        OnSelectIntroAni(Target.IntroType);
        OnSelectOuttroAni(Target.OuttroTime);
    }
    /// <summary>
    /// Call after Target is set
    /// </summary>
    protected abstract void OnCreateGUI();
    protected virtual void OnSelectIntroAni(int type)
    {
        if ((VisualElementAnimation.Mode)type == VisualElementAnimation.Mode.None)
            return;
        IntroAnimation = (callback) => { this.Fade(1, Target.IntroTime, 20, callback); };
    }
    protected virtual void OnSelectOuttroAni(int type)
    {
        if ((VisualElementAnimation.Mode)type == VisualElementAnimation.Mode.None)
            return;
        OuttroAnimation = (callback) => { this.Fade(0, Target.OuttroTime, 20, callback); };
    }
}
