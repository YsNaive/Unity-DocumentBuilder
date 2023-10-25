using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocImage : DocVisual<DocImage.Data>
    {
        public override string VisualID => "5";

        protected override void OnCreateGUI()
        {
            LoadDataFromTarget();

            if (visualData.mode == Mode.Object)
                this.Add(generateObjectVisual(visualData));
            else if (visualData.mode == Mode.Url)
                this.Add(generateUrlVisual(visualData));
            else if (visualData.mode == Mode.Base64)
                this.Add(generateBase64Visual(visualData));
        }

        public VisualElement generateObjectVisual(Data data)
        {
            Label label = new Label();
            label.style.width = Length.Percent(30);
            label.style.height = Length.Percent(20);
            float imageWidth, height;
            Texture2D texture = null;

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

            OnWidthChanged += (newWidth) =>
            {
                if (texture != null)
                {
                    float width = texture.width * data.scale;
                    if (width > newWidth || data.scale == -1)
                        width = newWidth;
                    root.style.width = width * 0.99f;
                    root.style.height = (texture.height * (width / texture.width)) * 0.99f;
                }
            };

            return root;
        }

        public VisualElement generateUrlVisual(Data data)
        {
            VisualElement root = new VisualElement();
            Texture2D img = null;
            float width = 0;
            URLImage urlImage = new URLImage(data.url, (image) => {
                img = image;
                float imageWidth = img.width * data.scale;
                if (imageWidth > width || data.scale == -1)
                    imageWidth = width;
                root.style.width = imageWidth;
                root.style.height = img.height * (imageWidth / image.width);
            });
            urlImage.style.height = Length.Percent(100);
            root.Add(urlImage);

            OnWidthChanged += newWidth =>
            {
                width = newWidth;
                if (img != null)
                {
                    width = img.width * data.scale;
                    if (width > newWidth || data.scale == -1)
                        width = newWidth;
                    root.style.width = width;
                    root.style.height = img.height * (width / img.width);
                }
            };
            return root;
        }

        public VisualElement generateBase64Visual(Data data)
        {
            VisualElement root = new VisualElement();
            Texture2D img = new(1,1);
            float width = 0;
            img.LoadImage(Convert.FromBase64String(data.base64));
            root.style.backgroundImage = img;
            OnWidthChanged += newWidth =>
            {
                width = newWidth;
                if (img != null)
                {
                    width = img.width * data.scale;
                    if (width > newWidth || data.scale == -1)
                        width = newWidth;
                    root.style.width = width;
                    root.style.height = img.height * (width / img.width);
                }
            };
            return root;
        }

        public class Data
        {
            public float scale = -1;
            public string url = "";
            public string base64 = "";
            public Mode mode = Mode.Object;
        }

        public enum Mode
        {
            Url, 
            Object,
            Base64
        }
    }
}