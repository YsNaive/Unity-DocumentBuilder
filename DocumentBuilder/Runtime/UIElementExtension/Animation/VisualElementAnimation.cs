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
        public static void Fade(this VisualElement element, float to, float ms, float step = 50, Action callback = null) {
            Fade(element, element.style.opacity.value, to, ms, step, callback);
        }
        public static void Fade(this VisualElement element, float from, float to, float ms, float step = 50, Action callback = null) {
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
    }

}