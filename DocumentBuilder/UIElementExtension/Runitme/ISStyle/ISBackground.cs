using UnityEngine;
using UnityEngine.UIElements;
namespace NaiveAPI_UI
{
    [System.Serializable]
    public class ISBackground
    {
        public Color Color = Color.clear;
        public Color ImageTint = Color.white;
        [SerializeField] private Sprite sprite = null;
        [SerializeField] private Texture2D texture = null;
        [SerializeField] private RenderTexture renderTexture = null;
        [SerializeField] private VectorImage vectorImage = null;
        public ScaleMode ScaleMode = ScaleMode.StretchToFill;
        public Sprite Sprite { get => sprite; set => sprite = value; }
        public Texture2D Texture2D { get => Texture2D; set => texture = value; }
        public RenderTexture RenderTexture { get => RenderTexture; set => renderTexture = value; }
        public VectorImage VectorImage { get => VectorImage; set => vectorImage = value; }
        public StyleBackground StyleBackground
        {
            get
            {
                if (sprite!=null)
                    return new  StyleBackground(sprite);
                if(texture!=null)
                    return new  StyleBackground(texture);
                if (renderTexture != null)
                {
                    Background background = new Background();
                    background.renderTexture = renderTexture;
                    return new StyleBackground(background);
                }
                if(vectorImage != null)
                    return new  StyleBackground(vectorImage);

                return null;
            }
        }
        public ISBackground Copy()
        {
            var copy = new ISBackground();
            copy.Color = Color;
            copy.ImageTint = ImageTint;
            copy.sprite = sprite;
            copy.texture = texture;
            copy.renderTexture = renderTexture;
            copy.vectorImage = vectorImage;
            return copy;
        }
        public static implicit operator StyleBackground(ISBackground background)
        {
            return background.StyleBackground;
        }
    }
}
