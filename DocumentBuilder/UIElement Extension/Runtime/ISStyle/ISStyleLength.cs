using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISStyleLength
    {
        public StyleKeyword Keyword = StyleKeyword.None;
        public ISLength Value = new ISLength();
        public StyleLength Get
        {
            get
            {
                if (Keyword == StyleKeyword.Auto || Keyword == StyleKeyword.Initial)
                {
                    return new StyleLength(Keyword);
                }
                else
                {
                    return new StyleLength(Value.Get);
                }
            }
        }
        public ISStyleLength Copy()
        {
            return new ISStyleLength { Keyword = Keyword , Value = Value.Copy()};
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

        public static implicit operator StyleLength(ISStyleLength styleLength) { return styleLength.Get; }
    }
}
