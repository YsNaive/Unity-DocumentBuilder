using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocCodeblock : DocVisual
    {
        public override string VisualID => "7";

        TextElement codeContents;
        ScrollView codeScrollView;
        protected override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            data ??=  new Data();
            IntroAnimation = (callBack) => { this.Fade(0, 1, 200, 50, callBack); };
            OuttroAnimation = (callBack) => { this.Fade(1, 0, 200, 50, callBack); };
            codeScrollView =DocRuntime.NewScrollView();
            if (Target.TextData.Count == 0) Target.TextData.Add("");
            ISPadding padding = ISPadding.Pixel(DocStyle.Current.MainTextSize / 2);
            codeContents = DocRuntime.NewTextElement($"<line-height={data.LineHeightPercent}%>"+DocumentBuilderParser.CSharpParser(Target.TextData[0]));
            codeContents.style.whiteSpace = WhiteSpace.NoWrap;
            codeContents.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
            codeContents.style.SetIS_Style(padding);
            codeContents.style.fontSize = DocStyle.Current.MainTextSize;
            codeContents.style.width = data.MinWidth;
            string lineNum = $"<line-height={data.LineHeightPercent}%>1";
            int i = 2;
            foreach(var c in Target.TextData[0])
            {
                if(c == '\n')
                {
                    lineNum += $"\n{i++}";
                }
            }
            TextElement lineNumber = DocRuntime.NewTextElement(lineNum);
            lineNumber.style.SetIS_Style(padding);
            lineNumber.style.fontSize = DocStyle.Current.MainTextSize;
            lineNumber.style.color = DocStyle.Current.SubFrontGroundColor;
            lineNumber.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
            lineNumber.style.borderRightWidth = 6;
            lineNumber.style.borderRightColor = DocStyle.Current.SubBackgroundColor;
            lineNumber.style.position = Position.Absolute;
            lineNumber.style.unityTextAlign = TextAnchor.MiddleRight;
            Button copy = null;
            copy = DocRuntime.NewButton("Copy", () =>
            {
                GUIUtility.systemCopyBuffer = Target.TextData[0];
                copy.text = "Copied !";
                copy.schedule.Execute(() =>
                {
                    copy.text = "Copy";
                }).ExecuteLater(1000);
            });
            copy.style.position = Position.Absolute;
            copy.style.top = 5;
            codeScrollView.style.maxHeight = data.MaxHeight;

            codeScrollView.Add(codeContents);
            codeScrollView.Add(lineNumber);
            Add(codeScrollView);
            Add(copy);

            RegisterCallback<GeometryChangedEvent>(e =>
            {
                codeContents.style.marginLeft = lineNumber.layout.width;
                copy.style.right = codeScrollView.verticalScroller.enabledInHierarchy ? 20 : 5;
            });
        }

        public class Data
        {
            public int MinWidth = -1;
            public int MaxHeight = 300;
            public int LineHeightPercent = 150;
        }
    }
}
