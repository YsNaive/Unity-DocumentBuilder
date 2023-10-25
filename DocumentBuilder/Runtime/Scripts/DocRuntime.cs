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

        public static List<Type> FindAllTypesWhere(Func<Type, bool> where)
        {
            List<Type> ret = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if(where(type))
                        ret.Add(type);
                }
            }
            return ret;
        }
        public static void Reload()
        {
            VisualID_Dict.Clear();
            foreach (var type in FindAllTypesWhere(t => { return (t.IsSubclassOf(typeof(DocVisual)) && !t.IsAbstract); }))
                VisualID_Dict.Add(((DocVisual)Activator.CreateInstance(type)).VisualID, type);
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
            button.style.minHeight = DocStyle.Current.LineHeight;
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
        public static Foldout NewFoldout(string text = "")
        {
            return new DSFoldout(text);
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


        /// <summary>
        /// This will create Custom String Field
        /// </summary>
        public static DSDropdown NewDropdown(string label, List<string> choices, Action<string> valueCallback = null)
        {
            DSDropdown dropdown = new DSDropdown(label);
            dropdown.choices = choices;
            dropdown.RegisterValueChangedCallback(evt => { valueCallback?.Invoke(evt.newValue); });
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

        public static void ApplyButtonStyle(Button button, Color color) { ApplyButtonStyle(button, color, Color.clear); }
        public static void ApplyButtonStyle(Button button, Color color, Color hoverColor)
        {
            button.style.ClearMarginPadding();
            if(hoverColor == Color.clear)
            {
                float h, s, v;
                Color.RGBToHSV(color, out h, out s, out v);
                v += (v > 0.5f) ? -0.1f : 0.055f;
                hoverColor = Color.HSVToRGB(h, s, v);
            }
            button.style.backgroundColor = color;
            button.style.SetIS_Style(DocStyle.Current.ButtonTextStyle);
            button.RegisterCallback<PointerEnterEvent>(e =>
            {
                button.style.backgroundColor = hoverColor;
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