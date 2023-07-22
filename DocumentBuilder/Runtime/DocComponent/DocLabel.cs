using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocLabel : DocVisual
    {
        public override string DisplayName => "Label";

        public override VisualElement CreateEditGUI(DocComponent docComponent, int width)
        {
            Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
            data ??= new Data();
            VisualElement hor = new VisualElement();
            hor.style.SetIS_Style(ISFlex.Horizontal);
            TextField field = new TextField();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Level; i++) sb.Append('#');
            sb.Append(' ');
            data.Content = sb.ToString() + data.Content;
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
            Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
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
            Data newData = new Data();
            newData.Content = ((TextField)visualElement[0]).value;
            newData.Level = 0;
            int index = -1;
            foreach (char c in newData.Content)
            {
                index++;
                if (c == '#') newData.Level++;
                else if (c == ' ') continue;
                else break;
            }
            newData.Content = newData.Content.Substring(index);
            if (!int.TryParse(((TextField)visualElement[1]).value, out newData.TextSize))
                newData.TextSize = -1;
            if (!ColorUtility.TryParseHtmlString(((TextField)visualElement[2]).value, out newData.TextColor))
                newData.TextColor = Color.clear;
            docComponent.JsonData = JsonUtility.ToJson(newData);
            docComponent.ObjData.Clear();
            return docComponent;
        }

        [System.Serializable]
        class Data
        {
            public int TextSize = -1;
            public Color TextColor = Color.clear;
            public string Content = "";
            public int Level = 0;
        }
    }
}
