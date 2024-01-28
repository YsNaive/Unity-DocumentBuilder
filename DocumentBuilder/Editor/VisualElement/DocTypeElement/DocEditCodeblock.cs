using NaiveAPI.DocumentBuilder;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Advance/Code block", 0)]
    public class DocEditCodeblock : DocEditVisual<DocCodeblock.Data>
    {
        [Obsolete]public override string DisplayName => "Code block";
        public override string VisualID => "7";
        public override ushort Version => 0;

        public TextField textInput;
        protected override void OnCreateGUI()
        {
            var hor = new DSHorizontal();
            IntegerField maxHeightInput = DocEditor.NewIntField(" Max height", e =>
            {
                visualData.MaxHeight = e.newValue;
                SaveDataToTarget();
            });
            maxHeightInput.value = visualData.MaxHeight;
            maxHeightInput.style.width = Length.Percent(50);
            maxHeightInput[0].style.minWidth = 75;
            hor.Add(maxHeightInput);

            IntegerField lineHeightInput = DocEditor.NewIntField(" Line Height%", e =>
            {
                visualData.LineHeightPercent = e.newValue;
                SaveDataToTarget();
            });
            lineHeightInput.value = visualData.LineHeightPercent;
            lineHeightInput.style.width = Length.Percent(50);
            lineHeightInput[0].style.minWidth = 100;
            hor.Add(lineHeightInput);
            textInput = new DSTextField("", e =>
            {
                if (Target.TextData.Count == 0) Target.TextData.Add(e.newValue);
                else Target.TextData[0] = e.newValue;
                SaveDataToTarget();
            });
            textInput.multiline = true;
            if (Target.TextData.Count == 0) Target.TextData.Add("");
            textInput.value = Target.TextData[0];
            Add(hor);
            Add(textInput);
        }
    }
}
