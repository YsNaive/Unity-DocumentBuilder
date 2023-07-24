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
            MainText = new ISText() { Color = new Color(0.85f, 0.85f, 0.85f), FontSize = 14 },
            LabelText = new ISText() { Color = new Color(0.9f, 0.9f, 0.9f), FontSize = 20 },
            FuncColor = new Color(1f, 0.9f, 0.35f),
            ArgsColor = new Color(0.65f, 0.85f, 0.95f),
            TypeColor = new Color(0.35f, 0.70f, 0.65f),
        };
        public static DocStyle Current => DocCache.Get().CurrentStyle;
    }
}