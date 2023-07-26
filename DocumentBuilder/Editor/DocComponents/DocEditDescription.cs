using NaiveAPI;
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
        DocDescription.Data data;
        protected override void OnCreateGUI()
        {
            data = JsonUtility.FromJson<DocDescription.Data>(Target.JsonData);
            data ??= new DocDescription.Data();
            VisualElement bar = new VisualElement();
            bar.style.SetIS_Style(ISFlex.Horizontal);
            IntegerField msField = new IntegerField();
            msField.style.width = Length.Percent(40);
            msField.style.ClearMarginPadding();
            msField.style.marginLeft = Length.Percent(9);
            msField.value = data.IntroAniTime;
            msField.label = "Duration";
            msField[0].style.minWidth = 60;
            msField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            msField.RegisterValueChangedCallback(val =>
            {
                data.IntroAniTime = val.newValue;
                save();
            });
            EnumField field = new EnumField();
            field.Init(DocDescription.AniMode.Fade);
            field.value = data.AnimateMode;
            field.style.width = Length.Percent(50);
            field.label = "Intro Mode";
            field[0].style.minWidth = 80;
            field[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            field.style.ClearMarginPadding();
            field.RegisterValueChangedCallback(val =>
            {
                data.AnimateMode = (DocDescription.AniMode)val.newValue;
                msField.visible = data.AnimateMode != DocDescription.AniMode.None;
                save();
            });
            bar.Add(field);
            bar.Add(msField);
            TextField textInput = DocRuntime.NewTextField();
            if (Target.TextData.Count == 0)
                Target.TextData.Add(string.Empty);
            textInput.value = Target.TextData[0];
            textInput.multiline = true;
            textInput.style.ClearMarginPadding();
            textInput.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.TextData.Add(val.newValue);
            });
            save();
            Add(bar);
            Add(textInput);
            msField.visible = data.AnimateMode != DocDescription.AniMode.None;
        }
        void save()
        {
            Target.JsonData = JsonUtility.ToJson(data);
        }
    }
}
