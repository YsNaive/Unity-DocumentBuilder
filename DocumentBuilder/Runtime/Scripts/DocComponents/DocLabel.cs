using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocLabel : DocVisual
    {
        public override string VisualID => "1";

        Data data;
        protected override void OnCreateGUI()
        {
            data = JsonUtility.FromJson<Data>(Target.JsonData);
            data ??= new Data();
            TextElement text = new TextElement();
            if (Target.TextData.Count > 0)
                text.text = Target.TextData[0];
            text.style.SetIS_Style(SODocStyle.Current.LabelText);
            text.style.fontSize = Mathf.Clamp(text.style.fontSize.value.value - (2 * (data.Level - 1)), SODocStyle.Current.MainTextSize, float.MaxValue);
            Add(text);
            IntroAnimation = (callBack) => { this.Fade(0,1, 500, 50, callBack); };
            OuttroAnimation = (callBack) => { this.Fade(1,0, 500, 50, callBack); };
        }

        public class Data
        {
            public int Level = 1;
        }
    }

}