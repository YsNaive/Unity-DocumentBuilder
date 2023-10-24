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
        public int MaxPopupCount = 10;

        public Type value
        {
            get => m_value;
            set
            {
                m_value = value;
                searchField.value = DocumentBuilderParser.CalGenericTypeName(value);
            }
        }
        Type m_value;
        PopupElement popup;
        DSTextField searchField;
        public DSTypeField(string label = "")
        {
            popup = new PopupElement();
            searchField = new DSTextField(label);
            Add(searchField);
            searchField.RegisterValueChangedCallback(evt =>
            {
                popup.Clear();
                if (evt.newValue == "") return;
                List<Type> matchedType = new();
                foreach (var asmdef in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in asmdef.GetTypes())
                    {
                        if (type.Name.Contains(evt.newValue))
                        {
                            matchedType.Add(type);
                        }
                    }
                }
                int i = 1;
                foreach (var type in matchedType.OrderBy(e =>
                {
                    float w = e.Name.Length;
                    int i = 0;
                    while(i<e.Name.Length && i < searchField.value.Length)
                    {
                        if (e.Name[i] == searchField.value[i])
                            w--;
                        else
                            break;
                        i++;
                    }
                    return  w;
                })) 
                {
                    if (i++ > MaxPopupCount) break;
                    var choice = DocumentBuilderParser.CalGenericTypeName(type);
                    var text = new DSTextElement(choice);
                    text.text = choice;
                    text.style.backgroundColor = DocStyle.Current.BackgroundColor;
                    text.RegisterCallback<PointerEnterEvent>(evt => { text.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
                    text.RegisterCallback<PointerLeaveEvent>(evt => { text.style.backgroundColor = DocStyle.Current.BackgroundColor; });
                    text.style.paddingLeft = DocStyle.Current.MainTextSize;
                    text.style.height = DocStyle.Current.LineHeight;
                    text.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
                    text.style.borderBottomWidth = 1.5f;
                    text.RegisterCallback<PointerDownEvent>(evt => { value = type; });
                    popup.Add(text);
                }
                popup.Open(this);
                popup.MoveBelow(searchField.InputFieldElement);
            });
            searchField.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (value != null)
                    searchField.SetValueWithoutNotify(DocumentBuilderParser.CalGenericTypeName(value));
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
