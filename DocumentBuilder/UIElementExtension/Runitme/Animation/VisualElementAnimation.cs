using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public abstract class VisualElementAnimationPlayer
    {
        public virtual long UpdateDelay => 20;
        public bool IsPlaying => m_IsPlaying;
        public bool IsPause => m_IsPause;

        public VisualElementAnimationPlayer(VisualElement target, float DurationSEC, Action callback)
        {
            this.target = target;
            this.DurationSEC = DurationSEC;
            Callback += callback;
        }

        protected VisualElement target;
        IVisualElementScheduledItem scheduler;
        public float DurationSEC;
        public event Action Callback;
        protected bool m_IsPlaying = false;
        protected bool m_IsPause = false;
        protected float m_BeginTime;
        protected float m_BeginPauseTime;
        protected float m_PausedTime = 0;
        public void Start()
        {
            if (IsPlaying) return;
            if (target == null) return;
            if (target.panel == null) return;
            SetBegin();
            m_BeginTime = Time.realtimeSinceStartup;
            m_PausedTime = 0;
            m_IsPlaying = true;
            scheduler = target.panel.visualTree.schedule.Execute(() =>
            {
                if(target.panel == null)
                {
                    Stop();
                    return;
                }
                if (IsPause) return;
                var percent = (Time.realtimeSinceStartup - m_BeginTime - m_PausedTime) / DurationSEC;
                if (percent > 1)
                {
                    Stop();
                    return;
                }
                Update(percent);
            });
            scheduler.Every(UpdateDelay);
            scheduler.Until(() => {
                if (IsPlaying)
                    return false;
                scheduler = null;
                return true;
            });
        }
        public void Stop()
        {
            if (!IsPlaying) return;
            SetEnd();
            m_IsPlaying = false;
            Callback?.Invoke();
        }
        public void Pause()
        {
            if (!IsPlaying) return;
            if (IsPause) return;
            m_IsPause = true;
            m_BeginPauseTime = Time.realtimeSinceStartup;
        }
        public void Continue()
        {
            if (!IsPause) return;
            m_IsPause = false;
            m_PausedTime += Time.realtimeSinceStartup - m_BeginPauseTime;
        }
        protected abstract void Update(float timePercent);
        public abstract void SetBegin();
        public abstract void SetEnd();
    }
    public static class VisualElementAnimation
    {
        public class GotoTransformPosition : VisualElementAnimationPlayer
        {
            protected Vector2 originPosition;
            protected Vector2 direction;
            protected AnimationCurve curve;
            public GotoTransformPosition(VisualElement target, float DurationSEC, Vector2 position, Action callback = null) : this(target, DurationSEC, position, AnimationCurve.EaseInOut(0, 0, 1, 1), callback) { }
            public GotoTransformPosition(VisualElement target, float DurationSEC,Vector2 position, AnimationCurve curve, Action callback = null) : base(target, DurationSEC, callback)
            {
                originPosition = target.transform.position;
                direction = position - originPosition;
                this.curve = curve;
            }

            public override void SetBegin()
            {
                target.transform.position = originPosition;
            }

            public override void SetEnd()
            {
                target.transform.position = originPosition + direction;
            }

            protected override void Update(float timePercent)
            {
                var curvePercent = curve.Evaluate(timePercent * (curve.keys[^1].time - curve[0].time ));
                target.transform.position = originPosition + curvePercent * direction;
            }

            public void ApplyToIStyleLeftTop()
            {
                target.transform.position = originPosition;
                target.style.left = direction.x + target.style.left.value.value;
                target.style.top = direction.y + target.style.top.value.value;
            }
        }

        public class GotoTransformPositionBackAndForth : GotoTransformPosition
        {
            bool m_IsStop = true;
            public GotoTransformPositionBackAndForth(VisualElement target, float DurationSEC, Vector2 position, Action callback = null) : this(target, DurationSEC, position, AnimationCurve.EaseInOut(0, 0, 1, 1), callback) { }
            public GotoTransformPositionBackAndForth(VisualElement target, float DurationSEC, Vector2 position, AnimationCurve curve, Action callback = null) : base(target, DurationSEC, position, curve, callback)
            {
                Callback += () =>
                {
                    if (m_IsStop) return;
                    ApplyToIStyleLeftTop();
                    direction = -direction;
                    (this as VisualElementAnimationPlayer).Start();
                };
            }
            public new void Start()
            {
                base.Start();
                m_IsStop = false;
            }
            public new void Stop()
            {
                base.Stop();
                m_IsStop = true;
            }
        }


        public static void GoToPosition(this VisualElement ve, Vector2 targetPos, Action callback = null)
        {
            GoToPosition(ve, targetPos, 10, 20, AnimationCurve.EaseInOut(0, 0, 20, 1), callback); }
        public static void GoToPosition(this VisualElement ve, Vector2 targetPos,int msPerStep,int steps, AnimationCurve curve, Action callback = null)
        {
            float curTime = curve[0].time;
            float totalTime = curve[curve.length - 1].time - curTime;
            float timePerStep = totalTime / steps;
            int curStep = 0;
            Vector2 originPos = ve.transform.position;
            ve.panel.visualTree.schedule.Execute(() =>
            {
                if (ve.panel == null) return;
                curStep++;
                curTime += timePerStep;
                float rate = curve.Evaluate(curTime);
                Vector2 newPos = originPos + ((targetPos - originPos) * rate);
                ve.transform.position = newPos;
            }).Every(msPerStep).Until(() =>
            {
                if(curStep > steps)
                {
                    callback?.Invoke();
                    ve.transform.position = targetPos;
                    return true;
                }return false;
            });
        }
        public static void Fade(this VisualElement element, float to, float ms, float step = 20, Action callback = null) {
            Fade(element, element.style.opacity.value, to, ms, step, callback);
        }
        public static void Fade(this VisualElement element, float from, float to, float ms, float step = 20, Action callback = null) {
            int curStep = 0;
            float sumVal = from;
            float stepVal = (to - sumVal)/step;
            int stepMs = (int)(ms/step);
            Action exe = null;
            exe = () =>
            {
                sumVal += stepVal;
                element.style.opacity = sumVal;
                curStep++;
                if (curStep < step)
                    element.schedule.Execute(exe).ExecuteLater(stepMs);
                else
                {
                    element.style.opacity = to;
                    callback?.Invoke();
                }
            };
            exe();
        }

        public static void Highlight(this VisualElement element, int msPerStep, int step = 20,  Action callback = null) { Highlight(element, msPerStep, new Color(.6f, .8f, .6f), step, callback); }
        public static void Highlight(this VisualElement element, int msPerStep, Color color,int step = 20,  Action callback = null)
        {
            VisualElement highlight = new VisualElement();
            color.a = 0.35f;
            highlight.style.backgroundColor = color;
            highlight.style.position = Position.Absolute;
            highlight.style.width = element.resolvedStyle.width;
            highlight.style.height = element.resolvedStyle.height;
            highlight.style.opacity = 0f;
            element.Add(highlight);
            int stepNow = 0;
            float stepMid = step / 2;
            highlight.schedule.Execute(() =>
            {
                if (stepNow < stepMid) highlight.style.opacity = stepNow / stepMid;
                else highlight.style.opacity = (20 - stepNow) / stepMid;
                stepNow++;
            }).Every(msPerStep).Until(() =>
            {
                if(stepNow >= step)
                {
                    callback?.Invoke();
                    element.Remove(highlight);
                    return true;
                }
                else return false;
            });
        }
    }

}