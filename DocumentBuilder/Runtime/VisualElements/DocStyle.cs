using NaiveAPI_UI;
using System;
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

        private static Texture2D m_whiteArrow;
        public static Texture2D WhiteArrow
        {
            get
            {
                if (m_whiteArrow == null)
                {
                    m_whiteArrow = new Texture2D(1, 1);
                    m_whiteArrow.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAZxJREFUeJztm7GOgkAURY+b/VVIqKxs/AAbKioS+FitHkFiNqvDvLmD71QM0XHu8Q4hESEIgiAIvpZT6gRN09y35+Z5Tp7Xi6SFvgpv1CLh40Wuw1+v1+X87XZ7ep26iJ/UCdbhAS6Xy9P4r5YokCzgFefzma7rlnHTNHdVEVkEGGsJoNmGrAIApmlimqZlrNaG7AKMtQTQaYObANBsg6sAQ6kNRQSAThuKCTBKt6G4AHjdBq/PlhBglNgSUgLAf0vICQDfC6SkAMOjDdICIH8b5AUYudpQjQDI04aqBBh7tqFKAbDfzVO1AozULVG9AEhrwyEEGNtrw384lIBPOJSAtm3ffs9vhnW4sw3+zo8x1TcgJTxU3IDU4EaVDdgrPFTWgD2DG9U0IEd4qKABuYIb0g3IHR5EG+AR3JBrgGd4EGvAOrzXozUSAry/9TXFt0DJ8FCwAaWDG0UaoBIenBugFNxwa4BieHBogGpwI6uAcRyXY7XgRhYBwzA8jVXDww7XgO3D0X3fL8fzPJ+Uw0M8Lh9/mAiCIAiCL+YBAFvXYZRLUdUAAAAASUVORK5CYII="));
                    m_whiteArrow.Apply();
                }
                return m_whiteArrow;

            }
        }
    }
}