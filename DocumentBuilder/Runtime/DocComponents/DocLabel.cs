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

        protected override void OnCreateGUI()
        {
            TextElement text = new TextElement();
            if (Target.TextData.Count > 0)
                text.text = Target.TextData[0];
            text.style.SetIS_Style(DocStyle.Current.LabelText);
            Add(text);
            IntroAnimation = (callBack) => { this.Fade(0,1, 500, 50, callBack); };
            OuttroAnimation = (callBack) => { this.Fade(1,0, 500, 50, callBack); };
        }
    }

}