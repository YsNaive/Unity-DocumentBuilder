using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocStyle
    {
        static DocStyle()
        {
            Application.quitting += () =>
            {
                Current = DocRuntimeData.Instance.CurrentStyle.Get(false);
            };
        }
        public static event Action<DocStyle> OnStyleChanged;
        public static DocStyle Current
        {
            get
            {
                if (m_current == null)
                    m_current = DocRuntimeData.Instance.CurrentStyle.Get();
                return m_current;
            }
            set
            {
                if (m_current == value) return;
                m_current = value;
                OnStyleChanged?.Invoke(value);
            }
        }
        private static DocStyle m_current;
        public int MainTextSize { get => MainText.FontSize; set => MainText.FontSize = value; }
        public int LabelTextSize { get => LabelText.FontSize; set => LabelText.FontSize = value; }
        public int ButtonTextSize { get => ButtonText.FontSize; set => ButtonText.FontSize = value; }
        public ISText MainText { get => MainTextStyle.Text; set => MainTextStyle.Text = value; }
        public ISText LabelText { get => LabelTextStyle.Text; set => LabelTextStyle.Text = value; }
        public ISText ButtonText { get => ButtonTextStyle.Text; set => ButtonTextStyle.Text = value; }
        
        public ISStyle MainTextStyle   = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.Text | ISStyleFlag.MarginPadding);
        public ISStyle LabelTextStyle  = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.Text | ISStyleFlag.MarginPadding);
        public ISStyle ButtonTextStyle = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.Text | ISStyleFlag.Padding);
        public ISStyle InputFieldStyle = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.MarginPadding | ISStyleFlag.Background);
        public ISStyle ElementMarginPadding = new ISStyle(ISStyleFlag.MarginPadding);

        public Color BackgroundColor = new Color(0, 0, 0, 1);
        public Color SubBackgroundColor = new Color(0, 0, 0, 1);
        public Color FrontgroundColor = new Color(0, 0, 0, 1);
        public Color SubFrontgroundColor = new Color(0, 0, 0, 1);

        public Color FuncColor = new Color(.89f, .79f, .35f);
        public Color ArgsColor = new Color(.65f, .85f, .95f);
        public Color TypeColor = new Color(.35f, .7f, .65f);
        public Color PrefixColor = new Color(.4f, .56f, .82f);
        public Color StringColor = new Color(.79f, .56f, .36f);
        public Color NumberColor = new Color(.6f, .8f, .6f);
        public Color ControlColor = new Color(.84f, .45f, .61f);
        public Color CommentsColor = new Color(.4f, .6f, .35f);
        public Color CodeBackgroundColor = new Color(.08f, .08f, .09f);

        public Color SuccessColor = new Color(0, 0, 0, 1);
        public Color WarningColor = new Color(0, 0, 0, 1);
        public Color DangerColor = new Color(0, 0, 0, 1);
        public Color HintColor = new Color(0, 0, 0, 1);
        public Color SuccessTextColor = new Color(0, 0, 0, 1);
        public Color WarningTextColor = new Color(0, 0, 0, 1);
        public Color DangerTextColor = new Color(0, 0, 0, 1);
        public Color HintTextColor = new Color(0, 0, 0, 1);

        public ISLength LineHeight = new ISLength { Unit = LengthUnit.Pixel, Value = 20 };
        public ISLength LabelWidth = new ISLength { Unit = LengthUnit.Pixel, Value = 200 };
        public Length ContentWidth(VisualElement ve)
        {
            if (LabelWidth.Unit == LengthUnit.Percent)
                return Length.Percent(100 - LabelWidth.Value);
            if (ve.parent == null) return new Length(0);
            return new Length(ve.parent.resolvedStyle.width - LabelWidth.Value);
        }


        public float MarginVer => ElementMarginPadding.Margin.Left.Value.Value;
        public float MarginHor => ElementMarginPadding.Margin.Top.Value.Value;
        public float PaddingVer => ElementMarginPadding.Padding.Left.Value.Value;
        public float PaddingHor => ElementMarginPadding.Padding.Top.Value.Value;
        public float ScrollerWidth = 14;
        public float ComponentSpace = 10;
        public float GUIScale = 1;


        public DocStyle Copy()
        {
            DocStyle docStyle = new DocStyle();
            docStyle.MainTextStyle = this.MainTextStyle.Copy();
            docStyle.LabelTextStyle = this.LabelTextStyle.Copy();
            docStyle.ButtonTextStyle = this.ButtonTextStyle.Copy();
            docStyle.InputFieldStyle = this.InputFieldStyle.Copy();
            docStyle.BackgroundColor = this.BackgroundColor;
            docStyle.SubBackgroundColor = this.SubBackgroundColor;
            docStyle.FrontgroundColor = this.FrontgroundColor;
            docStyle.SubFrontgroundColor = this.SubFrontgroundColor;

            docStyle.FuncColor = this.FuncColor;
            docStyle.ArgsColor = this.ArgsColor;
            docStyle.TypeColor = this.TypeColor;
            docStyle.PrefixColor = this.PrefixColor;
            docStyle.StringColor = this.StringColor;  
            docStyle.NumberColor = this.NumberColor;  
            docStyle.ControlColor = this.ControlColor;  
            docStyle.CommentsColor = this.CommentsColor;  
            docStyle.CodeBackgroundColor = this.CodeBackgroundColor;  

            docStyle.SuccessColor = this.SuccessColor;  
            docStyle.WarningColor = this.WarningColor;  
            docStyle.DangerColor = this.DangerColor;  
            docStyle.HintColor = this.HintColor;  
            docStyle.SuccessTextColor = this.SuccessTextColor;  
            docStyle.WarningTextColor = this.WarningTextColor;  
            docStyle.DangerTextColor = this.DangerTextColor;  
            docStyle.HintTextColor = this.HintTextColor;

            docStyle.LabelWidth = this.LabelWidth;
            docStyle.LineHeight = this.LineHeight;

            docStyle.ElementMarginPadding = this.ElementMarginPadding.Copy();
            
            docStyle.ScrollerWidth = this.ScrollerWidth;
            docStyle.ComponentSpace = this.ComponentSpace;
            docStyle.GUIScale = this.GUIScale;

            return docStyle;
        }
    }

}