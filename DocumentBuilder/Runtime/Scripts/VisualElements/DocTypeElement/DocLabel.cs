using NaiveAPI_UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocLabel : DocVisual<DocLabel.Data>
    {
        public override string VisualID => "1";

        protected override void OnCreateGUI()
        {
            TextElement text = new TextElement();
            if (Target.TextData.Count > 0)
                text.text = Target.TextData[0];
            text.style.SetIS_Style(DocStyle.Current.LabelText);
            text.style.fontSize = Mathf.Clamp(text.style.fontSize.value.value - (2 * (visualData.Level - 1)), DocStyle.Current.MainTextSize, float.MaxValue);
            Add(text);
        }

        public class Data
        {
            public int Level = 1;
        }

        public static DocComponent CreateComponent(string text, int level = 1)
        {
            DocComponent component = new DocComponent();
            component.VisualID = "1";
            component.TextData.Add(text);

            Data data = new Data();
            data.Level = level;
            component.JsonData = JsonUtility.ToJson(data);

            return component;
        }
        public static DocVisual Create(string text, int level = 1)
        {
            return Create(CreateComponent(text, level));
        }
    }

}