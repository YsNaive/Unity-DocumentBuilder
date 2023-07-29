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
            TextElement text = DocRuntime.NewTextElement(Target.TextData[0]);
            text.style.SetIS_Style(DocStyle.Current.MainText);
            Add(text);
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            data ??= new Data();
            if (text.text == "")
            {
                text.text = "Warning ! This is a Empty Description";
                data.Type = Type.Warning;
            }
            if (data.Type != Type.None)
            {
                text.style.backgroundColor = getTypeColor(data.Type);
                text.style.color = getTypeTextColor(data.Type);
                text.style.SetIS_Style(ISPadding.Percent(5));
            }
            switch (data.IntroMode)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    IntroAnimation = (callBack) => { this.Fade(0,1, data.IntroAniTime,50,callBack); };
                    break;
                case AniMode.TextFade:
                    IntroAnimation = (callBack) => { text.TextFadeIn(Target.TextData[0],data.IntroAniTime,1, callBack); };
                    break;
            }
            switch (data.OutroMode)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    OuttroAnimation = (callBack) => { this.Fade(1,0, data.OutroAniTime, 50, callBack); };
                    break;
                case AniMode.TextFade:
                    OuttroAnimation = (callBack) => { text.TextFadeOut(data.OutroAniTime,1, callBack); };
                    break;
            }
        }

        public class Data
        {
            public Type Type = Type.None;
            public AniMode IntroMode = AniMode.Fade;
            public AniMode OutroMode = AniMode.Fade;
            public int IntroAniTime = 250;
            public int OutroAniTime = 250;

        }
        public enum AniMode
        {
            None,
            Fade,
            TextFade,
        }
        public enum Type
        {
            None,
            Success,
            Warning,
            Danger,
            Hint,
        }
        Color getTypeColor(Type type)
        {
            if (type == Type.Hint) return DocStyle.Current.HintColor;
            if (type == Type.Success) return DocStyle.Current.SuccessColor;
            if (type == Type.Warning) return DocStyle.Current.WarningColor;
            return DocStyle.Current.DangerColor;
        }
        Color getTypeTextColor(Type type)
        {
            if (type == Type.Hint) return DocStyle.Current.HintTextColor;
            if (type == Type.Success) return DocStyle.Current.SuccessTextColor;
            if (type == Type.Warning) return DocStyle.Current.WarningTextColor;
            return DocStyle.Current.DangerTextColor;
        }
    }

}