using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISText
    {
        public ISText() { }
        public ISText(ISText src)
        {
            this.FontStyle = src.FontStyle;
            this.FontSize = src.FontSize;
            this.FontStyle = src.FontStyle;
            this.Color = src.Color;
            this.Align = src.Align;
            this.Wrap = src.Wrap;
        }

        public FontAsset FontAsset;
        public FontStyle FontStyle;
        public int FontSize = 14;
        public Color Color = Color.black;
        public TextAnchor Align = TextAnchor.MiddleLeft;
        public Wrap Wrap;
    }
}
