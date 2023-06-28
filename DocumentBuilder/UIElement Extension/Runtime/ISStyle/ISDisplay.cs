using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISDisplay
    {
        [Range(0f, 1f)]
        public float Opacity = 1f;
        public DisplayStyle Display = DisplayStyle.Flex;
        public Visibility Visibility = Visibility.Visible;
        public Overflow Overflow = Overflow.Visible;
    }
}