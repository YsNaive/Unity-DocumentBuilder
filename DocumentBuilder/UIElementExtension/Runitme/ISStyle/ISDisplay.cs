using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISDisplay
    {
        [SerializeField, Range(0f, 1f)]
        private float m_opacity = 1f;
        public float Opacity
        {
            get => m_opacity;
            set => Mathf.Clamp01(value);
        }
        public DisplayStyle Display = DisplayStyle.Flex;
        public Visibility Visibility = Visibility.Visible;
        public Overflow Overflow = Overflow.Visible;

        public ISDisplay Copy()
        {
            var copy = new ISDisplay();
            copy.Opacity = m_opacity;
            copy.Visibility = Visibility;
            copy.Display = Display;
            copy.Overflow = Overflow;
            return copy;
        }
    }
}