using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

        public override string ToMarkdown(string dstPath)
        {
            DocImage.Data data = JsonUtility.FromJson<DocImage.Data>(Target.JsonData);
            StringBuilder stringBuilder = new StringBuilder();
            switch (data.mode)
            {
                case DocImage.Mode.Url:
                    string strLink = $"![]({data.url})";
                    stringBuilder.AppendLine(strLink);
                    break;
                case DocImage.Mode.Object:
                    break;
            }
            return base.ToMarkdown(dstPath);
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
