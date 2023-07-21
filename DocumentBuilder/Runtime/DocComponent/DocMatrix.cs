using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocMatrix : DocVisual
    {
        private static ISMargin margin = ISMargin.None;
        private static ISPadding padding = ISPadding.None;
        private float space;
        private VisualElement matrixVisual;

        public override string DisplayName => "Matrix";

        public override VisualElement CreateEditGUI(DocComponent docComponent, int width)
        {
            VisualElement root = new VisualElement();
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.style.width = width;
            space = (width * 0.005f);
            Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
            if (data == null)
                data = new Data();
            TextField rowTextField = new TextField();
            rowTextField.label = "row";
            rowTextField.style.SetIS_Style(DocStyle.Current.MainText);
            rowTextField.style.SetIS_Style(ISPadding.Pixel((int)space));
            rowTextField.style.width = width;
            rowTextField.RegisterValueChangedCallback((value) =>
            {
                int rowNum;
                if (int.TryParse(value.newValue, out rowNum))
                {
                    data.row = rowNum;
                    root.Remove(matrixVisual);
                    matrixVisual = generateMatrixVisual(data, width);
                    root.Add(matrixVisual);
                }
            });
            root.Add(rowTextField);

            TextField colTextField = new TextField();
            colTextField.label = "col";
            colTextField.style.SetIS_Style(DocStyle.Current.MainText);
            colTextField.style.SetIS_Style(ISPadding.Pixel((int)space));
            colTextField.style.width = width;
            colTextField.RegisterValueChangedCallback((value) =>
            {
                int colNum;
                if (int.TryParse(value.newValue, out colNum))
                {
                    data.col = colNum;
                    root.Remove(matrixVisual);
                    matrixVisual = generateMatrixVisual(data, width);
                    root.Add(matrixVisual);
                }
            });
            root.Add(colTextField);
            matrixVisual = generateMatrixVisual(data, width);
            root.Add(matrixVisual);

            return root;
        }

        public override VisualElement CreateViewGUI(DocComponent docComponent, int width)
        {
            VisualElement root = new VisualElement();
            JsonUtility.FromJson<Data>(docComponent.JsonData);

            return root;
        }

        public override DocComponent SaveTo(VisualElement visualElement, DocComponent docComponent)
        {
            throw new System.NotImplementedException();
        }

        private VisualElement generateMatrixVisual(Data data, int width)
        {
            ISPosition position = new ISPosition();
            position.Position = Position.Absolute;
            position.Left = ISStyleLength.Pixel(space);
            position.Top = ISStyleLength.Pixel(space);
            VisualElement root = new VisualElement();
            root.style.width = width;
            float childWidth = ((width - 2f * space) / data.col) - space;
            float childHeight = 30;
            for (int i = 0; i < data.row; i++)
            {
                for (int j = 0; j < data.col; j++)
                {
                    TextField textField = new TextField();
                    textField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    textField.label = "";
                    textField.style.SetIS_Style(margin);
                    textField.style.SetIS_Style(padding);
                    textField.style.SetIS_Style(position);
                    textField.style.SetIS_Style(DocStyle.Current.MainText);
                    textField.style.width = childWidth;
                    textField.style.height = childHeight;
                    position.Left += childWidth + space;
                    root.Add(textField);
                }
                position.Left = ISStyleLength.Pixel(space);
                position.Top += childHeight + space;
            }
            root.style.height = position.Top + space;
            return root;
        }

        private string[] resizeContent(Data data)
        {
            string[,] contents = new string[data.row, data.col];
            for (int i = 0;i < data.row; i++)
            {
                for (int j = 0;j < data.col; j++)
                {

                }
            }
            return null;
        }

        [System.Serializable]
        public class Data
        {
            public int row, col;
            public string[,] contents;

            public Data()
            {
                row = col = 3;
                contents = new string[3, 3];
            }
        }
    }
}
