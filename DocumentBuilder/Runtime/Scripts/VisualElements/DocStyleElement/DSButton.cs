using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI_UI;

namespace NaiveAPI.DocumentBuilder
{
    public class DSButton : Button
    {
        public Color BackgroundColor
        {
            get => m_BackgroundColor;
            set
            {
                style.backgroundColor = value;
                m_BackgroundColor = value;
            }
        }
        public Color HoverColor
        {
            get => m_HoverColor;
            set => m_HoverColor = value;
        }
        Color m_BackgroundColor;
        Color m_HoverColor;

        public DSButton(Action clicked = null) : this("", DocStyle.Current.SubBackgroundColor, Color.clear, clicked) { }
        public DSButton(string text, Action clicked = null) : this(text, DocStyle.Current.SubBackgroundColor, Color.clear, clicked) { }
        public DSButton(string text, Color color) : this(text, color, Color.clear, null) { }
        public DSButton(string text, Color color, Action clicked = null) : this(text, color, Color.clear, clicked) { }
        public DSButton(string text, Color color, Color hoverColor, Action clicked = null)
        {
            if(clicked != null)
                this.clicked += clicked;

            if(m_HoverColor == Color.clear)
                SetColor(color);
            else
            {
                m_BackgroundColor = color;
                m_HoverColor = hoverColor;
            }

            this.text = text;   

            style.backgroundColor = m_BackgroundColor;
            style.SetIS_Style(DocStyle.Current.ButtonTextStyle);
            RegisterCallback<PointerEnterEvent>(e => { style.backgroundColor = m_HoverColor; });
            RegisterCallback<PointerLeaveEvent>(e => { style.backgroundColor = m_BackgroundColor; });
            style.minHeight = DocStyle.Current.LineHeight;
        }

        public void SetColor(Color color)
        {
            m_BackgroundColor = color;
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            v += (v > 0.5f) ? -0.1f : 0.055f;
            m_HoverColor = Color.HSVToRGB(h, s, v);
        }
    }
}