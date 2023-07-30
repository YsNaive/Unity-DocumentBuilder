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
            textInput.style.ClearMarginPadding();
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
            Add(typeField);
            Add(textInput);
        }
        void save()
        {
            Target.JsonData = JsonUtility.ToJson(data);
        }
    }
}
