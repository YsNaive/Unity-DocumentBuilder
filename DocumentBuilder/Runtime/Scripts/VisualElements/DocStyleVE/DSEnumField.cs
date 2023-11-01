using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;

namespace NaiveAPI.DocumentBuilder
{
    public class DSEnumField : VisualElement,INotifyValueChanged<Enum>
    {
        List<string> choices;

        public Enum value
        {
            get => m_value;
            set
            {
                if (choices == null)
                {
                    Debug.LogWarning($"DSDropdown: choices list not exist");
                    return;
                }
                if (panel != null)
                {
                    using (ChangeEvent<Enum> evt = ChangeEvent<Enum>.GetPooled(m_value, value))
                    {
                        evt.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(evt);
                    }
                }
                else
                {
                    SetValueWithoutNotify(value);
                    using var evt = ChangeEvent<Enum>.GetPooled(m_value, value);
                }
            }
        }
        Enum m_value;

        public DSTextElement labelElement => m_labelElement;
        DSTextElement m_labelElement, m_fieldElement;
        PopupElement m_popupElement;
        Type enumType;

        public DSEnumField(string label,Enum initValue , EventCallback<ChangeEvent<Enum>> valueChange = null)
        {
            enumType = initValue.GetType();
            choices = new(Enum.GetNames(enumType));
            var container = new DSHorizontal();
            m_fieldElement = new DSTextElement();
            m_fieldElement.style.whiteSpace = WhiteSpace.NoWrap;
            m_fieldElement.style.flexGrow = 1f;
            m_fieldElement.style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            m_fieldElement.style.unityBackgroundImageTintColor = DocStyle.Current.SubBackgroundColor;
            m_labelElement = new DSTextElement();
            m_labelElement.style.whiteSpace = WhiteSpace.NoWrap;
            if (label != "")
            {
                m_labelElement.text = label;
                m_labelElement.style.width = DocStyle.Current.LabelWidth;
            }
            container.Add(m_labelElement);
            container.Add(m_labelElement);
            container.Add(m_fieldElement);
            Add(container);

            var arrow = new VisualElement();
            arrow.style.SetIS_Style(DocStyle.Current.ArrowIcon);
            arrow.style.marginLeft = StyleKeyword.Auto;
            arrow.style.marginRight = 0;
            arrow.style.rotate = new Rotate(90);
            m_fieldElement.Add(arrow);

            m_popupElement = DSStringMenu.CreatePopupMenu(choices, str =>
            {
                value = (Enum)Enum.Parse(enumType, str);
            });
            m_fieldElement.text = initValue.ToString();
            m_fieldElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                m_popupElement.Open(m_fieldElement);
                m_popupElement.MoveBelow(m_fieldElement);
            });

            if (valueChange !=null)
                this.RegisterValueChangedCallback(valueChange);
        }

        public void SetValueWithoutNotify(Enum newValue)
        {
            m_value = newValue;
            ((INotifyValueChanged<string>)m_fieldElement).SetValueWithoutNotify(newValue.ToString());
        }
    }
    public class DSEnumField<EType> : VisualElement, INotifyValueChanged<EType>
        where EType : struct, IConvertible
    {
        static List<string> choices = new(Enum.GetNames(typeof(EType)));

        public EType value
        {
            get => m_value;
            set
            {
                if (choices == null)
                {
                    Debug.LogWarning($"DSDropdown: choices list not exist");
                    return;
                }
                if (panel != null)
                {
                    using (ChangeEvent<EType> evt = ChangeEvent<EType>.GetPooled(m_value, value))
                    {
                        evt.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(evt);
                    }
                }
                else
                {
                    SetValueWithoutNotify(value);
                    using var evt = ChangeEvent<EType>.GetPooled(m_value, value);
                }
            }
        }
        EType m_value;

        public DSTextElement labelElement => m_labelElement;
        DSTextElement m_labelElement, m_fieldElement;
        PopupElement m_popupElement;

        public DSEnumField(string label,EType initValue, EventCallback<ChangeEvent<EType>> valueChange = null)
        {
            if (!typeof(EType).IsEnum) throw new ArgumentException("EType must be an enum type");

            var container = new DSHorizontal();
            m_fieldElement = new DSTextElement();
            m_fieldElement.style.flexGrow = 1f;
            m_fieldElement.style.whiteSpace = WhiteSpace.NoWrap;
            m_fieldElement.style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            m_fieldElement.style.unityBackgroundImageTintColor = DocStyle.Current.SubBackgroundColor;
            m_labelElement = new DSTextElement();
            m_labelElement.style.whiteSpace = WhiteSpace.NoWrap;
            if (label != "")
            {
                m_labelElement.text = label;
                m_labelElement.style.width = DocStyle.Current.LabelWidth;
            }
            container.Add(m_labelElement);
            container.Add(m_fieldElement);
            Add(container);

            var arrow = new VisualElement();
            arrow.style.SetIS_Style(DocStyle.Current.ArrowIcon);
            arrow.style.marginLeft = StyleKeyword.Auto;
            arrow.style.marginRight = 0;
            arrow.style.rotate = new Rotate(90);
            m_fieldElement.Add(arrow);

            m_popupElement = DSStringMenu.CreatePopupMenu(choices, str =>
            {
                value = (EType)Enum.Parse(typeof(EType), str);
            });
            m_fieldElement.text = initValue.ToString();
            m_fieldElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                m_popupElement.Open(m_fieldElement);
                m_popupElement.MoveBelow(m_fieldElement);
            });

            if (valueChange !=null)
                this.RegisterValueChangedCallback(valueChange);
        }

        public void SetValueWithoutNotify(EType newValue)
        {
            m_value = newValue;
            ((INotifyValueChanged<string>)m_fieldElement).SetValueWithoutNotify(newValue.ToString());
        }
    }

}