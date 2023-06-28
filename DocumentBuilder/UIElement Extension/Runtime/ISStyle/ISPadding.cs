using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISPadding
    {
        public ISStyleLength Left = ISStyleLength.Pixel(0);
        public ISStyleLength Top = ISStyleLength.Pixel(0);
        public ISStyleLength Right = ISStyleLength.Pixel(0);
        public ISStyleLength Bottom = ISStyleLength.Pixel(0);
        public static ISPadding Pixel(int px)
        {
            return new ISPadding
            {
                Left = ISStyleLength.Pixel(px),
                Top = ISStyleLength.Pixel(px),
                Right = ISStyleLength.Pixel(px),
                Bottom = ISStyleLength.Pixel(px)
            };
        }
        public static ISPadding Percent(int percent)
        {
            return new ISPadding
            {
                Left = ISStyleLength.Pixel(percent),
                Top = ISStyleLength.Pixel(percent),
                Right = ISStyleLength.Pixel(percent),
                Bottom = ISStyleLength.Pixel(percent)
            };
        }
        public static ISPadding None
        {
               get { return new ISPadding(); }
        }
    }
}