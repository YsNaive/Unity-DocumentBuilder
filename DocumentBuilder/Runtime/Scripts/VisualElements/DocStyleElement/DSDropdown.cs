using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSDropdown : VisualElement, INotifyValueChanged<string>
    {
        public string value
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
                    using (ChangeEvent<string> evt = ChangeEvent<string>.GetPooled(m_value, value))
                    {
                        evt.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(evt);
                    }
                }
                else
                {
                    SetValueWithoutNotify(value);
                    using var evt = ChangeEvent<string>.GetPooled(m_value, value);
                }
            }
        }
        public int index
        {
            get => m_choices.IndexOf(value);
            set {
                if(value >=0 && value < m_choices.Count)
                {
                    this.value = m_choices[value];
                }
                else
                {
                    this.value = "";
                }
            }
        }
        string m_value;
        public void SetValueWithoutNotify(string newValue)
        {
            m_value = newValue;
            ((INotifyValueChanged<string>)m_fieldElement).SetValueWithoutNotify(newValue.Substring(newValue.IndexOf('/') + 1));
        }

        public DSTextElement labelElement => m_labelElement;
        DSTextElement m_labelElement, m_fieldElement;
        public List<string> choices
        {
            get => m_choices;
            set
            {
                if (value == m_choices) return;
                popupMenu = DSStringMenu.CreatePopupMenu(value, (str) => { this.value = str; });
                m_choices = value;
            }
        }
        List<string> m_choices;
        PopupElement popupMenu;
        public DSDropdown()
        {
            var container = new DSHorizontal();
            m_fieldElement = new DSTextElement();
            m_labelElement = new DSTextElement();
            container.Add(m_labelElement);
            container.Add(m_fieldElement);
            Add(container);

            m_fieldElement.style.flexGrow = 1f;
            m_fieldElement.style.whiteSpace = WhiteSpace.NoWrap;
            m_fieldElement.style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            m_fieldElement.style.unityBackgroundImageTintColor = DocStyle.Current.SubBackgroundColor;
            var arrow = new VisualElement();
            arrow.style.SetIS_Style(DocStyle.Current.ArrowIcon);
            arrow.style.marginLeft = StyleKeyword.Auto;
            arrow.style.marginRight = 0;
            arrow.style.rotate = new Rotate(90);
            m_fieldElement.Add(arrow);

            m_fieldElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                popupMenu.Open(this);
                var pos = new Vector2(0, m_fieldElement.localBound.height);
                pos = m_fieldElement.LocalToWorld(pos);
                pos = popupMenu.CoverMask.WorldToLocal(pos);
                popupMenu.style.left = pos.x;
                popupMenu.style.top = pos.y;
            });
        }
        public DSDropdown(string label) : this()
        {
            m_labelElement.text = label;
            m_labelElement.style.width = DocStyle.Current.LabelWidth;
        }
    }
}