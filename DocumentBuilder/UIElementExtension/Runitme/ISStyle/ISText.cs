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

        public ISText Copy()
        {
            var copy = new ISText();
            copy.FontAsset = this.FontAsset;
            copy.FontStyle = this.FontStyle;
            copy.FontSize = this.FontSize;
            copy.Color = this.Color;
            copy.Align = this.Align;
            copy.Wrap = this.Wrap;
            return copy;
        }

        public FontAsset FontAsset;
        public FontStyle FontStyle;
        public int FontSize = 14;
        public Color Color = Color.black;
        public TextAnchor Align = TextAnchor.MiddleLeft;
        public Wrap Wrap;
    }
}
