using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocDescription : DocVisual
    {
        public override string VisualID => "2";

        protected override void OnCreateGUI()
        {
            TextElement text = new TextElement();
            text.text = Target.TextData[0];
            text.style.SetIS_Style(DocStyle.Current.MainText);
            Add(text);
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            data ??= new Data();
            switch (data.AnimateMode)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    IntroAnimation = (callBack) => { this.Fade(1, data.IntroAniTime,50,callBack); };
                    break;
                case AniMode.TextFade:
                    IntroAnimation = (callBack) => { text.TextFadeIn(data.IntroAniTime,1, callBack); };
                    break;
            }
        }

        public class Data
        {
            public AniMode AnimateMode = AniMode.Fade;
            public int IntroAniTime = 500;
        }
        public enum AniMode
        {
            None,
            Fade,
            TextFade,
        }
    }

}