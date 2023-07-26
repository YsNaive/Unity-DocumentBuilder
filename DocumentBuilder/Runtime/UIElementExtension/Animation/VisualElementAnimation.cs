using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public static class VisualElementAnimation
    {
        public static void Fade(this VisualElement element, float opacity, float ms, float step = 50, Action encCallback = null) {
            int curStep = 0;
            float sumVal = element.style.opacity.value;
            float stepVal = (opacity - sumVal)/step;
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
                    element.style.opacity = opacity;
                    encCallback?.Invoke();
                }
            };
            exe();
        }
    }

}