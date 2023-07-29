using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditLabel : DocEditVisual
    {
        public override string DisplayName => "Label";
        public override string VisualID => "1";

        DocLabel.Data data;
        protected override void OnCreateGUI()
        {
            data = JsonUtility.FromJson<DocLabel.Data>(Target.JsonData);
            data ??= new DocLabel.Data();
            style.SetIS_Style(ISFlex.Horizontal);
            TextField labelInput = new TextField();
            labelInput.style.width = Length.Percent(68);
            if (Target.TextData.Count == 0)
                Target.TextData.Add(string.Empty);
            labelInput.value = Target.TextData[0];
            labelInput.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.TextData.Add(val.newValue);
            });
            labelInput.style.SetIS_Style(ISMargin.None);
            Add(labelInput);
            IntegerField intField = null;
            intField = DocEditor.NewIntField("", e =>
            {
                data.Level = Mathf.Clamp(e.newValue, 1, 6);
                if(e.newValue != data.Level)
                    intField.value = data.Level;
                save();
            });
            intField.style.width = Length.Percent(20);
            intField.value = data.Level;
            intField.label = "Level";
            intField[0].style.minWidth = 36;
            intField.style.marginLeft = Length.Percent(1);
            var addBtn = DocRuntime.NewButton("+", DocStyle.Current.SuccessColor, () =>
            {
                intField.value += 1;
            });
            var subBtn = DocRuntime.NewButton("-", DocStyle.Current.DangerColor, () =>
            {
                intField.value -= 1;
            });
            intField.RegisterCallback<GeometryChangedEvent>(e =>{
                addBtn.style.width = e.newRect.height;
                addBtn.style.height = e.newRect.height;
                subBtn.style.width = e.newRect.height;
                subBtn.style.height = e.newRect.height;
            });
            Add(intField);
            Add(addBtn);
            Add(subBtn);
        }
        void save()
        {
            Target.JsonData = JsonUtility.ToJson(data);
        }
    }
}
