using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSTypeField : VisualElement, INotifyValueChanged<Type>
    {
        public int MaxPopupCount = 35;

        public Type value
        {
            get => m_value;
            set
            {
                using var evt = ChangeEvent<Type>.GetPooled(m_value, value);
                evt.target = this;
                SendEvent(evt);

                m_value = value;
                ((INotifyValueChanged<string>)searchField).SetValueWithoutNotify(TypeReader.GetName(value));
            }
        }
        Type m_value;
        PopupElement popup;
        DSTextField searchField;
        public DSTypeField(string label = "")
        {
            style.minHeight = DocStyle.Current.LineHeight;
            popup = new PopupElement();
            popup.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1));
            var container = new DSScrollView();
            container.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                container.style.maxHeight = container.panel.visualTree.worldBound.height * 0.75f;
                container.style.maxWidth = container.panel.visualTree.worldBound.width * 0.75f;
            });
            popup.Add(container);
            searchField = new DSTextField(label);
            Add(searchField);
            searchField.RegisterValueChangedCallback(evt =>
            {
                container.Clear();
                if (evt.newValue == "") return;
                List<Type> matchedType = new();
                foreach (var type in TypeReader.ActiveTypes)
                {
                    string name = type.Name;
                    if (name.Contains(evt.newValue))
                    {
                        matchedType.Add(type);
                    }
                }
                int i = 1;
                foreach (var type in matchedType.OrderBy(e =>
                {
                    return e.Name.Length;
                })) 
                {
                    if (i++ > MaxPopupCount) break;
                    var text = new DSTextElement(TypeReader.GetName(type));
                    text.style.backgroundColor = DocStyle.Current.BackgroundColor;
                    text.RegisterCallback<PointerEnterEvent>(evt => { text.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
                    text.RegisterCallback<PointerLeaveEvent>(evt => { text.style.backgroundColor = DocStyle.Current.BackgroundColor; });
                    text.style.paddingLeft = DocStyle.Current.MainTextSize;
                    text.style.minHeight = DocStyle.Current.LineHeight;
                    text.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
                    text.style.borderBottomWidth = 1.5f;
                    text.style.flexShrink = 0;
                    text.RegisterCallback<PointerDownEvent>(evt => { value = type; });
                    container.Add(text);
                }
                popup.Open(this);
                popup.MoveBelow(searchField.InputFieldElement);
            });
            searchField.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (value != null)
                    searchField.SetValueWithoutNotify(TypeReader.GetName(value));
                else
                    searchField.SetValueWithoutNotify("");
                popup.Close();
            });

        }

        public void SetValueWithoutNotify(Type newValue)
        {
            m_value = newValue;
        }
    }
}
