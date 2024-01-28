using NaiveAPI_UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocCodeblock : DocVisual<DocCodeblock.Data>
    {
        public class Data
        {
            public int MaxHeight = 300;
            public int LineHeightPercent = 125;
        }
        public override string VisualID => "7";

        TextElement codeContents;
        ScrollView codeScrollView;
        protected override void OnCreateGUI()
        {
            codeScrollView =new DSScrollView();
            codeScrollView.nestedInteractionKind = ScrollView.NestedInteractionKind.ForwardScrolling;
            if (Target.TextData.Count == 0) Target.TextData.Add("");
            ISPadding padding = ISPadding.Pixel(DocStyle.Current.MainTextSize / 2);
            codeContents = new DSTextElement($"<line-height={visualData.LineHeightPercent}%>"+DocumentBuilderParser.CSharpParser(Target.TextData[0]));
            codeContents.style.whiteSpace = WhiteSpace.NoWrap;
            codeContents.style.color = new Color(.85f, .85f, .85f);
            codeContents.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
            codeContents.style.SetIS_Style(padding);
            codeContents.style.fontSize = DocStyle.Current.MainTextSize;
            //codeContents.style.width = data.MinWidth;
            string lineNum = $"<line-height={visualData.LineHeightPercent}%>1";
            int i = 2;
            foreach(var c in Target.TextData[0])
            {
                if(c == '\n')
                {
                    lineNum += $"\n{i++}";
                }
            }
            ScrollView numScrollView = new DSScrollView();
            TextElement lineNumber = new DSTextElement(lineNum);
            lineNumber.style.SetIS_Style(padding);
            lineNumber.style.fontSize = DocStyle.Current.MainTextSize;
            lineNumber.style.color = DocStyle.Current.SubFrontgroundColor;
            lineNumber.style.color = new Color(.6f, .6f, .6f);
            numScrollView.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
            numScrollView.style.borderRightWidth = 6;
            numScrollView.style.borderRightColor = DocStyle.Current.SubBackgroundColor;
            numScrollView.style.position = Position.Absolute;
            lineNumber.style.unityTextAlign = TextAnchor.MiddleRight;
            Button copy = null;
            copy = new DSButton("Copy", () =>
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
            codeScrollView.style.maxHeight = visualData.MaxHeight;
            numScrollView.style.maxHeight = visualData.MaxHeight ;
            numScrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            numScrollView.verticalScroller.valueChanged += v => { codeScrollView.verticalScroller.value = v; };
            codeScrollView.verticalScroller.valueChanged += v => { numScrollView.verticalScroller.value = v; };
            codeScrollView.Add(codeContents);
            codeScrollView.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
            numScrollView.Add(lineNumber);
            Add(codeScrollView);
            Add(numScrollView);
            Add(copy);

            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (Mathf.Abs(e.oldRect.width - e.newRect.width) < 7) return;
                codeScrollView.style.width = e.newRect.width;
                codeContents.style.paddingLeft = lineNumber.layout.width*1.5f;
                codeScrollView.style.marginLeft = 6;
                copy.style.right = codeScrollView.verticalScroller.enabledInHierarchy ? (DocStyle.Current.ScrollerWidth) : 0;
            });
        }
    }
}
