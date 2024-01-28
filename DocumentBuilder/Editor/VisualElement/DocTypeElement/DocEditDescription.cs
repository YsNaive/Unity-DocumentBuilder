using NaiveAPI.DocumentBuilder;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Description",0)]
    public class DocEditDescription : DocEditVisual<DocDescription.Data>
    {
        [Obsolete] public override string DisplayName => "Description";
        public override string VisualID => "2";
        public override ushort Version => 0;
        protected override void OnCreateGUI()
        {
            TextField textInput = new DSTextField();
            if (Target.TextData.Count == 0)
                Target.TextData.Add(string.Empty);
            textInput.value = Target.TextData[0];
            textInput.multiline = true;
            textInput.style.whiteSpace = WhiteSpace.Normal;
            textInput.RegisterValueChangedCallback((val) =>
            {
                if(Target.TextData.Count == 0)
                    Target.TextData.Add(val.newValue);
                else
                    Target.TextData[0] = val.newValue;
            });
            var typeField = new DSEnumField<DocDescription.DescriptionType>("Type", visualData.Type, e =>
            {
                visualData.Type = e.newValue;
                SaveDataToTarget();
            });
            typeField.labelElement.style.minWidth = 45;
            typeField.labelElement.style.width = 45;
            typeField.labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
            Add(typeField);
            Add(textInput);
        }
        public override string ToMarkdown(string dstPath)
        {
            return Target.TextData[0];
        }
    }
}
