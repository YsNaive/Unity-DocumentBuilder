using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
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
        public static void ApplyMarginPadding(VisualElement ve)
        {
            ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding);
        }
        public static void ApplyMargin(VisualElement ve)
        {
            ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding.Margin);
        }
        public static void ApplyPadding(VisualElement ve)
        {
            ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding.Padding);
        }
        public static DocVisual CreateDocVisual(DocComponent docComponent)
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
            VisualElement visualElement = NewEmpty();
            visualElement.style.flexDirection = FlexDirection.Row;
            return visualElement;
        }
        public static VisualElement NewHorizontalBar(params VisualElement[] elements) { return NewHorizontalBar(0.0f, elements); }
        public static VisualElement NewHorizontalBar(float space, params VisualElement[] elements)
        {
            VisualElement bar = NewEmptyHorizontal();
            bar.style.minHeight = DocStyle.Current.LineHeight;
            float width = (100f / (float)elements.Length);
            VisualElement last = null;
            foreach (var ve in elements)
            {
                if(ve != null)
                {
                    ve.style.width = Length.Percent(width-space);
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
        public static VisualElement NewContainer()
        {
            var ve = NewEmpty();
            ve.style.minHeight = DocStyle.Current.LineHeight;
            ve.style.backgroundColor = DocStyle.Current.BackgroundColor;
            return ve;
        }
        public static Button NewButton(Action onClick = null) { return NewButton("", DocStyle.Current.SubBackgroundColor, onClick); }
        public static Button NewButton(string text, Action onClick = null) { return NewButton(text, DocStyle.Current.SubBackgroundColor, onClick); }
        public static Button NewButton(Color color, Action onClick = null) { return NewButton("",color, onClick); }
        public static Button NewButton(string text, Color color, Action onClick = null)
        {
            Button button = new Button();
            ApplyButtonStyle(button, color);
            button.text = text;
            button.style.height = DocStyle.Current.MainTextSize*1.5f;
            if (onClick != null) button.clicked+= onClick;
            return button;
        }
        public static CheckButton NewCheckButton(string text, Action onClick = null) { return NewCheckButton(text, DocStyle.Current.SubBackgroundColor, DocStyle.Current.SuccessColor, DocStyle.Current.DangerColor, onClick); }
        public static CheckButton NewCheckButton(string text, Color color, Action onClick = null) { return NewCheckButton(text, color, DocStyle.Current.SuccessColor, DocStyle.Current.DangerColor, onClick); }
        public static CheckButton NewCheckButton(string text, Color color,Color confirm, Color cancel, Action onClick = null)
        {
            CheckButton button = new CheckButton();
            ApplyButtonStyle(button.MainBtn, color);
            ApplyButtonStyle(button.ConfirmButton, confirm);
            ApplyButtonStyle(button.CancelButton, cancel);
            button.text = text;
            button.style.minHeight = DocStyle.Current.LineHeight;
            if (onClick != null) button.Confirm += onClick;
            return button;
        }
        public static TextField NewTextField(string label = "", EventCallback<ChangeEvent<string>> eventCallback = null)
        {
            TextField textField = new TextField();
            textField.style.minHeight = DocStyle.Current.LineHeight;
            textField[0].style.paddingLeft = DocStyle.Current.MainTextSize / 2f;
            textField[0].style.SetIS_Style(DocStyle.Current.MainTextStyle);
            textField[0].style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            if (!string.IsNullOrEmpty(label))
            {
                textField.label = label;
                textField.labelElement.style.SetIS_Style(DocStyle.Current.MainTextStyle);
                ApplyLabelStyle(textField.labelElement);
            }
            if(eventCallback != null)
                textField.RegisterValueChangedCallback(eventCallback);
            return  textField;
        }
        public static Foldout NewFoldout(string text = "")
        {
            Foldout foldout = new Foldout();
            
            foldout.style.SetIS_Style(DocStyle.Current.MainTextStyle);
            foldout.contentContainer.style.minHeight = DocStyle.Current.LineHeight;
            foldout.contentContainer.style.paddingLeft = 15;
            foldout.text = text;
            var toggle = foldout.Q<Toggle>();
            toggle.style.ClearMarginPadding();
            toggle.style.paddingLeft = 10;
            toggle.style.backgroundColor = DocStyle.Current.BackgroundColor * 0.75f;
            toggle[0].focusable = false;
            var img = toggle[0][0];
            img.style.backgroundImage = SODocStyle.WhiteArrow;
            img.style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontgroundColor;
            img.style.ClearMarginPadding();
            img.style.marginRight = DocStyle.Current.MainTextSize/2f;
            toggle.RegisterValueChangedCallback(e =>
            {
                img.style.rotate = new Rotate(e.newValue ? 90 : 0);
            });
            foldout.value = false;
            return foldout;
        }
        /// <summary>
        /// This will create Unity Dropdown Field
        /// </summary>
        public static DropdownField NewDropdownField(string text, List<string> choice, EventCallback<ChangeEvent<string>> eventCallback = null) 
        {
            DropdownField dropField = new DropdownField();
            dropField.focusable = false;
            dropField.style.minHeight = DocStyle.Current.LineHeight;
            dropField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            dropField[0].style.SetIS_Style(DocStyle.Current.MainTextStyle);
            dropField[0].style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            dropField[0][0].style.SetIS_Style(DocStyle.Current.MainTextStyle);

            if (!string.IsNullOrEmpty(text))
            {
                dropField.label = text;
                dropField.labelElement.style.SetIS_Style(DocStyle.Current.MainTextStyle);
                ApplyLabelStyle(dropField.labelElement);
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
            textElement.style.whiteSpace = WhiteSpace.Normal;
            textElement.style.minHeight = DocStyle.Current.LineHeight;
            textElement.style.SetIS_Style(DocStyle.Current.MainTextStyle);
            return textElement;
        }
        /// <summary>
        /// This will create Custom String Field
        /// </summary>
        public static StringDropdown NewDropdown(string label, List<string> choices, Action<string> valueCallback = null)
        {
            StringDropdown dropdown = new StringDropdown(label);
            dropdown.Choices = choices;
            dropdown.OnValueChanged += valueCallback;
            dropdown.style.minHeight = DocStyle.Current.LineHeight;
            return dropdown;
        }
        public static Label NewLabel(string text)
        {
            Label label = new Label(text);
            label.style.SetIS_Style(DocStyle.Current.LabelTextStyle);
            label.style.minHeight = DocStyle.Current.LineHeight;
            return label;
        }
        private static void applyScrollBarStyle(Scroller bar, bool isHor = false)
        {
            ISBorder border = new ISBorder();
            float width = DocStyle.Current.ScrollerWidth;
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
                    ve.style.height = DocStyle.Current.ScrollerWidth;
                else
                    ve.style.width = DocStyle.Current.ScrollerWidth;
                ve.style.backgroundColor = Color.clear;
                ve.style.ClearMarginPadding();
            }
            var dragContainer = bar.Q("unity-tracker");
            dragContainer.style.backgroundColor = new Color(0,0,0,0.1f);
            dragContainer.style.SetIS_Style(border);
            var drag = bar.Q("unity-dragger");
            drag.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            if (isHor)
            {
                bar.slider.style.height = DocStyle.Current.ScrollerWidth;
                drag.style.height = Length.Percent(80);
                drag.style.top = Length.Percent(10);
            }
            else
            {
                bar.slider.style.width = DocStyle.Current.ScrollerWidth;
                drag.style.width = Length.Percent(80);
                drag.style.left = Length.Percent(10);
            }

        }
        public static ScrollView NewScrollView()
        {
            ScrollView scrollView = new ScrollView();
            scrollView.style.minHeight = DocStyle.Current.LineHeight;
            ApplyScrollViewStyle(scrollView);
            return scrollView;
        }
        public static void ApplyScrollViewStyle(ScrollView scrollView)
        {
            scrollView.style.ClearMarginPadding();
            applyScrollBarStyle(scrollView.verticalScroller);
            applyScrollBarStyle(scrollView.horizontalScroller, true);
            scrollView.verticalScroller.style.width = DocStyle.Current.ScrollerWidth;
        }
        public static void ApplyButtonStyle(Button button, Color color)
        {
            Color org = color;
            float h,s,v;
            Color.RGBToHSV(org, out h, out s, out v);
            v += (v > 0.5f) ? -0.1f : 0.055f;
            Color height = Color.HSVToRGB(h, s, v);
            button.style.SetIS_Style(DocStyle.Current.ButtonTextStyle);
            button.style.backgroundColor = color;
            button.style.SetIS_Style(DocStyle.Current.ButtonTextStyle);
            button.RegisterCallback<PointerEnterEvent>(e =>
            {
                button.style.backgroundColor = height;
            });
            button.RegisterCallback<PointerLeaveEvent>(e =>
            {
                button.style.backgroundColor = color;
            });
            button.style.minHeight = DocStyle.Current.LineHeight;
        }
        public static void ApplyLabelStyle(TextElement label)
        {
            label.style.minWidth = 0;
            label.style.width = DocStyle.Current.LabelWidth;
        }
        #endregion
    }

}