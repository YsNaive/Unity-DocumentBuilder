using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Label", 1)]
    public class DocEditLabel : DocEditVisual<DocLabel.Data>
    {
        [Obsolete] public override string DisplayName => "Label";
        public override string VisualID => "1";
        protected override void OnCreateGUI()
        {
            TextField labelInput = new DSTextField("", (val) =>
            {
                Target.TextData.Clear();
                Target.TextData.Add(val.newValue);
            });
            if (Target.TextData.Count == 0)
                Target.TextData.Add(string.Empty);
            labelInput.value = Target.TextData[0];
            labelInput.style.SetIS_Style(ISMargin.None);
            IntegerField intField = null;
            DocStyle.Current.BeginLabelWidth(ISLength.Pixel(36));
            intField = DocEditor.NewIntField("Level", e =>
            {
                visualData.Level = Mathf.Clamp(e.newValue, 1, 6);
                if (e.newValue != visualData.Level)
                    intField.value = visualData.Level;
                SaveDataToTarget();
            });
            intField.value = visualData.Level;
            intField.label = "Level";
            DocStyle.Current.EndLabelWidth();
            var addBtn = new DSButton("+", DocStyle.Current.SuccessColor, () =>
            {
                intField.value += 1;
            });
            var subBtn = new DSButton("-", DocStyle.Current.DangerColor, () =>
            {
                intField.value -= 1;
            });
            intField.RegisterCallback<GeometryChangedEvent>(e =>
            {
                addBtn.style.width = e.newRect.height;
                addBtn.style.height = e.newRect.height;
                subBtn.style.width = e.newRect.height;
                subBtn.style.height = e.newRect.height;
            });
            Add(new DSHorizontal(1f,labelInput,null,new DSHorizontal(intField,null,addBtn,subBtn)));
        }

        public override string ToMarkdown(string dstPath)
        {
            string output = "";
            for(int i=0;i<visualData.Level;i++)
                output += "#";
            output +=' '+ Target.TextData[0];
            return output;
        }
    }
}
