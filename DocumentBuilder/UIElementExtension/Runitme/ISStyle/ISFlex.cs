using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISFlex
    {
        public ISStyleLength Basis = ISStyleLength.Auto;
        public float FlexGrow = 0;
        public FlexDirection Direction;
        public Wrap Wrap;

        public ISFlex Copy()
        {
            var copy = new ISFlex();
            copy.Basis = Basis;
            copy.Direction = Direction;
            copy.Wrap = Wrap;
            copy.FlexGrow = FlexGrow;
            return copy;
        }

        public static ISFlex Horizontal => new ISFlex
        {
            Direction = FlexDirection.Row,
            Wrap = Wrap.Wrap,
            FlexGrow = 1,
        };

        public static ISFlex Vertical => new ISFlex
        {
            Direction = FlexDirection.Column,
            Wrap = Wrap.Wrap
        };
    }
}