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

        public int MainTextSize { get => MainText.FontSize; set => MainText.FontSize = value; }
        public int LabelTextSize { get => LabelText.FontSize; set => LabelText.FontSize = value; }
        public int ButtonTextSize { get => ButtonText.FontSize; set => ButtonText.FontSize = value; }

        public ISText MainText = new ISText();
        public ISText LabelText = new ISText();
        public ISText ButtonText = new ISText();

        public Color FuncColor;
        public Color ArgsColor;
        public Color TypeColor;
        public Color PrefixColor;
        public Color StringColor;
        public Color NumberColor;
        public Color ControlColor;
        public Color CommentsColor;
        public Color CodeBackgroundColor;

        public Color SuccessColor;
        public Color WarningColor;
        public Color DangerColor;
        public Color HintColor;
        public Color SuccessTextColor;
        public Color WarningTextColor;
        public Color DangerTextColor;
        public Color HintTextColor;

        public float MarginVer;
        public float MarginHor;
        public float LineHeight;
        public float ScrollerWidth;
        public float ComponentSpace;
        public float GUIScale = 1;


        public static DocStyle Dark = new DocStyle()
        { 
            BackgroundColor = new Color(0.15f, 0.15f, 0.16f),
            SubBackgroundColor = new Color(0.25f, 0.25f, 0.28f),
            FrontGroundColor = new Color(0.65f, 0.65f, 0.65f),
            SubFrontGroundColor = new Color(0.55f, 0.55f, 0.55f),
            MainText = new ISText() { Color = new Color(0.85f, 0.85f, 0.85f), FontSize = 12, Align = TextAnchor.MiddleLeft },
            LabelText = new ISText() { Color = new Color(0.9f, 0.9f, 0.9f), FontSize = 18, Align = TextAnchor.MiddleLeft },
            ButtonText = new ISText() { Color = new Color(0.85f, 0.85f, 0.85f), FontSize = 12, Align = TextAnchor.MiddleCenter },
            FuncColor = new Color(0.89f, 0.79f, 0.35f),
            ArgsColor = new Color(0.65f, 0.85f, 0.95f),
            TypeColor = new Color(0.35f, 0.70f, 0.65f),
            SuccessColor = new Color(0.2f, 0.3f, 0.2f),
            WarningColor = new Color(0.3f, 0.3f, 0.2f),
            DangerColor = new Color(0.3f, 0.2f, 0.2f),
            HintColor = new Color(0.2f, 0.25f, 0.3f),
            SuccessTextColor = new Color(0.7f, 0.9f, 0.7f),
            WarningTextColor = new Color(0.9f, 0.9f, 0.7f),
            DangerTextColor = new Color(0.9f, 0.7f, 0.7f),
            HintTextColor = new Color(0.8f, 0.9f, 1f),
            PrefixColor = new Color(0.4f, 0.56f, 0.82f),
            StringColor = new Color(0.84f, 0.55f, 0.29f),
            NumberColor = new Color(0.6f, 0.8f, 0.6f),
            ControlColor = new Color(0.84f, 0.45f, 0.61f),
            CodeBackgroundColor = new Color(0.1f, 0.1f, 0.12f),
            CommentsColor = new Color(0.4f,0.6f,0.35f),
            MarginHor = 0,
            MarginVer = 1,
            LineHeight = 18,
            ScrollerWidth = 14,
            ComponentSpace = 10,
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
            copy.ButtonText = this.ButtonText.Copy();
            copy.FuncColor = this.FuncColor;
            copy.ArgsColor = this.ArgsColor;
            copy.TypeColor = this.TypeColor;
            copy.SuccessColor = this.SuccessColor;
            copy.SuccessTextColor = this.SuccessTextColor;
            copy.WarningColor = this.WarningColor;
            copy.WarningTextColor = this.WarningTextColor;
            copy.DangerColor = this.DangerColor;
            copy.DangerTextColor = this.DangerTextColor;
            copy.HintColor = this.HintColor;
            copy.HintTextColor = this.HintTextColor;
            copy.MarginHor = this.MarginHor;
            copy.MarginVer = this.MarginVer;
            copy.LineHeight = this.LineHeight;
            copy.PrefixColor = this.PrefixColor;
            copy.StringColor = this.StringColor;
            copy.NumberColor = this.NumberColor;
            copy.ControlColor = this.ControlColor;
            copy.CodeBackgroundColor = this.CodeBackgroundColor;
            copy.ComponentSpace = this.ComponentSpace;
            copy.ScrollerWidth = this.ScrollerWidth;
            copy.CommentsColor = this.CommentsColor;

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