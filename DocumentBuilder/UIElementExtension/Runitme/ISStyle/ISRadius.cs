namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISRadius
    {
        public ISLength TopLeft = new ISLength();
        public ISLength BottomLeft = new ISLength();
        public ISLength TopRight = new ISLength();
        public ISLength BottomRight = new ISLength();
        public ISRadius Copy()
        {
            return new ISRadius
            {
                TopLeft = TopLeft,
                BottomLeft = BottomLeft,
                TopRight = TopRight,
                BottomRight = BottomRight
            };
        }
        public static ISRadius Pixel(float px)
        {
            return new ISRadius
            {
                TopLeft = ISLength.Pixel(px),
                TopRight = ISLength.Pixel(px),
                BottomLeft = ISLength.Pixel(px),
                BottomRight = ISLength.Pixel(px)
            };
        }
        public static ISRadius Percent(float percent)
        {
            return new ISRadius
            {
                TopLeft = ISLength.Percent(percent),
                TopRight = ISLength.Percent(percent),
                BottomLeft = ISLength.Percent(percent),
                BottomRight = ISLength.Percent(percent)
            };
        }
    }
}