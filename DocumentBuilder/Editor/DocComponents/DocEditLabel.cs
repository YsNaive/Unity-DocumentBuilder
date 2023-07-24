using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditLabel : DocEditVisual
    {
        public override string DisplayName => "Label";
        public override string VisualID => "1";

        public override void OnCreateGUI()
        {
            TextField labelInput = new TextField();
            labelInput.value = Target.TextData[0];
            labelInput.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.TextData.Add(val.newValue);
            });
            Add(labelInput);
        }
    }
}
