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

        public ISText MainText;
        public ISText LabelText;

        public Color FuncColor;
        public Color ArgsColor;
        public Color TypeColor;

        public float GUIScale = 1;
        public float TextScale = 1;

        public static DocStyle Dark = new DocStyle()
        {
            BackgroundColor = new Color(0.15f, 0.15f, 0.16f),
            SubBackgroundColor = new Color(0.2f, 0.2f, 0.22f),
            FrontGroundColor = new Color(0.65f,0.65f,0.65f),
            SubFrontGroundColor = new Color(0.55f, 0.55f, 0.55f),
            MainText = new ISText(){Color = new Color(0.85f, 0.85f, 0.85f), FontSize = 14},
            LabelText = new ISText(){Color = new Color(0.9f, 0.9f, 0.9f),FontSize = 20},
            FuncColor = new Color(1f, 0.9f, 0.35f),
            ArgsColor = new Color(0.65f, 0.85f, 0.95f),
            TypeColor = new Color(0.35f, 0.70f, 0.65f),
        };
        public static DocStyle Current = Dark;
    }
}
