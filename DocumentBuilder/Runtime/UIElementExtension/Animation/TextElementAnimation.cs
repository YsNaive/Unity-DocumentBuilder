using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public static class TextElementAnimation
    {
        public static void TextFadeIn(this TextElement ve, int msPerLen = 50, int lenPerStep = 1, Action endCallback = null)
        {
            int len = ve.text.Length;
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
                    ve.schedule.Execute(exe).ExecuteLater(msPerLen);
                else
                    endCallback?.Invoke();
            };
            exe();
        }
    }
}
