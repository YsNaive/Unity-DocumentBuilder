using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocDescription : DocVisual
    {
        public override string DisplayName => "Description";

        public override VisualElement CreateEditGUI(DocComponent docComponent, int width)
        {
            Data data = getData(docComponent);
            TextField field = new TextField();
            field.multiline = true;
            field.value = data.Content;
            return field;
        }

        public override VisualElement CreateViewGUI(DocComponent docComponent, int width)
        {
            TextElement textElement = new TextElement();
            textElement.style.width = width;
            textElement.text = getData(docComponent).Content;
            textElement.style.SetIS_Style(DocStyle.Current.MainText);
            return textElement;
        }

        public override DocComponent SaveTo(VisualElement visualElement, DocComponent docComponent)
        {
            Data data = new Data();
            data.Content = ((TextField)visualElement).value;
            docComponent.JsonData = JsonUtility.ToJson(data);
            return docComponent;
        }
        [System.Serializable]
        class Data
        {
            public string Content;
        }
        Data getData(DocComponent docComponent)
        {
            Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
            if(data == null) { data = new Data(); }
            return data;
        }
    }

}