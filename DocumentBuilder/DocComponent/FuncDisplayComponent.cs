using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DocumentBuilder
{
    [Serializable]
    public class FuncDisplayComponent : DocComponent
    {
        public override VisualElement CreateEditorGUI()
        {
            throw new System.NotImplementedException();
        }

        public override VisualElement CreateRuntimeGUI()
        {
            FuncDisplayVisual output = new FuncDisplayVisual(this);
            return output;
        }
    }

    public class FuncDisplayVisual : VisualElement
    {
        public FuncDisplayVisual(FuncDisplayComponent data)
        {
            TextElement funcElement = new TextElement();
            TextElement descriptionElement = new TextElement();
            funcElement.text = data.StrData[0];
            descriptionElement.text = "\t" + data.StrData[1];
            this.Add(funcElement);
            this.Add(descriptionElement);
        }
    }
}
