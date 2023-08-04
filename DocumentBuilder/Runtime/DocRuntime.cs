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

        #region VisualElement
        public static void ApplyMargin(VisualElement ve)
        {
            ve.style.marginTop = SODocStyle.Current.MarginVer;
            ve.style.marginLeft = SODocStyle.Current.MarginHor;
            ve.style.marginRight = SODocStyle.Current.MarginHor;
            ve.style.marginBottom = SODocStyle.Current.MarginVer;
        }
        public static DocVisual CreateVisual(DocComponent docComponent)
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
        public static VisualElement NewHorizontalBar(params VisualElement[] elements) { return NewHorizontalBar(0.0f, elements); }
        public static VisualElement NewHorizontalBar(float space, params VisualElement[] elements)
        {
            VisualElement bar = NewEmptyHorizontal();
            float width = (99.9f - space* elements.Length) / elements.Length;
            VisualElement last = null;
            foreach (var ve in elements)
            {
                if(ve != null)
                {
                    ve.style.width = Length.Percent(width);
                    ve.style.marginRight = Length.Percent(space/2f);
                    ve.style.marginLeft = Length.Percent(space/2f);
                    last = ve;
                    bar.Add(ve);
                }
                else
                {
                    if (last == null) continue;
                    last.style.width = Length.Percent(last.style.width.value.value + width);
                }
            }
            return bar;
        }
        public static Button NewButton(Action onClick = null) { return NewButton("", SODocStyle.Current.SubBackgroundColor, onClick); }
        public static Button NewButton(string text, Action onClick = null) { return NewButton(text, SODocStyle.Current.SubBackgroundColor, onClick); }
        public static Button NewButton(Color color, Action onClick = null) { return NewButton("",color, onClick); }
        public static Button NewButton(string text, Color color, Action onClick = null)
        {
            Button button = new Button();
            button.style.height = SODocStyle.Current.LineHeight;
            ApplyStyle(button, color);
            button.style.paddingLeft = 4;
            button.style.paddingRight = 4;
            button.text = text;
            button.style.height = SODocStyle.Current.MainTextSize*1.5f;
            if (onClick != null) button.clicked+= onClick;
            return button;
        }
        public static CheckButton NewCheckButton(string text, Action onClick = null) { return NewCheckButton(text, SODocStyle.Current.SubBackgroundColor, SODocStyle.Current.SuccessColor, SODocStyle.Current.DangerColor, onClick); }
        public static CheckButton NewCheckButton(string text, Color color, Action onClick = null) { return NewCheckButton(text, color, SODocStyle.Current.SuccessColor, SODocStyle.Current.DangerColor, onClick); }
        public static CheckButton NewCheckButton(string text, Color color,Color confirm, Color cancel, Action onClick = null)
        {
            CheckButton button = new CheckButton();
            button.style.height = SODocStyle.Current.LineHeight;
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
            textField[0].style.backgroundColor = SODocStyle.Current.SubBackgroundColor;
            textField[0].style.SetIS_Style(SODocStyle.Current.MainText);
            if (!string.IsNullOrEmpty(label))
            {
                textField.label = label;
                textField[0].style.SetIS_Style(SODocStyle.Current.MainText);
            }
            if(eventCallback != null)
                textField.RegisterValueChangedCallback(eventCallback);
            return  textField;
        }
        public static DropdownField NewDropdownField(string text, List<string> choice, EventCallback<ChangeEvent<string>> eventCallback = null) 
        {
            DropdownField dropField = new DropdownField();
            dropField.style.ClearPadding();
            dropField.focusable = false;
            ApplyMargin(dropField);
            dropField.style.height = SODocStyle.Current.LineHeight;
            dropField[0].style.backgroundColor = SODocStyle.Current.SubBackgroundColor;
            dropField[0].style.SetIS_Style(SODocStyle.Current.MainText);
            if (!string.IsNullOrEmpty(text))
            {
                dropField.label = text;
                dropField[0].style.SetIS_Style(SODocStyle.Current.MainText);
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
            textElement.style.SetIS_Style(SODocStyle.Current.MainText);
            return textElement;
        }
        public static Label NewLabel(string text)
        {
            Label label = new Label(text);
            label.style.ClearMarginPadding();
            label.style.SetIS_Style(SODocStyle.Current.LabelText);
            return label;
        }
        private static void applyScrollBarStyle(Scroller bar, bool isHor = false)
        {
            ISBorder border = new ISBorder();
            float width = SODocStyle.Current.ScrollerWidth;
            bar.slider.style.marginTop = 0;
            bar.slider.style.marginBottom = 0;
            bar.style.ClearMarginPadding();
            bar.style.borderLeftWidth = 0;
            bar.style.borderTopWidth = 0;
            bar.highButton.style.display = DisplayStyle.None;
            bar.lowButton.style.display = DisplayStyle.None;
            bar.contentContainer.style.backgroundColor = Color.clear;
            foreach (var ve in bar.slider.contentContainer.Children())
            {
                if (isHor)
                    ve.style.height = SODocStyle.Current.ScrollerWidth;
                else
                    ve.style.width = SODocStyle.Current.ScrollerWidth;
                ve.style.backgroundColor = Color.clear;
                ve.style.ClearMarginPadding();
            }
            var dragContainer = bar.Q("unity-tracker");
            dragContainer.style.backgroundColor = new Color(0,0,0,0.1f);
            dragContainer.style.SetIS_Style(border);
            var drag = bar.Q("unity-dragger");
            drag.style.backgroundColor = SODocStyle.Current.SubBackgroundColor;
            if (isHor)
            {
                bar.slider.style.height = SODocStyle.Current.ScrollerWidth;
                drag.style.height = Length.Percent(80);
                drag.style.top = Length.Percent(10);
            }
            else
            {
                bar.slider.style.width = SODocStyle.Current.ScrollerWidth;
                drag.style.width = Length.Percent(80);
                drag.style.left = Length.Percent(10);
            }

        }
        public static ScrollView NewScrollView()
        {
            
            ScrollView scrollView = new ScrollView();
            ApplyStyle(scrollView);
            return scrollView;
        }
        public static void ApplyStyle(ScrollView scrollView)
        {
            scrollView.style.ClearMarginPadding();
            applyScrollBarStyle(scrollView.verticalScroller);
            applyScrollBarStyle(scrollView.horizontalScroller, true);
            scrollView.verticalScroller.style.width = SODocStyle.Current.ScrollerWidth;
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
            button.style.height = SODocStyle.Current.LineHeight;
            button.style.backgroundColor = color;
            button.style.SetIS_Style(SODocStyle.Current.ButtonText);
            button.RegisterCallback<PointerEnterEvent>(e =>
            {
                button.style.backgroundColor = height;
            });
            button.RegisterCallback<PointerLeaveEvent>(e =>
            {
                button.style.backgroundColor = color;
            });
        }
        #endregion
    }

}