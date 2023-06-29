using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DocumentBuilder
{
    [Serializable]
    public class PictureComponent : DocComponent
    {
        public override VisualElement CreateEditorGUI()
        {
            throw new System.NotImplementedException();
        }

        public override VisualElement CreateRuntimeGUI()
        {
            PictureVisual output = new PictureVisual(this);
            return output;
        }
    }

    public class PictureVisual : VisualElement
    {
        public PictureVisual(PictureComponent data)
        {
            Texture2D texture = (Texture2D)data.ObjData[0];
            float ratio = float.Parse(data.StrData[0]);
            this.style.backgroundImage = new StyleBackground(texture);
            this.style.SetIS_Style(ISSize.Pixel(texture.width * ratio, texture.height * ratio));
        }
    }
}

