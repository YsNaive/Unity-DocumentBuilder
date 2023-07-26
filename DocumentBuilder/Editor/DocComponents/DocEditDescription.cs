using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
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
        public override void OnCreateGUI()
        {
            DocDescription.Data data = JsonUtility.FromJson<DocDescription.Data>(Target.JsonData);
            data ??= new DocDescription.Data();
            VisualElement bar = new VisualElement();
            bar.style.SetIS_Style(ISFlex.Horizontal);
            EnumField field = new EnumField();
            field.Init(DocDescription.AniMode.Fade);
            field.value = data.AnimateMode;
            field.style.width = Length.Percent(49);
            bar.Add(field);
            IntegerField msField = new IntegerField();
            msField.style.width = Length.Percent(49);
            msField.value = data.IntroAniTime;
            bar.Add(msField);
            TextField textInput = new TextField();
            if (Target.TextData.Count == 0)
                Target.TextData.Add(string.Empty);
            textInput.value = Target.TextData[0];
            textInput.multiline = true;
            textInput.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.TextData.Add(val.newValue);
            });
            Add(bar);
            Add(textInput);
        }

    }
}
