using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Advance/See Also")]
    public class DocEditSeeAlso : DocEditVisual<DocSeeAlso.Data>
    {
        [Obsolete] public override string DisplayName => "Advance/See Also";

        public override string VisualID => "6";

        protected override void OnCreateGUI()
        {
            init();
            this.Add(generateTextVisual());
            this.Add(generateHeightModeVisual());
        }

        public override string ToMarkdown(string dstPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Target.TextData[0]);
            if (Target.TextData[0] != "")
                stringBuilder.Append("\t");
            switch (visualData.mode)
            {
                case DocSeeAlso.Mode.OpenPage:
                    stringBuilder.Append("See Also : ");
                    stringBuilder.Append(Target.ObjsData[0].name);
                    break;
                case DocSeeAlso.Mode.OpenUrl:
                    string strLink = $"[{Target.TextData[1]}]({visualData.url})";
                    stringBuilder.Append(strLink);
                    break;
            }

            return stringBuilder.ToString();
        }

        private void init()
        {
            while (Target.TextData.Count < 2)
            {
                Target.TextData.Add("");
            }

            while (Target.TextData.Count < 2)
            {
                Target.TextData.Add("");
            }
        }

        private VisualElement generateTextVisual()
        {
            VisualElement root = new DSHorizontal();
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(40));
            TextField descriptionField = new DSTextField("description", (value) =>
            {
                Target.TextData[0] = value.newValue;
            });
            descriptionField.value = Target.TextData[0];
            descriptionField.style.width = Length.Percent(50);
            root.Add(descriptionField);
            TextField buttonTextField = new DSTextField("buttonText", (value) =>
            {
                Target.TextData[1] = value.newValue;
            });
            buttonTextField.value = Target.TextData[1];
            buttonTextField.style.paddingLeft = DocStyle.Current.MainTextSize;
            buttonTextField.style.width = Length.Percent(50);
            root.Add(buttonTextField);

            DocStyle.Current.EndLabelWidth();

            return root;
        }

        private VisualElement generateHeightModeVisual()
        {
            VisualElement root = new DSHorizontal();
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(30));
            IntegerField integerField = DocEditor.NewIntField("height", (value) =>
            {
                visualData.height = value.newValue;
                SaveDataToTarget();
            });
            integerField.value = visualData.height;
            integerField.style.width = Length.Percent(30);
            root.Add(integerField);
            DocStyle.Current.EndLabelWidth();
            VisualElement veMode = generateModeVisual();
            veMode.style.width = Length.Percent(70);
            root.Add(veMode);

            return root;
        }

        private VisualElement generateModeVisual()
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            DocStyle.Current.BeginLabelWidth(ISLength.Pixel(40));
            ObjectField objectField = DocEditor.NewObjectField<SODocPage>("page", (value) =>
            {
                Target.ObjsData[0] = value.newValue;
            });
            objectField.style.width = Length.Percent(50);
            objectField.style.paddingLeft = DocStyle.Current.MainTextSize;
            while (Target.ObjsData.Count < 1)
                Target.ObjsData.Add(null); 
            objectField.value = Target.ObjsData[0];
            TextField urlTextField = new DSTextField("url", (value) =>
            {
                visualData.url = value.newValue;
                SaveDataToTarget();
            });
            urlTextField.value = visualData.url;
            urlTextField.style.paddingLeft = DocStyle.Current.MainTextSize;
            urlTextField.style.width = Length.Percent(50);
            DocStyle.Current.EndLabelWidth();
            var enumField = new DSEnumField<DocSeeAlso.Mode>("Mode", visualData.mode, (value) =>
            {
                visualData.mode = value.newValue;
                if (visualData.mode == DocSeeAlso.Mode.OpenPage)
                {
                    root.Remove(urlTextField);
                    root.Add(objectField);
                }
                else
                {
                    root.Remove(objectField);
                    root.Add(urlTextField);
                }
                SaveDataToTarget();
            });
            enumField.style.paddingLeft = DocStyle.Current.MainTextSize;
            enumField.labelElement.style.minWidth = Length.Percent(30);
            enumField.labelElement.style.width = Length.Percent(30);
            enumField.style.width = Length.Percent(44);
            root.Add(enumField);
            if (visualData.mode == DocSeeAlso.Mode.OpenPage)
            {
                if (root.Contains(urlTextField))
                    root.Remove(urlTextField);
                root.Add(objectField);
            }
            else
            {
                if (root.Contains(objectField))
                    root.Remove(objectField);
                root.Add(urlTextField);
            }

            return root;
        }
    }
}
