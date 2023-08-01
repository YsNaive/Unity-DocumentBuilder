using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DocVisual : VisualElement
{
    public enum AniMode
    {
        None = 0,
        Fade = 1,
    }
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
        OnSelectOuttroAni(Target.OuttroType);
    }
    /// <summary>
    /// Call after Target is set
    /// </summary>
    protected abstract void OnCreateGUI();
    protected virtual void OnSelectIntroAni(int type)
    {
        if ((AniMode)type == AniMode.None)
            return;
        else if((AniMode)type == AniMode.Fade)
            IntroAnimation = (callback) => { this.Fade(0,1, Target.IntroTime, 20, callback); };
    }
    protected virtual void OnSelectOuttroAni(int type)
    {
        if ((AniMode)type == AniMode.None)
            return;
        else if ((AniMode)type == AniMode.Fade)
            OuttroAnimation = (callback) => { this.Fade(1,0, Target.OuttroTime, 20, callback); };
    }
}
