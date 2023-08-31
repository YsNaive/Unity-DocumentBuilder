using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISPosition
    {
        public Position Position;
        public ISStyleLength Left = new ISStyleLength();
        public ISStyleLength Top = new ISStyleLength();
        public ISStyleLength Right = new ISStyleLength();
        public ISStyleLength Bottom = new ISStyleLength();
        public ISPosition Copy()
        {
            return new ISPosition
            {
                Position = Position,
                Left = Left,
                Top = Top,
                Right = Right,
                Bottom = Bottom
            };
        }
        public override string ToString()
        {
            return $"{Position}: Left({Left}) Top({Top}) Right({Right}) Bottom({Bottom})";
        }
    }
}
