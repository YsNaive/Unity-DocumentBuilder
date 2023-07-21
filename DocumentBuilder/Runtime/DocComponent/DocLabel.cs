using NaiveAPI_UI;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocLabel : DocVisual
    {
        public override string DisplayName => "Label";

        Data data = new Data();
        public override VisualElement CreateEditGUI(DocComponent docComponent, int width)
        {
            data = JsonUtility.FromJson<Data>(docComponent.JsonData);
            data ??= new Data();
            VisualElement hor = new VisualElement();
            hor.style.SetIS_Style(ISFlex.Horizontal);
            TextField field = new TextField();
            field.value = data.Content;
            field.label = DisplayName;
            field.ElementAt(0).style.minWidth = width*0.1f;
            field.style.width = width*0.65f;
            field.RegisterValueChangedCallback((value) =>
            {
                data.Content = value.newValue;
            });
            hor.Add(field);

            // Font Size
            field = new TextField();
            field.value = data.TextSize.ToString();
            field.style.width = width * 0.1f;
            field.RegisterValueChangedCallback((value) =>
            {
                int val;
                if(int.TryParse(value.newValue, out val))
                    data.TextSize = val;
            });
            hor.Add(field);

            // Custom Color
            field = new TextField();
            if (data.TextColor != Color.clear)
                field.value = '#'+ColorUtility.ToHtmlStringRGBA(data.TextColor);
            else
                field.value = "default";
            
            field.style.width = width * 0.15f;
            field.RegisterValueChangedCallback((value) =>
            {
                Color val;
                if (ColorUtility.TryParseHtmlString(value.newValue, out val))
                    data.TextColor = val;
            });
            hor.Add(field);
            return hor;
        }

        public override VisualElement CreateViewGUI(DocComponent docComponent, int width)
        {
            data = JsonUtility.FromJson<Data>(docComponent.JsonData);
            data ??= new Data();
            TextElement text = new TextElement();
            text.text = data.Content;
            text.style.SetIS_Style(DocStyle.Current.LabelText);
            if(data.TextSize != -1)text.style.fontSize = data.TextSize;
            if(data.TextColor != Color.clear)text.style.color = data.TextColor;
            return text;
        }

        public override DocComponent SaveTo(VisualElement visualElement, DocComponent docComponent)
        {
            docComponent.JsonData = JsonUtility.ToJson(data);
            return docComponent;
        }

        [System.Serializable]
        class Data
        {
            public int TextSize = -1;
            public Color TextColor = Color.clear;
            public string Content = "";
        }
    }
}
