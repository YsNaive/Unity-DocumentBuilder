using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DocumentBuilder 
{
    [Serializable]
    public class DescriptionComponent : DocComponent
    {
        public override VisualElement CreateEditorGUI()
        {
            throw new System.NotImplementedException();
        }

        public override VisualElement CreateRuntimeGUI()
        {
            DescriptionVisual output = new DescriptionVisual(this);
            return output;
        }
    }

    public class DescriptionVisual : TextElement
    {
        public DescriptionVisual(DescriptionComponent data)
        {
            this.text = data.StrData[0];
        }
    }
}
