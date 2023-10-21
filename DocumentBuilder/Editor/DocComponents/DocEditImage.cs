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
    [CustomDocEditVisual("Charts/Image")]
    public class DocEditImage : DocEditVisual
    {
        [Obsolete] public override string DisplayName => "Image";

        public override string VisualID => "5";

        private VisualElement root, urlVisual, objVisual;

        protected override void OnCreateGUI()
        {
            DocImage.Data data = JsonUtility.FromJson<DocImage.Data>(Target.JsonData);
            if (data == null)
                data = new DocImage.Data();
            root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            TextField scaleField = DocRuntime.NewTextField("Scale", value =>
            {
                float.TryParse(value.newValue, out data.scale);
                Target.JsonData = JsonUtility.ToJson(data);
            });
            DocStyle.Current.EndLabelWidth();
            scaleField.value = data.scale + "";
            scaleField.style.width = Length.Percent(39);
            EnumField enumField = DocEditor.NewEnumField("", data.mode, value =>
            {
                data.mode = (DocImage.Mode)value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
                urlObjDisplay(data.mode);
            });
            enumField.style.width = Length.Percent(20);
            urlVisual = generateUrlVisual(data);
            objVisual = generateObjVisual(data);
            root.Add(scaleField);
            root.Add(enumField);
            this.Add(root);
            urlObjDisplay(data.mode);
        }

        public override string ToMarkdown(string dstPath)
        {
            DocImage.Data data = JsonUtility.FromJson<DocImage.Data>(Target.JsonData);
            StringBuilder stringBuilder = new StringBuilder();
            switch (data.mode)
            {
                case DocImage.Mode.Url:
                    string strImage = $"![]({data.url})";
                    stringBuilder.Append(strImage);
                    break;
                case DocImage.Mode.Object:
                    break;
            }

            return stringBuilder.ToString();
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

        private VisualElement generateObjVisual(DocImage.Data data)
        {
            Texture2D texture;
            if (Target.ObjsData.Count != 0)
                texture = (Texture2D)Target.ObjsData[0];
            else
                texture = null;
            ObjectField objectField = DocEditor.NewObjectField<Texture2D>(value =>
            {
                Target.ObjsData.Clear();
                Target.ObjsData.Add(value.newValue);
            });
            objectField.style.width = Length.Percent(40);
            objectField.value = texture;

            return objectField;
        }

        private VisualElement generateUrlVisual(DocImage.Data data)
        {
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(10));
            TextField urlField = DocRuntime.NewTextField("Url", value =>
            {
                data.url = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            DocStyle.Current.EndLabelWidth();
            urlField.value = data.url + "";
            urlField.style.width = Length.Percent(40);

            return urlField;
        }
    }
}
