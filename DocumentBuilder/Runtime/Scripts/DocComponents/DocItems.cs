using NaiveAPI_UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public sealed class DocItems : DocVisual<DocItems.Data>
    {
        public enum Mode
        {
            Disorder,
            Order,
        }
        public class Data
        {
            public Mode Mode = Mode.Disorder;
        }
        public override string VisualID => "8";

        List<VisualElement> icons = new();
        static ISRadius radius = ISRadius.Percent(100);
        protected override void OnCreateGUI()
        {
            int i = 0;
            var iconWidth = DocStyle.Current.LineHeight;
            var iconScale = new Scale(new Vector3(.35f, .35f, .35f));
            foreach(var text in Target.TextData)
            {
                i++;
                var textElement = new DSTextElement(text);
                textElement.style.unityTextAlign = TextAnchor.MiddleLeft;
                VisualElement icon = null;
                if(visualData.Mode == Mode.Order)
                {
                    icon = new DSTextElement($"{i}.");
                    icon.style.unityFontStyleAndWeight = FontStyle.Bold;
                    icon.style.unityTextAlign = TextAnchor.MiddleLeft;
                    icons.Add(icon);
                    textElement.Add(icon);
                }
                else if(visualData.Mode == Mode.Disorder)
                {
                    icon = new VisualElement();
                    icon.style.backgroundColor = DocStyle.Current.FrontgroundColor;
                    icon.style.SetIS_Style(radius);
                    icon.style.scale = iconScale;
                    icons.Add(icon);
                    textElement.Add(icon);
                }
                icon.style.width = iconWidth;
                icon.style.height = iconWidth;
                icon.style.left = iconWidth * -1;
                textElement.style.paddingLeft = iconWidth * 2f;
                Add(textElement);
            }
        }

        public static DocComponent CreateComponent(IEnumerable<string> items, Mode mode = Mode.Disorder)
        {
            DocComponent component = new()
            {
                VisualID = "8",
                TextData = new(items),
            };
            SaveJsonData(component, new Data() { Mode = mode });
            return component;
        }
        public static DocVisual Create(IEnumerable<string> items, Mode mode = Mode.Disorder)
        {
            return Create(CreateComponent(items, mode));
        }
    }
}
