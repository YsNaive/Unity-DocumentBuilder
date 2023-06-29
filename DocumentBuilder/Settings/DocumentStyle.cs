using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DocumentBuilder
{
    [System.Serializable]
    public class DocumentStyle
    {

        public Color BackgroundColor;
        public Color MainColor;
        public Color SubColor;
        public Color TextColor;

        public ISBackground ISBackground;
        public ISBorder ISBorder;
        public ISText ISText;

        public static DocumentStyle DarkTheme = new DocumentStyle
        {
            BackgroundColor = new Color(0.086f, 0.086f, 0.096f),
            MainColor = new Color(0.149f, 0.149f, 0.159f),
            SubColor = new Color(0.388f, 0.388f, 0.398f),
            TextColor = new Color(0.921f, 0.921f, 0.931f),
            ISBackground = new ISBackground() { Color = new Color(0.086f, 0.086f, 0.096f) },
            ISBorder = new ISBorder(),
            ISText = new ISText() { Color = new Color(0.921f, 0.921f, 0.931f), Align = TextAnchor.UpperLeft }
        };
    }
}
