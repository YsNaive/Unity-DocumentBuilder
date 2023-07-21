using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISFlex
    {
        public ISStyleLength Basis = ISStyleLength.Auto;
        public FlexDirection Direction;
        public Wrap Wrap;

        public static ISFlex Horizontal
        {
            get
            {
                return new ISFlex
                {
                    Direction = FlexDirection.Row,
                    Wrap = Wrap.Wrap
                };
            }
        }
        public static ISFlex Vertical
        {
            get
            {
                return new ISFlex
                {
                    Direction = FlexDirection.Column,
                    Wrap = Wrap.Wrap
                };
            }
        }
    }
}