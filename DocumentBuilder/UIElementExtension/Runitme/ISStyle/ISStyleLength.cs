using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public struct ISStyleLength
    {
        public StyleKeyword Keyword;
        public ISLength Value;

        public StyleLength Get
        {
            get
            {
                if(Keyword == StyleKeyword.Auto || Keyword == StyleKeyword.Initial)
                    return Keyword;
                else
                    return new StyleLength(Value.Get) { keyword = StyleKeyword.Undefined };
            }
        }
        public override string ToString()
        {
            return $"{Keyword}, {Value}";
        }
        public static ISStyleLength Auto { get => new ISStyleLength { Keyword = StyleKeyword.Auto }; }
        public static ISStyleLength Initial { get => new ISStyleLength { Keyword = StyleKeyword.Initial }; }
        public static ISStyleLength Pixel(float px)
        {
            return new ISStyleLength
            {
                Keyword = StyleKeyword.None,
                Value = ISLength.Pixel(px)
            };
        }
        public static ISStyleLength Percent(float percent)
        {
            return new ISStyleLength
            {
                Keyword = StyleKeyword.None,
                Value = ISLength.Percent(percent)
            };
        }

        public static ISStyleLength operator+ (ISStyleLength styleLength, int value)
        {
            styleLength.Value += value;
            return styleLength;
        }
        public static ISStyleLength operator- (ISStyleLength styleLength, int value)
        {
            styleLength.Value -= value;
            return styleLength;
        }
        public static ISStyleLength operator+ (ISStyleLength styleLength, float value)
        {
            styleLength.Value += value;
            return styleLength;
        }
        public static ISStyleLength operator- (ISStyleLength styleLength, float value)
        {
            styleLength.Value -= value;
            return styleLength;
        }
        public static ISStyleLength operator* (ISStyleLength styleLength, float value)
        {
            styleLength.Value *= value;
            return styleLength;
        }

        public static implicit operator StyleLength(ISStyleLength styleLength) { return styleLength.Get; }
    }
}
