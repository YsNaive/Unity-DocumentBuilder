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
    public class DocEditSeeAlso : DocEditVisual
    {
        public override string DisplayName => "Advance/See Also";

        public override string VisualID => "6";

        protected override void OnCreateGUI()
        {
            DocSeeAlso.Data data = setData(Target.JsonData, Target.TextData);
            this.Add(generateAnimVisual(data));
            this.Add(generateTextVisual(data));
            this.Add(generateHeightModeVisual(data));
        }

        private DocSeeAlso.Data setData(string jsonData, List<string> texts)
        {
            DocSeeAlso.Data data = JsonUtility.FromJson<DocSeeAlso.Data>(jsonData);
            if (data == null)
            {
                data = new DocSeeAlso.Data();
                Target.JsonData = JsonUtility.ToJson(data);
                Target.TextData.Clear();
                Target.TextData.Add("");
                Target.TextData.Add("");
                Target.ObjsData.Add(null);
            }

            return data;
        }

        private VisualElement generateAnimVisual(DocSeeAlso.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.paddingLeft = Length.Percent(1);
            root.style.paddingRight = Length.Percent(1);
            VisualElement childIntro = new VisualElement();
            childIntro.style.SetIS_Style(ISFlex.Horizontal);

            EnumField introField = new EnumField();
            introField.Init(DocSeeAlso.AniMode.None);
            introField.value = data.IntroAniMode;
            introField.style.width = Length.Percent(50);
            introField.label = "Intro Mode";
            introField[0].style.minWidth = Length.Percent(30);
            introField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introField.style.ClearMarginPadding();
            childIntro.Add(introField);
            TextField introDurationField = new TextField();
            introDurationField.label = "IntroDuration";
            introDurationField.value = data.IntroDuration.ToString();
            introDurationField.style.width = Length.Percent(50);
            introDurationField[0].style.minWidth = Length.Percent(30);
            introDurationField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introDurationField.visible = data.IntroAniMode != DocSeeAlso.AniMode.None;
            introDurationField.style.ClearMarginPadding();
            childIntro.Add(introDurationField);
            introField.RegisterValueChangedCallback(value =>
            {
                data.IntroAniMode = (DocSeeAlso.AniMode)value.newValue;
                introDurationField.visible = data.IntroAniMode != DocSeeAlso.AniMode.None;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            introDurationField.RegisterValueChangedCallback(value =>
            {
                if (int.TryParse(value.newValue, out int duration))
                {
                    data.IntroDuration = duration;
                    Target.JsonData = JsonUtility.ToJson(data);
                }
            });
            VisualElement childOuttro = new VisualElement();
            childOuttro.style.SetIS_Style(ISFlex.Horizontal);
            EnumField outtroField = new EnumField();
            outtroField.Init(DocSeeAlso.AniMode.None);
            outtroField.label = "Outtro Mode";
            outtroField.value = data.IntroAniMode;
            outtroField.style.width = Length.Percent(50);
            outtroField[0].style.minWidth = Length.Percent(30);
            outtroField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroField.style.ClearMarginPadding();
            childOuttro.Add(outtroField);
            TextField outtroDurationField = new TextField();
            outtroDurationField.label = "OuttroDuration";
            outtroDurationField.value = data.OuttroDuration.ToString();
            outtroDurationField.style.width = Length.Percent(50);
            outtroDurationField[0].style.minWidth = Length.Percent(30);
            outtroDurationField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroDurationField.visible = data.OuttroAniMode != DocSeeAlso.AniMode.None;
            outtroDurationField.style.ClearMarginPadding();
            childOuttro.Add(outtroDurationField);
            outtroField.RegisterValueChangedCallback(value =>
            {
                data.OuttroAniMode = (DocSeeAlso.AniMode)value.newValue;
                outtroDurationField.visible = data.OuttroAniMode != DocSeeAlso.AniMode.None;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            outtroDurationField.RegisterValueChangedCallback(value =>
            {
                if (int.TryParse(value.newValue, out int duration))
                {
                    data.OuttroDuration = duration;
                    Target.JsonData = JsonUtility.ToJson(data);
                }
            });
            root.Add(childIntro);
            root.Add(childOuttro);

            return root;
        }

        private VisualElement generateTextVisual(DocSeeAlso.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
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

            return root;
        }

        private VisualElement generateHeightModeVisual(DocSeeAlso.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            IntegerField integerField = DocEditor.NewIntField("height", (value) =>
            {
                data.height = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            integerField.style.width = Length.Percent(50);
            integerField.value = data.height;
            root.Add(integerField);
            VisualElement veMode = generateModeVisual(data);
            veMode.style.width = Length.Percent(50);
            root.Add(veMode);

            return root;
        }

        private VisualElement generateModeVisual(DocSeeAlso.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            ObjectField objectField = DocEditor.NewObjectField<SODocPage>("page", (value) =>
            {
                Target.ObjsData[0] = value.newValue;
            });
            objectField.style.width = Length.Percent(50);
            objectField.style.paddingLeft = DocStyle.Current.MainTextSize;
            objectField[0].style.minWidth = Length.Percent(25);
            objectField.value = Target.ObjsData[0];
            TextField urlTextField = DocRuntime.NewTextField("url", (value) =>
            {
                data.url = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            urlTextField.value = data.url;
            urlTextField.style.paddingLeft = DocStyle.Current.MainTextSize;
            urlTextField[0].style.minWidth = Length.Percent(25);
            urlTextField.style.width = Length.Percent(50);
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
            root.Add(enumField);
            enumField[0].style.minWidth = Length.Percent(25);
            enumField.style.width = Length.Percent(50);
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
