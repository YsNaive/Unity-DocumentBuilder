using NaiveAPI_UI;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocDescription : DocVisual<ValueTuple<DocDescription.DescriptionType>>
    {
        public override string VisualID => "2";
        TextElement text;
        protected override void OnCreateGUI()
        {
            if(Target.TextData.Count == 0)Target.TextData.Add("");
            text = DocRuntime.NewTextElement(Target.TextData[0]);
            text.style.SetIS_Style(DocStyle.Current.MainText);
            Add(text);
            if (text.text == "")
            {
                text.text = "Warning ! This is a Empty Description";
                visualData.Item1 = DescriptionType.Warning;
            }
            if (visualData.Item1 != DescriptionType.None)
            {
                text.style.backgroundColor = getTypeColor(visualData.Item1);
                text.style.color = getTypeTextColor(visualData.Item1);
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
        public new enum AniMode
        {
            None,
            Fade,
            TextFade,
        }
        public enum DescriptionType
        {
            None,
            Success,
            Warning,
            Danger,
            Hint,
        }
        Color getTypeColor(DescriptionType type)
        {
            if (type == DescriptionType.Hint) return DocStyle.Current.HintColor;
            if (type == DescriptionType.Success) return DocStyle.Current.SuccessColor;
            if (type == DescriptionType.Warning) return DocStyle.Current.WarningColor;
            return DocStyle.Current.DangerColor;
        }
        Color getTypeTextColor(DescriptionType type)
        {
            if (type == DescriptionType.Hint) return DocStyle.Current.HintTextColor;
            if (type == DescriptionType.Success) return DocStyle.Current.SuccessTextColor;
            if (type == DescriptionType.Warning) return DocStyle.Current.WarningTextColor;
            return DocStyle.Current.DangerTextColor;
        }
    }

}