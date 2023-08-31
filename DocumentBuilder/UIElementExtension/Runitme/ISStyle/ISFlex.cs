using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISFlex
    {
        public ISStyleLength Basis = ISStyleLength.Auto;
        public FlexDirection Direction;
        public Wrap Wrap;

        public ISFlex Copy()
        {
            var copy = new ISFlex();
            copy.Basis = Basis.Copy();
            copy.Direction = Direction;
            copy.Wrap = Wrap;
            return copy;
        }

        public static ISFlex Horizontal => new ISFlex
        {
            Direction = FlexDirection.Row,
            Wrap = Wrap.Wrap,
        };

        public static ISFlex Vertical => new ISFlex
        {
            Direction = FlexDirection.Column,
            Wrap = Wrap.Wrap
        };
    }
}