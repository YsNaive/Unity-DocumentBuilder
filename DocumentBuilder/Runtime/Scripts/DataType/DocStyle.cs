using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocStyle
    {
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

        public ISStyle MainTextStyle = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.Text | ISStyleFlag.MarginPadding)
        {
            Margin  = new ISMargin  { Left = ISStyleLength.Pixel(2), Top = ISStyleLength.Auto, Right = ISStyleLength.Pixel(2), Bottom = ISStyleLength.Auto},
            Padding = new ISPadding { Left = ISStyleLength.Pixel(6), Top = ISStyleLength.Auto, Right = ISStyleLength.Pixel(6), Bottom = ISStyleLength.Auto},
            Text = new ISText { FontSize = 12},
        };
        public ISStyle LabelTextStyle  = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.Text | ISStyleFlag.MarginPadding)
        {
            Margin  = new ISMargin  { Left = ISStyleLength.Pixel(0), Top = ISStyleLength.Pixel(4), Right = ISStyleLength.Pixel(4), Bottom = ISStyleLength.Auto },
            Padding = new ISPadding { Left = ISStyleLength.Pixel(2), Top = ISStyleLength.Pixel(4), Right = ISStyleLength.Pixel(2), Bottom = ISStyleLength.Pixel(4) },
            Text = new ISText { FontSize = 22 },
        };
        public ISStyle ButtonTextStyle = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.Text)
        {
            Text = new ISText { FontSize = 12, Align = TextAnchor.MiddleCenter },
        };
        public ISStyle InputFieldStyle = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.Padding | ISStyleFlag.Background)
        {
            Padding = new ISPadding { Left = ISStyleLength.Pixel(6), Top = ISStyleLength.Auto, Right = ISStyleLength.Pixel(6), Bottom = ISStyleLength.Auto },
        };
        public ISStyle ElementMarginPadding = new ISStyle(ISStyleFlag.MarginPadding)
        {
            Margin  = new ISMargin  { Left = ISStyleLength.Pixel(10), Top = ISStyleLength.Pixel(2), Right = ISStyleLength.Pixel(10), Bottom = ISStyleLength.Pixel(2) },
            Padding = new ISPadding { Left = ISStyleLength.Pixel(0) , Top = ISStyleLength.Pixel(0), Right = ISStyleLength.Auto, Bottom = ISStyleLength.Auto },
        };

        public Color BackgroundColor = new Color(0, 0, 0, 1);
        public Color SubBackgroundColor = new Color(0, 0, 0, 1);
        public Color FrontgroundColor = new Color(0, 0, 0, 1);
        public Color SubFrontgroundColor = new Color(0, 0, 0, 1);

        public Color FuncColor = new Color(.89f, .79f, .35f);
        public Color ArgsColor = new Color(.65f, .85f, .95f);
        public Color TypeColor = new Color(.35f, .7f, .65f);
        public Color ValueTypeColor = new Color(.55f, .8f, .75f);
        public Color PrefixColor = new Color(.4f, .56f, .82f);
        public Color StringColor = new Color(.79f, .56f, .36f);
        public Color NumberColor = new Color(.6f, .8f, .6f);
        public Color ControlColor = new Color(.84f, .45f, .61f);
        public Color CommentsColor = new Color(.4f, .6f, .35f);
        public Color CodeBackgroundColor = new Color(.08f, .08f, .09f);
        public Color CodeTextColor = new Color(.85f, .85f, .85f);

        public Color SuccessColor = new Color(0, 0, 0, 1);
        public Color WarningColor = new Color(0, 0, 0, 1);
        public Color DangerColor = new Color(0, 0, 0, 1);
        public Color HintColor = new Color(0, 0, 0, 1);
        public Color SuccessTextColor = new Color(0, 0, 0, 1);
        public Color WarningTextColor = new Color(0, 0, 0, 1);
        public Color DangerTextColor = new Color(0, 0, 0, 1);
        public Color HintTextColor = new Color(0, 0, 0, 1);

        public ISLength LineHeight = new ISLength { Unit = LengthUnit.Pixel, Value = 20 };
        public ISLength LabelWidth = new ISLength { Unit = LengthUnit.Pixel, Value = 120 };


        public ISStyle ArrowIcon
        {
            get
            {
                var copy = IconStyle.Copy();
                copy.Background.Sprite = ArrowSprite;
                return copy;
            }
        }

        public ISStyle IconStyle = new ISStyle(ISStyleFlag.Editable | ISStyleFlag.Background | ISStyleFlag.Size);
        public Sprite ArrowSprite;
        public Sprite GearSprite;

        public float MarginVer => ElementMarginPadding.Margin.Left.Value.Value;
        public float MarginHor => ElementMarginPadding.Margin.Top.Value.Value;
        public float PaddingVer => ElementMarginPadding.Padding.Left.Value.Value;
        public float PaddingHor => ElementMarginPadding.Padding.Top.Value.Value;
        public ISLength ScrollerWidth = ISLength.Pixel(14);
        public ISLength ComponentSpace = ISLength.Pixel(10);

        private Stack<ISLength> m_ISLengthBuffer = new Stack<ISLength>();
        public void BeginLabelWidth(ISLength width)
        {
            m_ISLengthBuffer.Push(LabelWidth);
            LabelWidth = width;
        }
        public void EndLabelWidth() {
            LabelWidth = m_ISLengthBuffer.Pop();
        }

        public static void ApplyByMask(VisualElement element, DocStyleFlag flag)
        {
            if ((flag & DocStyleFlag.MainText) == DocStyleFlag.MainText)
                element.style.SetIS_Style(Current.MainText);
            if ((flag & DocStyleFlag.LabelText) == DocStyleFlag.LabelText)
                element.style.SetIS_Style(Current.LabelText);
            if ((flag & DocStyleFlag.ButtonText) == DocStyleFlag.ButtonText)
                element.style.SetIS_Style(Current.ButtonText);
            if ((flag & DocStyleFlag.InputField) == DocStyleFlag.InputField)
                element.style.SetIS_Style(Current.InputFieldStyle);
            if ((flag & DocStyleFlag.Element) == DocStyleFlag.Element)
                element.style.SetIS_Style(Current.ElementMarginPadding);
        }
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
            docStyle.ValueTypeColor = this.ValueTypeColor;
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

            docStyle.IconStyle = this.IconStyle.Copy();
            docStyle.ArrowSprite = this.ArrowSprite;
            docStyle.GearSprite = this.GearSprite;

            return docStyle;
        }
    }
}