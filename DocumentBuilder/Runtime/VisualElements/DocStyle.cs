using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocStyle
    {
        public Color BackgroundColor;
        public Color SubBackgroundColor;
        public Color FrontGroundColor;
        public Color SubFrontGroundColor;

        public int MainTextSize;
        public int LabelTextSize;

        public ISText MainText;
        public ISText LabelText;

        public Color FuncColor;
        public Color ArgsColor;
        public Color TypeColor;

        public Color SuccessColor;
        public Color WarningColor;
        public Color DangerColor;
        public Color HintColor;

        public float GUIScale = 1;
        public float TextScale
        {
            get { return m_textScale; }
            set
            {
                m_textScale = value;
                MainText.FontSize = (int)(MainTextSize * m_textScale);
                LabelText.FontSize = (int)(LabelTextSize * m_textScale);
            }
        }

        private float m_textScale = 1;

        public static DocStyle Dark = new DocStyle()
        {
            BackgroundColor = new Color(0.15f, 0.15f, 0.16f),
            SubBackgroundColor = new Color(0.2f, 0.2f, 0.22f),
            FrontGroundColor = new Color(0.65f, 0.65f, 0.65f),
            SubFrontGroundColor = new Color(0.55f, 0.55f, 0.55f),
            MainTextSize = 14,
            LabelTextSize = 20,
            MainText = new ISText() { Color = new Color(0.85f, 0.85f, 0.85f), FontSize = 12 },
            LabelText = new ISText() { Color = new Color(0.9f, 0.9f, 0.9f), FontSize = 18 },
            FuncColor = new Color(1f, 0.9f, 0.35f),
            ArgsColor = new Color(0.65f, 0.85f, 0.95f),
            TypeColor = new Color(0.35f, 0.70f, 0.65f),
            SuccessColor = new Color(0.2f, 0.3f, 0.2f),
            WarningColor = new Color(0.3f, 0.3f, 0.2f),
            DangerColor  = new Color(0.3f, 0.2f, 0.2f),
            HintColor    = new Color(0.2f, 0.2f, 0.3f),
        };

        public static DocStyle Current => DocCache.Get().CurrentStyle;
        public DocStyle Copy()
        {
            DocStyle copy = new DocStyle();

            copy.BackgroundColor = this.BackgroundColor;
            copy.SubBackgroundColor = this.SubBackgroundColor;
            copy.FrontGroundColor = this.FrontGroundColor;
            copy.SubFrontGroundColor = this.SubFrontGroundColor;
            copy.MainTextSize = this.MainTextSize;
            copy.LabelTextSize = this.LabelTextSize;
            copy.MainText = this.MainText.Copy();
            copy.LabelText = this.LabelText.Copy();
            copy.FuncColor = this.FuncColor;
            copy.ArgsColor = this.ArgsColor;
            copy.TypeColor = this.TypeColor;
            copy.SuccessColor = this.SuccessColor;
            copy.WarningColor = this.WarningColor;
            copy.DangerColor = this.DangerColor;
            copy.HintColor = this.HintColor;

            return copy;
        }
    }
}