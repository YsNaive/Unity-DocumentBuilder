using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace DocumentBuilder
{
    public class LabelComponent : DocComponent
    {
        public override VisualElement CreateRuntimeGUI()
        {
            LabelVisual output = new LabelVisual(this);
            return output;
        }

        public override VisualElement CreateEditorGUI()
        {
            LabelVisual output = new LabelVisual(this);
            return output;
        }

    }

    public class LabelVisual : TextElement
    {
        public LabelVisual(LabelComponent data)
        {
            this.text = data.StrData[0];
        }
    }
}
