using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public static class TextElementAnimation
    {
        public static void TextFadeIn(this TextElement ve, int ms, int lenPerStep = 1)
        {
            int len = ve.text.Length;
            int msPerStep = (int)(ms / ((float)len / lenPerStep));
            string content = ve.text;
            ve.text = string.Empty;
            Action exe = null;
            int curIndex = 0;
            exe = () =>
            {
                for(int i =0;i< lenPerStep && curIndex < len; i++, curIndex++)
                {
                    ve.text += content[curIndex];
                }
                if (curIndex < len)
                    ve.schedule.Execute(exe).ExecuteLater(msPerStep);
            };
            exe();
        }
    }
}
