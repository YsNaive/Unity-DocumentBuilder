using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI
{
    public static class DocRuntime
    {
        static DocRuntime()
        {
            Reload();
        }
        public static Dictionary<string, Type> VisualID_Dict = new Dictionary<string, Type>();

        public static void Reload()
        {
            VisualID_Dict.Clear();
            Type baseType = typeof(DocVisual);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                    {
                        DocVisual doc = (DocVisual)System.Activator.CreateInstance(type);
                        VisualID_Dict.Add(doc.VisualID, type);
                    }
                }
            }
        }
        public static void ApplyMargin(VisualElement ve)
        {
            ve.style.marginTop = DocStyle.Current.MarginVer;
            ve.style.marginLeft = DocStyle.Current.MarginHor;
            ve.style.marginRight = DocStyle.Current.MarginHor;
            ve.style.marginBottom = DocStyle.Current.MarginVer;
        }
        public static VisualElement CreateVisual(DocComponent docComponent)
        {
            Type t;
            if (!VisualID_Dict.TryGetValue(docComponent.VisualID, out t))
            {
                DocDescription textElement = new DocDescription();
                textElement.SetTarget(new DocComponent { VisualID = "2", TextData = new List<string>() { $"Not Fount View for ID \"{docComponent.VisualID}\"" } });
                return textElement;
            }
            DocVisual doc = (DocVisual)Activator.CreateInstance(t);
            doc.SetTarget(docComponent);
            return doc;
        }
        public static VisualElement NewEmpty()
        {
            VisualElement visualElement = new VisualElement();
            visualElement.style.ClearMarginPadding();
            return visualElement;
        }
        public static VisualElement NewEmptyHorizontal()
        {
            VisualElement visualElement = new VisualElement();
            visualElement.style.ClearMarginPadding();
            visualElement.style.flexDirection = FlexDirection.Row;
            return visualElement;
        }
        public static Button NewButton(Action onClick = null) { return NewButton("", DocStyle.Current.SubBackgroundColor, onClick); }
        public static Button NewButton(string text, Action onClick = null) { return NewButton(text, DocStyle.Current.SubBackgroundColor, onClick); }
        public static Button NewButton(Color color, Action onClick = null) { return NewButton("",color, onClick); }
        public static Button NewButton(string text, Color color, Action onClick = null)
        {
            Button button = new Button();
            button.style.height = DocStyle.Current.LineHeight;
            ApplyStyle(button, color);
            button.style.paddingLeft = 4;
            button.style.paddingRight = 4;
            button.text = text;
            if(onClick != null) button.clicked+= onClick;
            return button;
        }
        public static CheckButton NewCheckButton(string text, Action onClick = null) { return NewCheckButton(text, DocStyle.Current.SubBackgroundColor, DocStyle.Current.SuccessColor, DocStyle.Current.DangerColor, onClick); }
        public static CheckButton NewCheckButton(string text, Color color, Action onClick = null) { return NewCheckButton(text, color, DocStyle.Current.SuccessColor, DocStyle.Current.DangerColor, onClick); }
        public static CheckButton NewCheckButton(string text, Color color,Color confirm, Color cancel, Action onClick = null)
        {
            CheckButton button = new CheckButton();
            button.style.height = DocStyle.Current.LineHeight;
            ApplyMargin(button);
            ApplyStyle(button.MainBtn, color);
            ApplyStyle(button.ConfirmButton, confirm);
            ApplyStyle(button.CancelButton, cancel);
            button.ConfirmButton.style.width = Length.Percent(50);
            button.CancelButton.style.width = Length.Percent(50);
            button.RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (e.oldRect.width != e.newRect.width)
                {
                    button.style.width = e.newRect.width;
                };
            });
            button.text = text;
            if (onClick != null) button.Confirm += onClick;
            return button;
        }
        public static TextField NewTextField(string label = "", EventCallback<ChangeEvent<string>> eventCallback = null)
        {
            TextField textField = new TextField();
            textField.style.ClearPadding();
            ApplyMargin(textField);
            textField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            textField[0].style.SetIS_Style(DocStyle.Current.MainText);
            if (!string.IsNullOrEmpty(label))
            {
                textField.label = label;
                textField[0].style.SetIS_Style(DocStyle.Current.MainText);
            }
            if(eventCallback != null)
                textField.RegisterValueChangedCallback(eventCallback);
            return  textField;
        }
        public static DropdownField NewDropdownField(string text, List<string> choice, EventCallback<ChangeEvent<string>> eventCallback = null) 
        {
            DropdownField dropField = new DropdownField();
            dropField.style.ClearPadding();
            ApplyMargin(dropField);
            dropField.style.height = DocStyle.Current.LineHeight;
            dropField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            dropField[0].style.SetIS_Style(DocStyle.Current.MainText);
            if (!string.IsNullOrEmpty(text))
            {
                dropField.label = text;
                dropField[0].style.SetIS_Style(DocStyle.Current.MainText);
            }
            if (eventCallback != null)
                dropField.RegisterValueChangedCallback(eventCallback);
            dropField.choices = choice;
            return dropField;
        }
        public static TextElement NewTextElement(string text)
        {
            TextElement textElement = new TextElement();
            textElement.text = text;
            textElement.style.ClearPadding();
            ApplyMargin(textElement);
            textElement.style.whiteSpace = WhiteSpace.Normal;
            textElement.style.SetIS_Style(DocStyle.Current.MainText);
            return textElement;
        }
        public static Label NewLabel(string text)
        {
            Label label = new Label(text);
            label.style.ClearMarginPadding();
            label.style.SetIS_Style(DocStyle.Current.LabelText);
            return label;
        }

        public static void ApplyStyle(Button button, Color color)
        {
            Color org = color;
            float h,s,v;
            Color.RGBToHSV(org, out h, out s, out v);
            v += (v > 0.5f) ? -0.1f : 0.055f;
            Color height = Color.HSVToRGB(h, s, v);
            button.style.ClearPadding();
            ApplyMargin(button);
            button.style.height = DocStyle.Current.LineHeight;
            button.style.backgroundColor = color;
            button.style.SetIS_Style(DocStyle.Current.ButtonText);
            button.RegisterCallback<PointerEnterEvent>(e =>
            {
                button.style.backgroundColor = height;
            });
            button.RegisterCallback<PointerLeaveEvent>(e =>
            {
                button.style.backgroundColor = color;
            });
        }
    }

}