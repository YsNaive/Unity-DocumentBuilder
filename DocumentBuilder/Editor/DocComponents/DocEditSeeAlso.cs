using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;
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
            this.Add(generateTextVisual(visualData));
            this.Add(generateHeightModeVisual(visualData));
        }

        public override string ToMarkdown(string dstPath)
        {
            DocSeeAlso.Data data = JsonUtility.FromJson<DocSeeAlso.Data>(Target.JsonData);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Target.TextData[0]);
            if (Target.TextData[0] != "")
                stringBuilder.Append("\t");
            switch (data.mode)
            {
                case DocSeeAlso.Mode.OpenPage:
                    stringBuilder.Append("See Also : ");
                    stringBuilder.Append(Target.ObjsData[0].name);
                    break;
                case DocSeeAlso.Mode.OpenUrl:
                    string strLink = $"[{Target.TextData[1]}]({data.url})";
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

        private VisualElement generateTextVisual(DocSeeAlso.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(40));
            TextField descriptionField = DocRuntime.NewTextField("description", (value) =>
            {
                Target.TextData[0] = value.newValue;
            });
            descriptionField.value = Target.TextData[0];
            descriptionField.style.width = Length.Percent(50);
            root.Add(descriptionField);
            TextField buttonTextField = DocRuntime.NewTextField("buttonText", (value) =>
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

        private VisualElement generateHeightModeVisual(DocSeeAlso.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(30));
            IntegerField integerField = DocEditor.NewIntField("height", (value) =>
            {
                data.height = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            integerField.value = data.height;
            integerField.style.width = Length.Percent(30);
            root.Add(integerField);
            DocStyle.Current.EndLabelWidth();
            VisualElement veMode = generateModeVisual(data);
            veMode.style.width = Length.Percent(70);
            root.Add(veMode);

            return root;
        }

        private VisualElement generateModeVisual(DocSeeAlso.Data data)
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
            objectField.value = Target.ObjsData[0];
            TextField urlTextField = DocRuntime.NewTextField("url", (value) =>
            {
                data.url = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            urlTextField.value = data.url;
            urlTextField.style.paddingLeft = DocStyle.Current.MainTextSize;
            urlTextField.style.width = Length.Percent(50);
            DocStyle.Current.EndLabelWidth();
            EnumField enumField = DocEditor.NewEnumField("Mode", data.mode, (value) =>
            {
                data.mode = (DocSeeAlso.Mode)value.newValue;
                if (data.mode == DocSeeAlso.Mode.OpenPage)
                {
                    root.Remove(urlTextField);
                    root.Add(objectField);
                }
                else
                {
                    root.Remove(objectField);
                    root.Add(urlTextField);
                }
                Target.JsonData = JsonUtility.ToJson(data);
            });
            enumField.style.paddingLeft = DocStyle.Current.MainTextSize;
            enumField[0].style.minWidth = Length.Percent(25);
            enumField.style.width = Length.Percent(50);
            root.Add(enumField);
            if (data.mode == DocSeeAlso.Mode.OpenPage)
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
