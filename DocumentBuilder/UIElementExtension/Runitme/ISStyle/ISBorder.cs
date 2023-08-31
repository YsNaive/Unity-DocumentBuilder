using UnityEngine;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISBorder
    {
        public Color LeftColor = Color.black; 
        public Color TopColor = Color.black; 
        public Color RightColor = Color.black;  
        public Color BottomColor = Color.black;
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public Color Color
        {
            set
            {
                LeftColor = value;
                TopColor = value;
                RightColor = value;
                BottomColor = value;
            }
        }
        public float Width
        {
            set
            {
                Left = value;
                Top = value;
                Right = value;
                Bottom = value;
            }
        }
        public ISBorder() { }

        public ISBorder(Color color, float size)
        {
            Color = color;
            Width = size;
        }

        public ISBorder Copy()
        {
            var copy  = new ISBorder();
            copy.Left = Left;
            copy.Top = Top;
            copy.Right = Right;
            copy.Bottom = Bottom;
            copy.LeftColor = LeftColor;
            copy.TopColor = TopColor;
            copy.RightColor = RightColor;
            copy.BottomColor = BottomColor;
            return copy;
        }
    }
}