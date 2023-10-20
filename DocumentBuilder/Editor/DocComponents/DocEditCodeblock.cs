using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditCodeblock : DocEditVisual<(int MaxHeight, int LineHeightPercent)>
    {
        public override string DisplayName => "Advance/Code block";

        public override string VisualID => "7";
        public override ushort Version => 1;

        public TextField textInput;
        protected override void OnCreateGUI()
        {
            var hor = DocRuntime.NewEmptyHorizontal();
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
            textInput = DocRuntime.NewTextField("", e =>
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
        protected override void VersionConflict()
        {
            if(Target.VisualVersion < 1)
            {
                Target.JsonData = Target.JsonData
                    .Replace("MaxHeight", "Item1")
                    .Replace("LineHeightPercent", "Item2");
            }
            Target.VisualVersion = 1;
        }
    }
}
