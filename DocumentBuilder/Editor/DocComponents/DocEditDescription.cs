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
    public class DocEditDescription : DocEditVisual<ValueTuple<DocDescription.DescriptionType>>
    {
        private static ISPadding padding = ISPadding.Pixel(5);
        public override string DisplayName => "Description";
        public override string VisualID => "2";
        public override ushort Version => 1;
        protected override Enum InitAniType => DocDescription.AniMode.Fade;
        protected override void OnCreateGUI()
        {
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
            var typeField = DocEditor.NewEnumField("Type", visualData.Item1, e =>
            {
                visualData.Item1 = (DocDescription.DescriptionType)e.newValue;
                SaveDataToTarget();
            });
            typeField[0].style.minWidth = 45;
            typeField[1].style.paddingLeft = 4;
            typeField[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            Add(typeField);
            Add(textInput);
        }
        public override string ToMarkdown(string dstPath)
        {
            return Target.TextData[0];
        }
        protected override void VersionConflict()
        {
            if(Target.VisualVersion < 1)
            {
                Target.JsonData = Target.JsonData.Replace("Type", "Item1");
            }
            Target.VisualVersion = 1;
        }
    }
}
