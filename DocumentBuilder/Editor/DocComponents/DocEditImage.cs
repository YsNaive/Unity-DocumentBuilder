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
    public class DocEditImage : DocEditVisual
    {
        public override string DisplayName => "Image";

        public override string VisualID => "5";

        private VisualElement root, urlVisual, objVisual;

        protected override void OnCreateGUI()
        {
            DocImage.Data data = JsonUtility.FromJson<DocImage.Data>(Target.JsonData);
            if (data == null)
                data = new DocImage.Data();
            this.Add(generateAnimVisual(data));
            root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            TextField scaleField = new TextField();
            scaleField.label = "scale";
            scaleField[0].style.minWidth = Length.Percent(20);
            scaleField.value = data.scale + "";
            scaleField.style.width = Length.Percent(39);
            scaleField.style.SetIS_Style(ISMargin.None);
            scaleField.RegisterValueChangedCallback(value =>
            {
                float.TryParse(value.newValue, out data.scale);
                Target.JsonData = JsonUtility.ToJson(data);
            });
            EnumField enumField = new EnumField();
            enumField.Init(DocImage.Mode.Object);
            enumField.value = data.mode;
            enumField.style.width = Length.Percent(20);
            enumField.style.SetIS_Style(ISMargin.None);
            enumField.RegisterValueChangedCallback(value =>
            {
                data.mode = (DocImage.Mode) value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
                urlObjDisplay(data.mode);
            });
            urlVisual = generateUrlVisual(data);
            objVisual = generateObjVisual(data);
            root.Add(scaleField);
            root.Add(enumField);
            this.Add(root);
            urlObjDisplay(data.mode);
        }

        private void urlObjDisplay(DocImage.Mode mode)
        {
            if (mode == DocImage.Mode.Url)
            {
                if (root.Contains(objVisual))
                    root.Remove(objVisual);
                root.Add(urlVisual);
            }
            else if (mode == DocImage.Mode.Object)
            {
                if (root.Contains(urlVisual))
                    root.Remove(urlVisual);
                root.Add(objVisual);
            }
        }

        private VisualElement generateAnimVisual(DocImage.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);

            VisualElement childIntro = DocRuntime.NewEmptyHorizontal();
            EnumField introField = new EnumField();
            introField.Init(DocImage.AniMode.None);
            introField.value = data.IntroAniMode;
            introField.style.width = Length.Percent(50);
            introField.label = "Intro Mode";
            introField[0].style.minWidth = Length.Percent(50);
            introField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introField.style.ClearMarginPadding();
            childIntro.Add(introField);
            TextField introDurationField = new TextField();
            introDurationField.label = "IntroDuration";
            introDurationField.value = data.IntroDuration.ToString();
            introDurationField.style.width = Length.Percent(50);
            introDurationField[0].style.minWidth = Length.Percent(50);
            introDurationField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introDurationField.visible = data.IntroAniMode != DocImage.AniMode.None;
            introDurationField.style.ClearMarginPadding();
            childIntro.Add(introDurationField);
            introField.RegisterValueChangedCallback(value =>
            {
                data.IntroAniMode = (DocImage.AniMode)value.newValue;
                introDurationField.visible = data.IntroAniMode != DocImage.AniMode.None;
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
            VisualElement childOuttro = DocRuntime.NewEmptyHorizontal();
            EnumField outtroField = new EnumField();
            outtroField.Init(DocImage.AniMode.None);
            outtroField.label = "Outtro Mode";
            outtroField.value = data.IntroAniMode;
            outtroField.style.width = Length.Percent(50);
            outtroField[0].style.minWidth = Length.Percent(50);
            outtroField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroField.style.ClearMarginPadding();
            childOuttro.Add(outtroField);
            TextField outtroDurationField = new TextField();
            outtroDurationField.label = "OuttroDuration";
            outtroDurationField.value = data.OuttroDuration.ToString();
            outtroDurationField.style.width = Length.Percent(50);
            outtroDurationField[0].style.minWidth = Length.Percent(50);
            outtroDurationField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroDurationField.visible = data.OuttroAniMode != DocImage.AniMode.None;
            outtroDurationField.style.ClearMarginPadding();
            childOuttro.Add(outtroDurationField);
            outtroField.RegisterValueChangedCallback(value =>
            {
                data.OuttroAniMode = (DocImage.AniMode)value.newValue;
                outtroDurationField.visible = data.OuttroAniMode != DocImage.AniMode.None;
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

        private VisualElement generateObjVisual(DocImage.Data data)
        {
            Texture2D texture;
            if (Target.ObjsData.Count != 0)
                texture = (Texture2D)Target.ObjsData[0];
            else
                texture = null;
            ObjectField objectField = new ObjectField();
            objectField.objectType = typeof(Texture2D);
            objectField.style.width = Length.Percent(40);
            objectField.style.SetIS_Style(ISMargin.None);
            objectField.value = texture;
            objectField.RegisterValueChangedCallback(value =>
            {
                Target.ObjsData.Clear();
                Target.ObjsData.Add(value.newValue);
            });

            return objectField;
        }

        private VisualElement generateUrlVisual(DocImage.Data data)
        {
            TextField urlField = new TextField();
            urlField.label = "url";
            urlField.value = data.url + "";
            urlField.style.width = Length.Percent(40);
            urlField[0].style.minWidth = Length.Percent(10);
            urlField.style.SetIS_Style(ISMargin.None);
            urlField.RegisterValueChangedCallback(value =>
            {
                data.url = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });

            return urlField;
        }
    }
}
