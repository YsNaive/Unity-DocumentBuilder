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
            TextField field = new TextField();
            field.multiline = true;
            field.value = docComponent.JsonData;
            return field;
        }

        public override VisualElement CreateViewGUI(DocComponent docComponent, int width)
        {
            TextElement textElement = new TextElement();
            textElement.style.width = width;
            textElement.text = docComponent.JsonData;
            textElement.style.SetIS_Style(DocStyle.Current.MainText);
            return textElement;
        }

        public override DocComponent SaveTo(VisualElement visualElement, DocComponent docComponent)
        {
            docComponent.JsonData = ((TextField)visualElement).value;
            return docComponent;
        }
    }

}