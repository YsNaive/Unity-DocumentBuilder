using NaiveAPI_UI;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocSeeAlso : DocVisual<DocSeeAlso.Data>
    {
        public override string VisualID => "6";

        protected override void OnCreateGUI()
        {
            VisualElement child = new VisualElement();
            child.style.SetIS_Style(ISFlex.Horizontal);
            child.style.paddingLeft = DocStyle.Current.MainTextSize;
            Action buttonClick = null;
            if (visualData.mode == Mode.OpenPage)
            {
                ScrollView scrollView = new DSScrollView();
                scrollView.style.maxHeight = visualData.height;
                scrollView.style.overflow = Overflow.Hidden;
                scrollView.style.marginLeft = 2 * DocStyle.Current.MainTextSize;
                VisualElement mask = new VisualElement();
                mask.pickingMode = PickingMode.Ignore;
                mask.style.position = Position.Absolute;
                mask.style.marginLeft = 2 * DocStyle.Current.MainTextSize;
                Color maskColor = DocStyle.Current.HintColor;
                maskColor.a = 0.25f;
                mask.style.backgroundColor = maskColor;
                VisualElement docPage;
                if (Target.ObjsData.Count > 0)
                {
                    if (Target.ObjsData[0] != null)
                        docPage = new DocPageVisual((SODocPage)Target.ObjsData[0]);
                    else
                        docPage = new VisualElement();
                }
                else
                    docPage = new VisualElement();
                scrollView.Add(docPage);
                buttonClick = () =>
                {
                    if (this.Contains(scrollView))
                        this.Remove(scrollView);
                    else
                        this.Add(scrollView);
                    if (this.Contains(mask))
                        this.Remove(mask);
                    else
                        this.Add(mask);
                };
                this.RegisterCallback<GeometryChangedEvent>(e =>
                {
                    mask.style.top = child.resolvedStyle.height;
                    mask.style.width = scrollView.resolvedStyle.width;
                    mask.style.height = scrollView.resolvedStyle.height;
                });
            }
            else if (visualData.mode == Mode.OpenUrl)
            {
                buttonClick = () =>
                {
                    Application.OpenURL(visualData.url);
                };
            }
            Button button = new DSButton("",buttonClick);
            button.text = Target.TextData[1];
            button.style.minWidth = Length.Percent(10);
            button.style.SetIS_Style(ISMargin.Pixel(DocStyle.Current.MainTextSize / 2));
            TextElement textElement = new DSTextElement(Target.TextData[0]);
            textElement.style.SetIS_Style(ISMargin.Pixel(DocStyle.Current.MainTextSize / 2));
            child.Add(textElement);
            child.Add(button);
            this.style.borderLeftWidth = DocStyle.Current.MainTextSize / 2f;
            this.style.borderLeftColor = DocStyle.Current.SubBackgroundColor;
            this.Add(child);
        }

        public class Data
        {
            public int height = 400;
            public string url = "";
            public Mode mode = Mode.OpenPage;
        }

        public enum Mode
        {
            OpenPage, OpenUrl
        }
    }
}
