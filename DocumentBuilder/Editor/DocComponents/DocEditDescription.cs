using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditDescription : DocEditVisual
    {
        public override string DisplayName => "Description";

        public override string VisualID => "2";

        public override void OnCreateGUI()
        {
            TextField textInput = new TextField();
            if (Target.TextData.Count == 0)
                Target.TextData.Add(string.Empty);
            textInput.value = Target.TextData[0];
            textInput.multiline = true;
            textInput.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.TextData.Add(val.newValue);
            });
            Add(textInput);
        }
    }
}
