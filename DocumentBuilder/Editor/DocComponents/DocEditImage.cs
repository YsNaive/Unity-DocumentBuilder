using NaiveAPI.DocumentBuilder;
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

        private VisualElement urlVisual, objVisual;

        public override void OnCreateGUI()
        {
            DocImage.Data data = JsonUtility.FromJson<DocImage.Data>(Target.JsonData);
            if (data == null)
                data = new DocImage.Data();
            VisualElement root = new VisualElement();
            TextField scaleField = new TextField();
            scaleField.label = "scale";
            scaleField.value = data.scale + "";
            scaleField.style.width = -1;
            scaleField.RegisterValueChangedCallback(value =>
            {
                float.TryParse(value.newValue, out data.scale);
                Target.JsonData = JsonUtility.ToJson(data);
            });
            EnumField enumField = new EnumField();
            enumField.Init(DocImage.Mode.Url);
            enumField.style.width = -1;
            enumField.RegisterValueChangedCallback(value =>
            {
                data.mode = (DocImage.Mode) value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
                urlObjDisplay(data.mode);
            });
            urlVisual = generateUrlVisual(data);
            objVisual = generateObjVisual(data);
            this.Add(scaleField);
            this.Add(enumField);
            urlObjDisplay(data.mode);
        }

        private void urlObjDisplay(DocImage.Mode mode)
        {
            if (mode == DocImage.Mode.Url)
            {
                if (this.Contains(objVisual))
                    this.Remove(objVisual);
                this.Add(urlVisual);
            }
            else if (mode == DocImage.Mode.Object)
            {
                if (this.Contains(urlVisual))
                    this.Remove(urlVisual);
                this.Add(objVisual);
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
            objectField.value = texture;
            objectField.style.width = -1;
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
            urlField.style.width = -1;
            urlField.RegisterValueChangedCallback(value =>
            {
                data.url = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });

            return urlField;
        }
    }
}
