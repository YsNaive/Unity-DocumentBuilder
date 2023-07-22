using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            rowTextField[0].style.minWidth = width * 0.2f;
            rowTextField.value = data.row + "";
            rowTextField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            rowTextField.style.SetIS_Style(DocStyle.Current.MainText);
            rowTextField.style.SetIS_Style(margin);
            rowTextField.style.SetIS_Style(ISPadding.Pixel((int)space));
            rowTextField.style.width = width - 2 * space;
            rowTextField.RegisterValueChangedCallback((value) =>
            {
                int rowNum;
                if (int.TryParse(value.newValue, out rowNum))
                {
                    data.row = rowNum;
                    root.Remove(matrixVisual);
                    data.contents = resizeContent(data.row, data.col, data.contents);
                    matrixVisual = generateEditMatrixVisual(data, width);
                    root.Add(matrixVisual);
                }
            });
            root.Add(rowTextField);

            TextField colTextField = new TextField();
            colTextField.label = "col";
            colTextField[0].style.minWidth = width * 0.2f;
            colTextField.value = data.col + "";
            colTextField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            colTextField.style.SetIS_Style(DocStyle.Current.MainText);
            colTextField.style.SetIS_Style(margin);
            colTextField.style.SetIS_Style(ISPadding.Pixel((int)space));
            colTextField.style.width = width - 2 * space;
            colTextField.RegisterValueChangedCallback((value) =>
            {
                int colNum;
                if (int.TryParse(value.newValue, out colNum))
                {
                    data.col = colNum;
                    root.Remove(matrixVisual);
                    data.contents = resizeContent(data.row, data.col, data.contents);
                    matrixVisual = generateEditMatrixVisual(data, width);
                    root.Add(matrixVisual);
                }
            });
            root.Add(colTextField);
            matrixVisual = generateEditMatrixVisual(data, width);
            root.Add(matrixVisual);

            return root;
        }

        public override VisualElement CreateViewGUI(DocComponent docComponent, int width)
        {
            Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
            if (data == null)
                data = new Data();

            return generateViewMatrixVisual(data, width);
        }

        public override DocComponent SaveTo(VisualElement visualElement, DocComponent docComponent)
        {
            Data newData = new Data();
            if (!int.TryParse(((TextField) visualElement[0]).value, out newData.row))
                newData.row = 3;
            if (!int.TryParse(((TextField) visualElement[1]).value, out newData.col))
                newData.col = 3;
            newData.contents = new string[newData.row, newData.col];
            newData.contents.Initialize();
            VisualElement matrix = visualElement[2];

            for (int i = 0;i < newData.row; i++)
            {
                for (int j = 0;j < newData.col; j++)
                {
                    int index = i * newData.col + j;
                    newData.contents[i, j] = ((TextField) matrix[index]).value;
                }
            }

            docComponent.JsonData = JsonUtility.ToJson(newData);
            //Debug.Log(docComponent.JsonData.ToString());
            docComponent.ObjData.Clear();

            return docComponent;
        }

        private VisualElement generateEditMatrixVisual(Data data, int width)
        {
            ISPosition position = new ISPosition();
            position.Position = Position.Absolute;
            position.Left = ISStyleLength.Pixel(space);
            position.Top = ISStyleLength.Pixel(space);
            VisualElement root = new VisualElement();
            root.style.width = width;
            float childWidth = ((width - space) / data.col) - space;
            float childHeight = 30;
            for (int i = 0; i < data.row; i++)
            {
                for (int j = 0; j < data.col; j++)
                {
                    TextField textField = new TextField();
                    textField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    textField.label = "";
                    textField.multiline = true;
                    textField.value = data.contents[i, j];
                    textField.style.SetIS_Style(margin);
                    textField.style.SetIS_Style(padding);
                    textField.style.SetIS_Style(position);
                    textField.style.SetIS_Style(DocStyle.Current.MainText);
                    textField.style.width = childWidth;
                    textField.style.height = childHeight;
                    int i1 = i, j1 = j;
                    textField.RegisterValueChangedCallback(value =>
                    {
                        data.contents[i1, j1] = value.newValue;
                    });
                    position.Left += childWidth + space;
                    root.Add(textField);
                }
                position.Left = ISStyleLength.Pixel(space);
                position.Top += childHeight + space;
            }
            root.style.height = position.Top + space;
            return root;
        }

        private VisualElement generateViewMatrixVisual(Data data, int width)
        {
            ISPosition position = new ISPosition();
            space = (width * 0.01f);
            position.Position = Position.Absolute;
            position.Left = ISStyleLength.Pixel(space);
            position.Top = ISStyleLength.Pixel(0);
            VisualElement root = new VisualElement();
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.style.width = width;
            float childWidth = ((width - space) / data.col) - space;
            float childHeight = 30;
            for (int i = 0; i < data.row; i++)
            {
                position.Top += space;
                for (int j = 0; j < data.col; j++)
                {
                    Label label = new Label();
                    label.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    label.text = data.contents[i, j];
                    label.style.SetIS_Style(margin);
                    label.style.SetIS_Style(padding);
                    label.style.SetIS_Style(position);
                    label.style.SetIS_Style(DocStyle.Current.MainText);
                    label.style.width = childWidth;
                    label.style.height = childHeight;
                    position.Left += childWidth + space;
                    root.Add(label);
                }
                position.Left = ISStyleLength.Pixel(space);
                position.Top += childHeight;
            }
            root.style.height = position.Top + space;
            return root;
        }

        private string[,] resizeContent(int row, int col, string[,] oldContents)
        {
            string[,] contents = new string[row, col];
            int oldRow = oldContents.GetLength(0);
            int oldCol = oldContents.GetLength(1);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (oldRow <= i || oldCol <= j)
                    {
                        contents[i, j] = "";
                    }
                    else
                    {
                        contents[i, j] = oldContents[i, j];
                    }
                }
            }

            return contents;
        }

        [System.Serializable]
        public class Data : ISerializationCallbackReceiver
        {
            public int row, col;
            public string[,] contents;

            [SerializeField]
            private List<NestedList> serializable;

            public Data()
            {
                row = col = 3;
                contents = new string[row,col];
                contents.Initialize();
            }

            public void OnAfterDeserialize()
            {
                if (serializable == null) return;
                contents = new string[row, col];
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        contents[i, j] = serializable[i][j];
                    }
                }
            }

            public void OnBeforeSerialize()
            {
                serializable = new List<NestedList>();
                for (int i = 0; i < row; i++)
                {
                    NestedList list = new NestedList();
                    for (int j = 0; j < col; j++)
                    {
                        list.Add(contents[i, j]);
                    }
                    serializable.Add(list);
                }
            }

            [System.Serializable]
            private class NestedList
            {
                public List<string> list = new List<string>();

                public void Add(string value)
                {
                    list.Add(value);
                }

                public string this[int index]
                {
                    get
                    {
                        return list[index];
                    }
                    set
                    {
                        list[index] = value;
                    }
                }
            }
        }
    }
}
