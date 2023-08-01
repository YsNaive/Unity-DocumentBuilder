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
        TextElement text;
        protected override void OnCreateGUI()
        {
            if(Target.TextData.Count == 0)Target.TextData.Add("");
            text = DocRuntime.NewTextElement(Target.TextData[0]);
            text.style.SetIS_Style(SODocStyle.Current.MainText);
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
        }
        protected override void OnSelectIntroAni(int type)
        {
            switch ((AniMode)type)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    IntroAnimation = (callBack) => { this.Fade(0, 1, Target.IntroTime, 50, callBack); };
                    break;
                case AniMode.TextFade:
                    IntroAnimation = (callBack) => { text.TextFadeIn(Target.TextData[0], Target.IntroTime, 1, callBack); };
                    break;
            }
        }
        protected override void OnSelectOuttroAni(int type)
        {
            switch ((AniMode)type)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    OuttroAnimation = (callBack) => { this.Fade(1, 0, Target.OuttroTime, 20, callBack); };
                    break;
                case AniMode.TextFade:
                    OuttroAnimation = (callBack) => { text.TextFadeOut(Target.OuttroTime, 1, callBack); };
                    break;
            }
        }
        public class Data
        {
            public Type Type = Type.None;
        }
        public new enum AniMode
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
            if (type == Type.Hint) return SODocStyle.Current.HintColor;
            if (type == Type.Success) return SODocStyle.Current.SuccessColor;
            if (type == Type.Warning) return SODocStyle.Current.WarningColor;
            return SODocStyle.Current.DangerColor;
        }
        Color getTypeTextColor(Type type)
        {
            if (type == Type.Hint) return SODocStyle.Current.HintTextColor;
            if (type == Type.Success) return SODocStyle.Current.SuccessTextColor;
            if (type == Type.Warning) return SODocStyle.Current.WarningTextColor;
            return SODocStyle.Current.DangerTextColor;
        }
    }

}