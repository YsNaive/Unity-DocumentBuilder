using NaiveAPI_UI;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public sealed class DocImage : DocVisual<DocImage.Data>
    {
        public Action<float> OnHeightChanged;
        public Action<float> OnWidthChanged;
        private int m_val;
        public int Value
        {
            get
            {
                return m_val;
            }
            set
            {
                m_val = value;
            }
        }
        public enum Mode
        {
            Url,
            Object,
            Base64
        }
        public override string VisualID => "5";
        static ISBorder border = new ISBorder(DocStyle.Current.SubBackgroundColor, 1f);
        public DocImage()
        {
            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (e.oldRect.width != e.newRect.width)
                    OnWidthChanged?.Invoke(e.newRect.width);
                if (e.oldRect.height != e.newRect.height)
                    OnHeightChanged?.Invoke(e.newRect.height);
            });
        }
        protected override void OnCreateGUI()
        {
            MakeValid(Target);
            if (visualData.mode == Mode.Object)
                this.Add(new ObjVisual(this));
            else if (visualData.mode == Mode.Url)
                this.Add(new UrlVisual(this));
            else if (visualData.mode == Mode.Base64)
                this.Add(new Base64Visual(this));
        }

        public static void MakeValid(DocComponent component)
        {
            var data = LoadJsonData(component);
            while(component.ObjsData.Count < 1)
                component.ObjsData.Add(null);
            while (component.ObjsData.Count > 1)
                component.ObjsData.RemoveAt(component.ObjsData.Count - 1);

            if(data.mode == Mode.Url)
            {
                data.base64 = "";
                component.ObjsData[0] = null;    
            }
            else if(data.mode == Mode.Object)
            {
                data.base64 = "";
                data.url = "";
            }
            else if(data.mode == Mode.Base64)
            {
                data.url = "";
                component.ObjsData[0] = null;
            }
            SaveJsonData(component, data);
        }

        public class Data
        {
            public float scale = -1;
            public string url = "";
            public string base64 = "";
            public Mode mode = Mode.Base64;
        }

        private class ObjVisual : VisualElement
        {
            public ObjVisual(DocImage docImage)
            {
                if (docImage.Target.ObjsData[0] != null)
                {
                    var texture = (Texture2D)docImage.Target.ObjsData[0];
                    style.backgroundImage = texture;
                    docImage.OnWidthChanged += (newWidth) =>
                    {
                        float width = texture.width * docImage.visualData.scale;
                        if (width > newWidth || docImage.visualData.scale == -1)
                            width = newWidth;
                        style.width = width * 0.99f;
                        style.height = (texture.height * (width / texture.width)) * 0.99f;
                    };
                }
                else
                {
                    style.height = DocStyle.Current.LineHeight * 3f;
                    style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    var label = new DSLabel("No Image");
                    label.style.flexGrow = 1f;
                    label.style.unityTextAlign = TextAnchor.MiddleCenter;
                    Add(label);
                }
                style.SetIS_Style(border);
            }
        }
        private class UrlVisual : VisualElement
        {
            public UrlVisual(DocImage docImage)
            {
                Texture2D img = null;
                float width = 0;
                URLImage urlImage = new URLImage(docImage.visualData.url, (image) => {
                    img = image;
                    float imageWidth = img.width * docImage.visualData.scale;
                    if (imageWidth > width || docImage.visualData.scale == -1)
                        imageWidth = width;
                    style.width = imageWidth;
                    style.height = img.height * (imageWidth / image.width);
                });
                urlImage.style.flexGrow = 1f;
                Add(urlImage);

                docImage.OnWidthChanged += newWidth =>
                {
                    width = newWidth;
                    if (img != null)
                    {
                        width = img.width * docImage.visualData.scale;
                        if (width > newWidth || docImage.visualData.scale == -1)
                            width = newWidth;
                        style.width = width;
                        style.height = img.height * (width / img.width);
                    }
                };
                style.SetIS_Style(border);
            }
        }
        private class Base64Visual : VisualElement
        {
            public Base64Visual(DocImage docImage)
            {
                if(docImage.visualData.base64 != "")
                {
                    Texture2D img = new(1, 1);
                    img.LoadImage(Convert.FromBase64String(docImage.visualData.base64));
                    style.backgroundImage = img;
                    docImage.OnWidthChanged += newWidth =>
                    {
                        if (img != null)
                        {
                            var width = img.width * docImage.visualData.scale;
                            if (width > newWidth || docImage.visualData.scale == -1)
                                width = newWidth;
                            style.width = width;
                            style.height = img.height * (width / img.width);
                        }
                    };
                }
                else
                {
                    style.height = DocStyle.Current.LineHeight * 3f;
                    style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    var label = new DSLabel("No Image");
                    label.style.flexGrow = 1f;
                    label.style.unityTextAlign = TextAnchor.MiddleCenter;
                    Add(label);
                }
                style.SetIS_Style(border);
            }
        }

        public static DocVisual Create(Texture2D texture, float scale = -1) { return Create(CreateComponent(texture, scale)); }
        public static DocComponent CreateComponent(Texture2D texture, float scale = -1)
        {
            DocComponent component = new DocComponent();
            component.VisualID = "5";
            component.ObjsData.Add(texture);
            SaveJsonData(component, new Data { mode = Mode.Object, scale = scale });
            return component;
        }
        public static DocVisual Create(string url, float scale = -1) { return Create(CreateComponent(url, scale)); }
        public static DocComponent CreateComponent(string url, float scale = -1)
        {
            DocComponent component = new DocComponent();
            component.VisualID = "5";
            SaveJsonData(component, new Data { mode = Mode.Url, url = url, scale = scale });
            return component;
        }
    }
}