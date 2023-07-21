using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DocMatrix : DocVisual
{
    public override VisualElement CreateEditGUI(ref DocComponent docComponent)
    {
        VisualElement root = new VisualElement();
        Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
        TextField rowTextField = new TextField();
        rowTextField.label = "row";
        foreach (var child in rowTextField.Children()){

        }
        root.Add(rowTextField);

        TextField colTextField = new TextField();
        rowTextField.label = "col";
        root.Add(colTextField);
        for (int i = 0;i < data.row;i++)
        {
            for (int j = 0;j < data.col; j++)
            {

            }
        }

        return root;
    }

    public override VisualElement CreateViewGUI(DocComponent docComponent)
    {
        VisualElement root = new VisualElement();
        JsonUtility.FromJson<Data>(docComponent.JsonData);

        return root;
    }

    public override DocComponent SaveFromEditGUI(VisualElement visualElement)
    {
        throw new System.NotImplementedException();
    }

    [System.Serializable]
    public class Data
    {
        public int row, col;
        public string[] contents;
    }
}
