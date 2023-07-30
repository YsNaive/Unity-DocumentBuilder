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
            VisualElement introBar = new VisualElement();
            introBar.style.SetIS_Style(ISFlex.Horizontal);
            IntegerField introTime = new IntegerField();
            introTime.style.width = Length.Percent(25);
            introTime.style.ClearMarginPadding();
            introTime.value = data.IntroAniTime;
            introTime[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introTime.RegisterValueChangedCallback(val =>
            {
                data.IntroAniTime = val.newValue;
                save();
            });
            EnumField introType = new EnumField();
            introType.Init(DocDescription.AniMode.Fade);
            introType.value = data.IntroMode;
            introType.style.width = Length.Percent(22);
            introType.label = "in";
            introType[0].style.minWidth = 20;
            introType[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introType.style.ClearMarginPadding();
            introType.RegisterValueChangedCallback(val =>
            {
                data.IntroMode = (DocDescription.AniMode)val.newValue;
                introTime.visible = data.IntroMode != DocDescription.AniMode.None;
                introTime.value = data.IntroMode == DocDescription.AniMode.Fade ? 250 : 25;
                save();
            });
            introBar.Add(introType);
            introBar.Add(introTime);
            Add(introBar);


            IntegerField outtroTime = new IntegerField();
            outtroTime.style.width = Length.Percent(25);
            outtroTime.style.ClearMarginPadding();
            outtroTime.value = data.IntroAniTime;
            outtroTime[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroTime.RegisterValueChangedCallback(val =>
            {
                data.OutroAniTime = val.newValue;
                save();
            });
            EnumField outtroType = new EnumField();
            outtroType.Init(DocDescription.AniMode.Fade);
            outtroType.value = data.IntroMode;
            outtroType.style.width = Length.Percent(22);
            outtroType.label = "out";
            outtroType[0].style.minWidth = 20;
            outtroType[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroType.style.ClearMarginPadding();
            outtroType.style.marginLeft = Length.Percent(5);
            outtroType.RegisterValueChangedCallback(val =>
            {
                data.OutroMode = (DocDescription.AniMode)val.newValue;
                outtroTime.visible = data.OutroMode != DocDescription.AniMode.None;
                outtroTime.value = (data.OutroMode == DocDescription.AniMode.Fade ? 250 : 25);
                save();
            });
            introBar.Add(outtroType);
            introBar.Add(outtroTime);



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
            introTime.visible = data.IntroMode != DocDescription.AniMode.None;
            outtroTime.visible = data.OutroMode != DocDescription.AniMode.None;
        }
        void save()
        {
            Target.JsonData = JsonUtility.ToJson(data);
        }
    }
}
