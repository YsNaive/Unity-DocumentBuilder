using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocSeeAlso : DocVisual
    {
        public override string VisualID => "6";

        protected override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            if (data == null) return;
            switch (data.IntroAniMode)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    IntroAnimation = (callBack) => { this.Fade(0, 1, data.IntroDuration, 50, callBack); };
                    break;
                case AniMode.TextFade:
                    break;
            }
            switch (data.OuttroAniMode)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    OuttroAnimation = (callBack) => { this.Fade(1, 0, data.OuttroDuration, 50, callBack); };
                    break;
                case AniMode.TextFade:
                    break;
            }
            ScrollView scrollView = DocRuntime.NewScrollView();
            scrollView.style.maxHeight = data.height;
            scrollView.style.overflow = Overflow.Hidden;
            scrollView.style.marginLeft = 2 * DocStyle.Current.MainTextSize;
            VisualElement mask = DocRuntime.NewEmpty();
            mask.pickingMode = PickingMode.Ignore;
            mask.style.position = Position.Absolute;
            mask.style.marginLeft = 2 * DocStyle.Current.MainTextSize;
            Color maskColor = DocStyle.Current.HintColor;
            maskColor.a = 0.25f;
            mask.style.backgroundColor = maskColor;
            VisualElement child = new VisualElement();
            child.style.SetIS_Style(ISFlex.Horizontal);
            child.style.paddingLeft = DocStyle.Current.MainTextSize;
            VisualElement ve = new VisualElement();
            VisualElement docPage = ve;
            Button button = DocRuntime.NewButton(() =>
            {
                if (data.mode == Mode.OpenPage)
                {
                    if (docPage == ve && Target.ObjsData[0] != null)
                        docPage = new DocPageVisual((SODocPage)Target.ObjsData[0]);
                    if (scrollView.Contains(docPage))
                        scrollView.Remove(docPage);
                    else
                        scrollView.Add(docPage);
                }
                else if (data.mode == Mode.OpenUrl)
                    Application.OpenURL(data.url);
            });
            button.text = Target.TextData[1];
            button.style.minWidth = Length.Percent(10);
            button.style.SetIS_Style(ISMargin.Pixel(DocStyle.Current.MainTextSize / 2));
            TextElement textElement = DocRuntime.NewTextElement(Target.TextData[0]);
            textElement.style.SetIS_Style(ISMargin.Pixel(DocStyle.Current.MainTextSize / 2));
            child.Add(textElement);
            child.Add(button);
            this.style.borderLeftWidth = DocStyle.Current.MainTextSize / 2f;
            this.style.borderLeftColor = DocStyle.Current.SubFrontGroundColor;
            this.Add(child);
            this.Add(scrollView);
            this.Add(mask);
            this.RegisterCallback<GeometryChangedEvent>(e =>
            {
                mask.style.top = child.resolvedStyle.height;
                mask.style.width = scrollView.resolvedStyle.width;
                mask.style.height = scrollView.resolvedStyle.height;
            });
        }

        public class Data
        {
            public int height = 400;
            public string url = "";
            public Mode mode = Mode.OpenPage;
            public AniMode IntroAniMode = AniMode.Fade, OuttroAniMode = AniMode.Fade;
            public int IntroDuration = 250, OuttroDuration = 250;
        }

        public enum AniMode
        {
            None,
            Fade,
            TextFade,
        }

        public enum Mode
        {
            OpenPage, OpenUrl
        }
    }
}
