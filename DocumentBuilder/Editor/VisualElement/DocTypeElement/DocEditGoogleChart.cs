using NaiveAPI_Editor.DocumentBuilder;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    [CustomDocEditVisual("Charts/GoogleChart", 4)]
    public class DocEditGoogleChart : DocEditVisual<DocGoogleChart.Data>
    {
        public override string VisualID => "9";

        VisualElement editViewContainer;
        DSEnumField<DocGoogleChart.ChType> chTypeSelector;
        protected override void OnCreateGUI()
        {
            editViewContainer = new();
            chTypeSelector = new DSEnumField<DocGoogleChart.ChType>("Chart Type", visualData.ChType, evt =>
            {
                if (evt.newValue == evt.previousValue) return;
                editViewContainer.Clear();
                visualData.ChType = evt.newValue;
                visualData.Contents = "";
                SaveDataToTarget();
                createChTypeEditView();
            }); createChTypeEditView();
            var sizeField = DocEditor.NewIntField("Size", evt =>
            {
                visualData.ChHeightPx = evt.newValue;
                SaveDataToTarget();
            });
            chTypeSelector.labelElement.style.width = 80;
            chTypeSelector.labelElement.style.minWidth = 80;
            sizeField.value = visualData.ChHeightPx;
            var scaleField = new DSTextField("Scale");
            scaleField.RegisterCallback<FocusOutEvent>(evt =>
            {
                float val;
                if(float.TryParse(scaleField.value, out val))
                {
                    visualData.Scale = val;
                    SaveDataToTarget();
                }
                else
                {
                    scaleField.value = visualData.Scale.ToString();
                }
            });
            scaleField.value = visualData.Scale.ToString();
            sizeField.labelElement.style.minWidth = 35;
            sizeField.labelElement.style.width = 35;
            scaleField.labelElement.style.minWidth = 35;
            scaleField.labelElement.style.width = 35;
            Add(new DSHorizontal(1f,chTypeSelector, null, sizeField, scaleField));
            Add(editViewContainer);
        }
        void createChTypeEditView()
        {
            if (chTypeSelector.value == DocGoogleChart.ChType.TexFormula)
                createEditTexFormula();
        }
        void createEditTexFormula()
        {
            var textField = new DSTextField("",evt =>
            {
                visualData.Contents = evt.newValue;
                SaveDataToTarget();
            }); textField.value = visualData.Contents;
            editViewContainer.Add(textField);
        }
    }
}