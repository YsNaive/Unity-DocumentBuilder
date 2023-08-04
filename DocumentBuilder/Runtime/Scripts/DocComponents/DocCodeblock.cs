using NaiveAPI_UI;
using System;
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
            ISPadding padding = ISPadding.Pixel(SODocStyle.Current.MainTextSize / 2);
            codeContents = DocRuntime.NewTextElement($"<line-height={data.LineHeightPercent}%>"+DocumentBuilderParser.CSharpParser(Target.TextData[0]));
            codeContents.style.whiteSpace = WhiteSpace.NoWrap;
            codeContents.style.color = new Color(.85f, .85f, .85f);
            codeContents.style.backgroundColor = SODocStyle.Current.CodeBackgroundColor;
            codeContents.style.SetIS_Style(padding);
            codeContents.style.fontSize = SODocStyle.Current.MainTextSize;
            //codeContents.style.width = data.MinWidth;
            string lineNum = $"<line-height={data.LineHeightPercent}%>1";
            int i = 2;
            foreach(var c in Target.TextData[0])
            {
                if(c == '\n')
                {
                    lineNum += $"\n{i++}";
                }
            }
            ScrollView numScrollView = DocRuntime.NewScrollView();
            TextElement lineNumber = DocRuntime.NewTextElement(lineNum);
            lineNumber.style.SetIS_Style(padding);
            lineNumber.style.fontSize = SODocStyle.Current.MainTextSize;
            lineNumber.style.color = SODocStyle.Current.SubFrontgroundColor;
            lineNumber.style.color = new Color(.6f, .6f, .6f);
            numScrollView.style.backgroundColor = SODocStyle.Current.CodeBackgroundColor;
            numScrollView.style.borderRightWidth = 6;
            numScrollView.style.borderRightColor = SODocStyle.Current.SubBackgroundColor;
            numScrollView.style.position = Position.Absolute;
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
            codeContents.style.position = Position.Absolute;
            EventCallback<GeometryChangedEvent> exe = null;
            exe = (e) =>
            {
                codeContents.style.minWidth = e.newRect.width  + lineNumber.layout.width * 1.5f;
                codeContents.style.position = Position.Relative;
                codeContents.UnregisterCallback(exe);
            };
            codeContents.RegisterCallback(exe);
            copy.style.position = Position.Absolute;
            copy.style.top = 5;
            codeScrollView.style.maxHeight = data.MaxHeight;
            numScrollView.style.maxHeight = data.MaxHeight ;
            numScrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            numScrollView.verticalScroller.valueChanged += v => { codeScrollView.verticalScroller.value = v; };
            codeScrollView.verticalScroller.valueChanged += v => { numScrollView.verticalScroller.value = v; };
            codeScrollView.Add(codeContents);
            codeScrollView.style.backgroundColor = SODocStyle.Current.CodeBackgroundColor;
            numScrollView.Add(lineNumber);
            Add(codeScrollView);
            Add(numScrollView);
            Add(copy);

            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (e.oldRect.width == e.newRect.width) return;
                codeScrollView.style.width = e.newRect.width;
                codeContents.style.paddingLeft = lineNumber.layout.width*1.5f;
                codeScrollView.style.marginLeft = 6;
                copy.style.right = codeScrollView.verticalScroller.enabledInHierarchy ? (SODocStyle.Current.ScrollerWidth) : 0;
            });
        }

        public class Data
        {
            public int MaxHeight = 300;
            public int LineHeightPercent = 125;
        }
    }
}