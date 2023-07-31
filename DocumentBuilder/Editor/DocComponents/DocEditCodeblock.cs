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
    public class DocEditCodeblock : DocEditVisual
    {
        public override string DisplayName => "Advance/Code block";

        public override string VisualID => "7";

        public TextField textInput;

        DocCodeblock.Data data;
        protected override void OnCreateGUI()
        {
            data = JsonUtility.FromJson<DocCodeblock.Data>(Target.JsonData);
            data ??= new DocCodeblock.Data();
            var hor = DocRuntime.NewEmptyHorizontal();
            IntegerField maxHeightInput = DocEditor.NewIntField(" Max height", e =>
            {
                data.MaxHeight = e.newValue;
                save();
            });
            maxHeightInput.value = data.MaxHeight;
            maxHeightInput.style.width = Length.Percent(50);
            maxHeightInput[0].style.minWidth = 75;
            hor.Add(maxHeightInput);

            IntegerField lineHeightInput = DocEditor.NewIntField(" Line Height%", e =>
            {
                data.LineHeightPercent = e.newValue;
                save();
            });
            lineHeightInput.value = data.LineHeightPercent;
            lineHeightInput.style.width = Length.Percent(50);
            lineHeightInput[0].style.minWidth = 100;
            hor.Add(lineHeightInput);
            textInput = DocRuntime.NewTextField("", e =>
            {
                if (Target.TextData.Count == 0) Target.TextData.Add(e.newValue);
                else Target.TextData[0] = e.newValue;
                save();
            });
            textInput.multiline = true;
            if (Target.TextData.Count == 0) Target.TextData.Add("");
            textInput.value = Target.TextData[0];
            Add(hor);
            Add(textInput);
        }

        void save()
        {
            Target.JsonData = JsonUtility.ToJson(data);
        }
    }
}
