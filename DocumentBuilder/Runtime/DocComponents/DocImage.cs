using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocImage : DocVisual
    {
        public override string VisualID => "5";

        public override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            if (data == null)
                data = new Data();
            if (data.mode == Mode.Object)
                this.Add(generateObjectVisual(data));
            else if (data.mode == Mode.Url)
                this.Add(generateUrlVisual(data));
        }

        public VisualElement generateObjectVisual(Data data)
        {
            Label label = new Label();
            label.style.width = Length.Percent(30);
            label.style.height = Length.Percent(20);
            float imageWidth, height;
            Texture2D texture;

            VisualElement root = new VisualElement();

            if (Target.ObjsData.Count > 0 && Target.ObjsData[0] != null)
            {
                texture = (Texture2D)Target.ObjsData[0];
                root.style.backgroundImage = texture;
                if (-1 < texture.width * data.scale || data.scale == -1)
                    imageWidth = -1;
                else
                    imageWidth = texture.width * data.scale;
                height = texture.height * (imageWidth / texture.width);
                label.visible = false;
            }
            else
            {
                root.style.backgroundColor = Color.black;
                label.text = "No Image";
                imageWidth = -1;
                height = imageWidth * 0.6f;
            }
            ISPosition position = new ISPosition();
            root.style.width = imageWidth;
            root.style.height = height;
            position.Left = ISStyleLength.Percent(35);
            position.Top = ISStyleLength.Percent(40);
            label.style.SetIS_Style(DocStyle.Current.LabelText);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.SetIS_Style(position);
            root.Add(label);

            return root;
        }

        public VisualElement generateUrlVisual(Data data)
        {
            VisualElement root = new VisualElement();
            Texture2D img = null;
            float width = 0;
            URLImage urlImage = new URLImage(data.url, (image) => {
                img = image;
                root.style.height = img.height * (width / image.width);
            });
            urlImage.RegisterCallback<GeometryChangedEvent>(e =>
            {
                width = e.newRect.width;
                if (e.oldRect.width != e.newRect.width && img != null)
                {
                    root.style.height = img.height * (e.newRect.width / img.width);
                }
            });
            urlImage.style.height = Length.Percent(100);
            root.Add(urlImage);
            return root;
        }
        public class Data
        {
            public float scale = -1;
            public string url = "";
            public Mode mode = Mode.Url;
        }

        public enum Mode
        {
            Url, Object
        }
    }
}