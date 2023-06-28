using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISBackground
    {
        public Color Color = Color.clear;
        public Color ImageTint = Color.white;
        [SerializeField] private Sprite Sprite = null;
        [SerializeField] private Texture2D Texture = null;
        [SerializeField] private RenderTexture RenderTexture = null;
        [SerializeField] private VectorImage VectorImage = null;
        public ScaleMode ScaleMode = ScaleMode.StretchToFill;

        public StyleBackground StyleBackground
        {
            get
            {
                if (Sprite!=null)
                    return new  StyleBackground(Sprite);
                if(Texture!=null)
                    return new  StyleBackground(Texture);
                if (RenderTexture != null)
                {
                    Background background = new Background();
                    background.renderTexture = RenderTexture;
                    return new StyleBackground(background);
                }
                if(Sprite!=null)
                    return new  StyleBackground(VectorImage);

                return null;
            }
        }
    }
}
