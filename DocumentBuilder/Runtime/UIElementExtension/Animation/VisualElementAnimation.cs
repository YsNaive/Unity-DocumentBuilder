using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public static class VisualElementAnimation
    {
        public enum Mode
        {
            None = 0,
            Fade = 1
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