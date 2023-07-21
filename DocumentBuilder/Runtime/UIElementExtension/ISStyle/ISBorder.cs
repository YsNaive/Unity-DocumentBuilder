using UnityEngine;
using UnityEngine.UIElements;
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

        public ISBorder() { }

        public ISBorder(Color color, float size)
        {
            LeftColor = color;
            TopColor = color;
            RightColor = color;
            BottomColor = color;
            Left = size;
            Top = size;
            Right = size;
            Bottom = size;
        }
    }
}