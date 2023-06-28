using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISSize
    {
        public ISStyleLength Width = ISStyleLength.Pixel(100);
        public ISStyleLength Height = ISStyleLength.Pixel(100);


        public ISSize Copy()
        {
            return new ISSize { Height = Height.Copy(), Width = Width.Copy() };
        }
        public static ISSize Percent(Vector2 size) { return Percent(size.x, size.y); }
        public static ISSize Percent(float width, float height)
        {
            return new ISSize()
            {
                Width = ISStyleLength.Percent(width),
                Height = ISStyleLength.Percent(height)
            };
        }
        public static ISSize Pixel(Vector2 size) { return Pixel(size.x, size.y); }
        public static ISSize Pixel(float width, float height)
        {
            return new ISSize()
            {
                Width = ISStyleLength.Pixel(width),
                Height = ISStyleLength.Pixel(height)
            };
        }
        public static ISSize Full
        {
            get
            {
                return new ISSize()
                {
                    Width = ISStyleLength.Percent(100),
                    Height = ISStyleLength.Percent(100)
                };
            }
        }

    }
}