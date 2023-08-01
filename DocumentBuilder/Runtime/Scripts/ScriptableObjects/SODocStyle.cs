using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/DocumentBuilder/new DocStyle")]
    public class SODocStyle : ScriptableObject
    {
        public int MainTextSize { get => MainText.FontSize; set => MainText.FontSize = value; }
        public int LabelTextSize { get => LabelText.FontSize; set => LabelText.FontSize = value; }
        public int ButtonTextSize { get => ButtonText.FontSize; set => ButtonText.FontSize = value; }

        public ISText MainText = new ISText();
        public ISText LabelText = new ISText();
        public ISText ButtonText = new ISText();

        public Color BackgroundColor = new Color(0,0,0,1);
        public Color SubBackgroundColor = new Color(0, 0, 0, 1);
        public Color FrontgroundColor = new Color(0, 0, 0, 1);
        public Color SubFrontgroundColor = new Color(0, 0, 0, 1);

        public Color FuncColor = new Color(.89f,.79f,.35f);
        public Color ArgsColor = new Color(.65f,.85f,.95f);
        public Color TypeColor = new Color(.35f,.7f,.65f);
        public Color PrefixColor = new Color(.4f,.56f,.82f);
        public Color StringColor = new Color(.79f,.56f,.36f);
        public Color NumberColor = new Color(.6f,.8f,.6f);
        public Color ControlColor = new Color(.84f,.45f,.61f);
        public Color CommentsColor = new Color(.4f,.6f,.35f);
        public Color CodeBackgroundColor = new Color(.08f,.08f,.09f);

        public Color SuccessColor = new Color(0, 0, 0, 1);
        public Color WarningColor = new Color(0, 0, 0, 1);
        public Color DangerColor = new Color(0, 0, 0, 1);
        public Color HintColor = new Color(0, 0, 0, 1);
        public Color SuccessTextColor = new Color(0, 0, 0, 1);
        public Color WarningTextColor = new Color(0, 0, 0, 1);
        public Color DangerTextColor = new Color(0, 0, 0, 1);
        public Color HintTextColor = new Color(0, 0, 0, 1);

        public float MarginVer = 1;
        public float MarginHor = 0;
        public float LineHeight = 18;
        public float ScrollerWidth = 14;
        public float ComponentSpace = 10;
        public float GUIScale = 1;

        public static SODocStyle Current =>DocRuntimeData.Instance.CurrentStyle;

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