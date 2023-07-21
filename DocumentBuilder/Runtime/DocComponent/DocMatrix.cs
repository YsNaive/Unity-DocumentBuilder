using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

public class DocMatrix : DocVisual
{
    private static ISMargin margin = ISMargin.Pixel(10);
    private static ISPadding padding = ISPadding.None;
    public override VisualElement CreateEditGUI(DocComponent docComponent, int width)
    {
        VisualElement root = new VisualElement();
        root.style.backgroundColor = Color.red;
        root.style.width = width;
        Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
        if (data ==  null ) 
            data = new Data();
        TextField rowTextField = new TextField();
        rowTextField.label = "row";
        rowTextField.style.SetIS_Style(ISPadding.Pixel(10));
        rowTextField.style.width = width;
        rowTextField.RegisterValueChangedCallback((value)=>{
            int rowNum;
            if (int.TryParse(value.newValue, out rowNum))
            {

            }
        });
        root.Add(rowTextField);

        TextField colTextField = new TextField();
        colTextField.label = "col";
        colTextField.style.SetIS_Style(ISPadding.Pixel(10));
        colTextField.style.width = width;
        root.Add(colTextField);
        for (int i = 0;i < data.row;i++)
        {
            VisualElement row = new VisualElement();
            row.style.SetIS_Style(ISFlex.Horizontal);
            row.style.width = new Length(width - 20, LengthUnit.Pixel);
            row.style.backgroundColor = Color.white;
            row.style.SetIS_Style(ISMargin.Pixel(10));
            Length childWidth = new Length(((width - 20) / data.col) - (data.col - 1) * 10, LengthUnit.Pixel);
            for (int j = 0;j < data.col; j++)
            {
                TextField textField = new TextField();
                textField.label = "";
                textField.style.width = childWidth;
                textField.style.SetIS_Style(margin);
                row.Add(textField);
            }
            root.Add(row);
        }

        return root;
    }

    public override VisualElement CreateViewGUI(DocComponent docComponent, int width)
    {
        VisualElement root = new VisualElement();
        JsonUtility.FromJson<Data>(docComponent.JsonData);

        return root;
    }

    public override DocComponent SaveTo(VisualElement visualElement, ref DocComponent docComponent)
    {
        throw new System.NotImplementedException();
    }

    [System.Serializable]
    public class Data
    {
        public int row, col;
        public string[] contents;

        public Data()
        {
            row = col = 3;
            contents = new string[9];
        }
    }
}
