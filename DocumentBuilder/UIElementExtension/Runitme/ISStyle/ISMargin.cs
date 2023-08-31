using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISMargin
    {
        public ISMargin() { }
        public ISMargin(TextAnchor anchor)
        {
            SetAnchor(anchor);
        }

        public ISStyleLength Left = ISStyleLength.Pixel(0);
        public ISStyleLength Top = ISStyleLength.Pixel(0);
        public ISStyleLength Right = ISStyleLength.Pixel(0);
        public ISStyleLength Bottom = ISStyleLength.Pixel(0);

        public void SetAnchor(TextAnchor anchor)
        {
            if(anchor == TextAnchor.UpperLeft)
            {
                if(Left.Keyword != StyleKeyword.None)
                    Left.Keyword = StyleKeyword.None;
                if(Top.Keyword != StyleKeyword.None)
                    Top.Keyword = StyleKeyword.None;
                Right.Keyword = StyleKeyword.Auto;
                Bottom.Keyword = StyleKeyword.Auto;
                return;
            }
            if(anchor == TextAnchor.UpperCenter)
            {
                Left.Keyword = StyleKeyword.Auto;
                if (Top.Keyword != StyleKeyword.None)
                    Top.Keyword = StyleKeyword.None;
                Right.Keyword = StyleKeyword.Auto;
                Bottom.Keyword = StyleKeyword.Auto;
                return;
            }
            if(anchor == TextAnchor.UpperRight)
            {
                Left.Keyword = StyleKeyword.Auto;
                if (Top.Keyword != StyleKeyword.None)
                    Top.Keyword = StyleKeyword.None;
                if (Right.Keyword != StyleKeyword.None)
                    Right.Keyword = StyleKeyword.None;
                Bottom.Keyword = StyleKeyword.Auto;
                return;
            }
            if(anchor == TextAnchor.MiddleLeft)
            {
                if (Left.Keyword != StyleKeyword.None)
                    Left.Keyword = StyleKeyword.None;
                Top.Keyword = StyleKeyword.Auto;
                Right.Keyword = StyleKeyword.Auto;
                Bottom.Keyword = StyleKeyword.Auto;
                return;
            }
            if(anchor == TextAnchor.MiddleCenter)
            {
                Left.Keyword = StyleKeyword.Auto;
                Top.Keyword = StyleKeyword.Auto;
                Right.Keyword = StyleKeyword.Auto;
                Bottom.Keyword = StyleKeyword.Auto;
                return;
            }
            if(anchor == TextAnchor.MiddleRight)
            {
                Left.Keyword = StyleKeyword.Auto;
                Top.Keyword = StyleKeyword.Auto;
                if (Right.Keyword != StyleKeyword.None)
                    Right.Keyword = StyleKeyword.None;
                Bottom.Keyword = StyleKeyword.Auto;
                return;
            }
            if(anchor == TextAnchor.LowerLeft)
            {
                if (Left.Keyword != StyleKeyword.None)
                    Left.Keyword = StyleKeyword.None;
                Top.Keyword = StyleKeyword.Auto;
                Right.Keyword = StyleKeyword.Auto;
                if (Bottom.Keyword != StyleKeyword.None)
                    Bottom.Keyword = StyleKeyword.None;
                return;
            }
            if(anchor == TextAnchor.LowerCenter)
            {
                Left.Keyword = StyleKeyword.Auto;
                Top.Keyword = StyleKeyword.Auto;
                Right.Keyword = StyleKeyword.Auto;
                if (Bottom.Keyword != StyleKeyword.None)
                    Bottom.Keyword = StyleKeyword.None;
                return;
            }
            if(anchor == TextAnchor.LowerRight)
            {
                Left.Keyword = StyleKeyword.Auto;
                Top.Keyword = StyleKeyword.Auto;
                if (Right.Keyword != StyleKeyword.None)
                    Right.Keyword = StyleKeyword.None;
                if (Bottom.Keyword != StyleKeyword.None)
                    Bottom.Keyword = StyleKeyword.None;
                return;
            }
        }

        public static ISMargin operator *(ISMargin styleLength, float value)
        {
            styleLength.Top *= value;
            styleLength.Left *= value;
            styleLength.Right *= value;
            styleLength.Bottom *= value;
            return styleLength;
        }
        public ISMargin Copy()
        {
            return new ISMargin
            {
                Left = Left,
                Top = Top,
                Right = Right,
                Bottom = Bottom
            };
        }
        public static ISMargin Pixel(int px)
        {
            return new ISMargin
            {
                Left = ISStyleLength.Pixel(px),
                Top = ISStyleLength.Pixel(px),
                Right = ISStyleLength.Pixel(px),
                Bottom = ISStyleLength.Pixel(px)
            };
        }
        public static ISMargin Percent(int percent)
        {
            return new ISMargin
            {
                Left = ISStyleLength.Percent(percent),
                Top = ISStyleLength.Percent(percent),
                Right = ISStyleLength.Percent(percent),
                Bottom = ISStyleLength.Percent(percent)
            };
        }

        public static ISMargin None { get { return new ISMargin(); } }
    }
}