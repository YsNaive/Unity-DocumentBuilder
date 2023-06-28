using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISLength
    {
        public LengthUnit Unit = LengthUnit.Pixel;
        public float Value = 0;
        public Length Get
        {
            get
            {
                return new Length(Value, Unit);
            }
        }
        public ISLength Copy() {
            return new ISLength { Unit = Unit, Value = Value};
        }
        public override string ToString()
        {
            return $"{Unit}, {Value}";
        }
        public static ISLength Pixel(float px)
        {
            return new ISLength
            {
                Unit = LengthUnit.Pixel,
                Value = px
            };
        }
        public static ISLength Percent(float percent)
        {
            return new ISLength
            {
                Unit = LengthUnit.Percent,
                Value = percent
            };
        }
        public static ISLength operator+ (ISLength length,int value){
            length.Value += value;
            return length;
        }
        public static ISLength operator- (ISLength length,int value){
            length.Value -= value;
            return length;
        }
        public static ISLength operator+ (ISLength length, float value){
            length.Value += value;
            return length;
        }
        public static ISLength operator- (ISLength length,float value){
            length.Value -= value;
            return length;
        }

        public static implicit operator Length(ISLength length) { return length.Get; }
    }
}