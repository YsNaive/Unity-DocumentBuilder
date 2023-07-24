using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocLabel : DocVisual
    {
        public override string VisualID => "1";

        public override void OnCreateGUI()
        {
            TextElement text = new TextElement();
            if (Target.TextData.Count > 0)
                text.text = Target.TextData[0];
            text.style.SetIS_Style(DocStyle.Current.LabelText);
            Add(text);
        }
    }

}