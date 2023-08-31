using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISAlign
    {
        public Justify Content;
        public Align Items;
        public ISAlign Copy()
        {
            return new ISAlign
            {
                Content = Content,
                Items = Items
            };
        }
    }
}