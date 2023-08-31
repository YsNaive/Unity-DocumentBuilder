using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditDescription : DocEditVisual
    {
        private static ISPadding padding = ISPadding.Pixel(5);
        public override string DisplayName => "Description";
        public override string VisualID => "2";
        protected override Enum InitAniType => DocDescription.AniMode.Fade;
        DocDescription.Data data;
        protected override void OnCreateGUI()
        {
            data = JsonUtility.FromJson<DocDescription.Data>(Target.JsonData);
            data ??= new DocDescription.Data();

            TextField textInput = DocRuntime.NewTextField();
            if (Target.TextData.Count == 0)
                Target.TextData.Add(string.Empty);
            textInput.value = Target.TextData[0];
            textInput.multiline = true;
            textInput[0].style.SetIS_Style(padding);
            textInput.style.whiteSpace = WhiteSpace.Normal;
            textInput.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.TextData.Add(val.newValue);
            });
            var typeField = DocEditor.NewEnumField("Type", data.Type, e =>
            {
                data.Type = (DocDescription.Type)e.newValue;
                save();
            });
            typeField[0].style.minWidth = 45;
            typeField[1].style.paddingLeft = 4;
            typeField[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            Add(typeField);
            Add(textInput);
        }
        void save()
        {
            Target.JsonData = JsonUtility.ToJson(data);
        }

        public override string ToMarkdown(string dstPath)
        {
            return Target.TextData[0];
        }
    }
}
