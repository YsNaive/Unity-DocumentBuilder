using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocMatrix : DocVisual
    {
        public override string VisualID => "3";

        private static ISMargin margin = ISMargin.None;
        private static ISPadding padding = ISPadding.None;
        private VisualElement matrixVisual;

        public override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            if (data == null) return;
            matrixVisual = generateViewMatrixVisual(data, Width);
            matrixVisual[matrixVisual.childCount - 1].RegisterCallback<GeometryChangedEvent>(repaintMatrix);
            this.Add(matrixVisual);
        }

        private VisualElement generateViewMatrixVisual(Data data, float width)
        {
            VisualElement root = new VisualElement();
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.style.width = width;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            List<string> texts = Target.TextData;
            for (int i = 0; i < data.row; i++)
            {
                VisualElement child = new VisualElement();
                child.style.backgroundColor = Color.blue;
                child.style.SetIS_Style(ISFlex.Horizontal);
                for (int j = 0; j < data.col; j++)
                {
                    Label label = new Label();
                    label.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    label.text = texts[i * data.col + j];
                    label.style.borderLeftWidth = 1;
                    label.style.borderLeftColor = DocStyle.Current.SubFrontGroundColor;
                    label.style.borderTopWidth = 1;
                    label.style.borderTopColor = DocStyle.Current.SubFrontGroundColor;
                    if (i == data.row - 1)
                    {
                        label.style.borderBottomWidth = 1;
                        label.style.borderBottomColor = DocStyle.Current.SubFrontGroundColor;
                    }
                    if (j == data.col - 1)
                    {
                        label.style.borderRightWidth = 1;
                        label.style.borderRightColor = DocStyle.Current.SubFrontGroundColor;
                    }
                    label.style.marginTop = 10;
                    label.style.marginBottom = 10;
                    label.style.unityTextAlign = data.anchors[j];
                    label.style.SetIS_Style(margin);
                    label.style.SetIS_Style(padding);
                    label.style.SetIS_Style(DocStyle.Current.MainText);
                    child.Add(label);
                }
                root.Add(child);
            }
            return root;
        }

        private void repaintMatrix(GeometryChangedEvent e)
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            VisualElement matrixRoot = this[0];
            float[] maxWidth = new float[data.col];
            float[] maxHeight = new float[data.row];
            for (int i = 0; i < data.row; i++)
            {
                for (int j = 0; j < data.col; j++)
                {
                    if (maxWidth[j] < matrixRoot[i][j].layout.width)
                    {
                        maxWidth[j] = matrixRoot[i][j].layout.width;
                    }
                    if (maxHeight[i] < matrixRoot[i][j].layout.height)
                    {
                        maxHeight[i] = matrixRoot[i][j].layout.height;
                    }
                }
            }

            float sumWidth = 0;
            float sumHeight = 0;

            for (int i = 0; i < data.col; i++)
            {
                sumWidth += maxWidth[i];
                //Debug.Log("width " + maxWidth[i]);
            }

            for (int i = 0; i < data.row; i++)
            {
                sumHeight += maxHeight[i];
                //Debug.Log("height " + maxHeight[i]);
            }
            float[] widthLength = new float[data.col];
            float[] heightLength = new float[data.row];

            for (int i = 0; i < data.col; i++)
            {
                widthLength[i] = maxWidth[i] / sumWidth * 100;
            }

            for (int i = 0; i < data.row; i++)
            {
                heightLength[i] = maxHeight[i] / sumHeight * 100;
            }
            sumHeight += (data.row * 5);

            ISPosition position = new ISPosition();
            position.Position = Position.Absolute;
            position.Left = ISStyleLength.Percent(0);
            position.Top = ISStyleLength.Percent(0);

            for (int i = 0; i < data.row; i++)
            {
                matrixRoot[i].style.width = Length.Percent(100);
                matrixRoot[i].style.height = Length.Percent(heightLength[i]);
                for (int j = 0; j < data.col; j++)
                {
                    matrixRoot[i][j].style.SetIS_Style(position);
                    matrixRoot[i][j].style.width = Length.Percent(widthLength[j]);
                    //matrixRoot[i][j].style.height = Length.Percent(heightLength[i]);
                    //Debug.Log(i + " " + j + " " + widthLength[j] + " " + heightLength[i]);
                    position.Left += widthLength[j];
                }
                position.Left = ISStyleLength.Percent(0);
            }
            matrixRoot.style.width = Length.Percent(100);
            matrixRoot.style.height = sumHeight;
            matrixRoot[matrixRoot.childCount - 1].UnregisterCallback<GeometryChangedEvent>(repaintMatrix);
        }

        [System.Serializable]
        public class Data
        {
            public int row, col;
            public string[,] contents;
            public TextAnchor[] anchors;

            public Data()
            {
                row = col = 3;
                contents = new string[row, col];
                anchors = new TextAnchor[col];
                for (int i = 0;i < col; i++)
                {
                    anchors[i] = TextAnchor.MiddleLeft;
                }
                contents.Initialize();
            }

            public void ResizeContent(int row, int col)
            {
                int oldRow = this.row;
                int oldCol = this.col;
                this.row = row;
                this.col = col;
                string[,] newContents = new string[row, col];
                if (col != oldCol)
                {
                    TextAnchor[] newAnchors = new TextAnchor[col];
                    for (int i = 0; i < col; i++)
                    {
                        if (i >= oldCol)
                            newAnchors[i] = TextAnchor.MiddleLeft;
                        else
                            newAnchors[i] = anchors[i];
                    }

                    this.anchors = newAnchors;
                }
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        if (oldRow <= i || oldCol <= j)
                        {
                            newContents[i, j] = "";
                        }
                        else
                        {
                            newContents[i, j] = contents[i, j];
                        }
                    }
                }

                this.contents = newContents;
            }
        }
    }
}
