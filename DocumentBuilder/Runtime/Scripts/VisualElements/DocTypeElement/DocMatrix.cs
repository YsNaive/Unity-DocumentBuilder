using NaiveAPI_UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocMatrix : DocVisual<DocMatrix.Data>
    {
        public override string VisualID => "3";

        private VisualElement matrixVisual;

        protected override void OnCreateGUI()
        {
            matrixVisual = generateViewMatrixVisual(visualData, -1);
            if(matrixVisual.childCount !=0)
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
            while (Target.TextData.Count < visualData.row * visualData.col)
                Target.TextData.Add("");
            for (int i = 0; i < data.row; i++)
            {
                VisualElement child = new VisualElement();
                child.style.SetIS_Style(ISFlex.Horizontal);
                for (int j = 0; j < data.col; j++)
                {
                    TextElement label = new TextElement();
                    label.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    label.text = texts[i * data.col + j];
                    label.style.borderLeftWidth = 1;
                    label.style.borderLeftColor = DocStyle.Current.SubFrontgroundColor;
                    label.style.borderTopWidth = 1;
                    label.style.borderTopColor = DocStyle.Current.SubFrontgroundColor;
                    if (i == data.row - 1)
                    {
                        label.style.borderBottomWidth = 1;
                        label.style.borderBottomColor = DocStyle.Current.SubFrontgroundColor;
                    }
                    if (j == data.col - 1)
                    {
                        label.style.borderRightWidth = 1;
                        label.style.borderRightColor = DocStyle.Current.SubFrontgroundColor;
                    }
                    label.style.ClearMarginPadding();
                    label.style.paddingLeft = 5;
                    label.style.paddingRight = 5;
                    label.style.unityTextAlign = data.anchors[j];
                    child.Add(label);
                }
                root.Add(child);
            }
            return root;
        }

        private void repaintMatrix(GeometryChangedEvent e)
        {
            LoadJsonData();
            VisualElement matrixRoot = this[0];
            float[] maxWidth = new float[visualData.col];
            float[] maxHeight = new float[visualData.row];
            for (int i = 0; i < visualData.row; i++)
            {
                for (int j = 0; j < visualData.col; j++)
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

            for (int i = 0; i < visualData.col; i++)
            {
                sumWidth += maxWidth[i];
                //Debug.Log("width " + maxWidth[i]);
            }

            for (int i = 0; i < visualData.row; i++)
            {
                sumHeight += maxHeight[i];
                //Debug.Log("height " + maxHeight[i]);
            }
            float[] widthLength = new float[visualData.col];
            float[] heightLength = new float[visualData.row];

            for (int i = 0; i < visualData.col; i++)
            {
                widthLength[i] = maxWidth[i] / sumWidth * 100;
            }

            for (int i = 0; i < visualData.row; i++)
            {
                heightLength[i] = maxHeight[i] / sumHeight * 100;
            }
            sumWidth += (visualData.col * 5);
            sumHeight += (visualData.row * 5);

            ISPosition position = new ISPosition();
            position.Position = Position.Absolute;
            position.Left = ISStyleLength.Percent(0);
            position.Top = ISStyleLength.Percent(0);

            for (int i = 0; i < visualData.row; i++)
            {
                matrixRoot[i].style.width = Length.Percent(100);
                matrixRoot[i].style.height = Length.Percent(heightLength[i]);
                for (int j = 0; j < visualData.col; j++)
                {
                    matrixRoot[i][j].style.SetIS_Style(position);
                    matrixRoot[i][j].style.width = Length.Percent(widthLength[j]);
                    //matrixRoot[i][j].style.height = Length.Percent(heightLength[i]);
                    //Debug.Log(i + " " + j + " " + widthLength[j] + " " + heightLength[i]);
                    position.Left += widthLength[j];
                }
                position.Left = ISStyleLength.Percent(0);
            }
            if (visualData.mode == Mode.FixedWidth)
                matrixRoot.style.width = Length.Percent(100);
            else if (visualData.mode == Mode.FixedText)
                matrixRoot.style.width = sumWidth;
            matrixRoot.style.height = sumHeight;
            matrixRoot[matrixRoot.childCount - 1].UnregisterCallback<GeometryChangedEvent>(repaintMatrix);
        }

        [System.Serializable]
        public class Data
        {
            public int row = 3, col = 3;
            public string[,] contents;
            public TextAnchor[] anchors;
            public Mode mode = Mode.FixedWidth;

            public Data()
            {
                contents = new string[row, col];
                contents.Initialize();
                anchors = new TextAnchor[col];
                for (int i = 0;i < col; i++)
                {
                    anchors[i] = TextAnchor.MiddleLeft;
                }
            }

            public void SetContents(List<string> texts)
            {
                if (contents.Length != texts.Count)
                {
                    contents = new string[row, col];
                }
                int index = 0;
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        contents[i, j] = texts[index++];
                    }
                }
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

            public void DeleteRow(int row)
            {
                if (row < 0 || row >= this.row)
                    return;
                this.row--;
                string[,] newContents = new string[this.row, this.col];
                int i1 = 0;
                for (int i = 0;i < this.row; i++)
                {
                    if (i1 == row)
                        i1++;
                    for (int j = 0;j < this.col; j++)
                    {
                        //Debug.Log(i1 + " " + j);
                        newContents[i, j] = this.contents[i1, j];
                    }
                    i1++;
                }
                this.contents = newContents;
            }

            public void DeleteCol(int col)
            {
                if (col < 0 || col >= this.col)
                    return;
                this.col--;
                string[,] newContents = new string[this.row, this.col];
                int j1 = 0;
                for (int i = 0; i < this.row; i++)
                {
                    for (int j = 0; j < this.col; j++)
                    {
                        if (j1 == col)
                            j1++;
                        //Debug.Log(i + " " + j1);
                        newContents[i, j] = this.contents[i, j1];
                        j1++;
                    }
                    j1 = 0;
                }

                this.contents = newContents;
                deleteColAnchor(col);
            }

            private void deleteColAnchor(int col)
            {
                int i1 = 0;
                TextAnchor[] newAnchors = new TextAnchor[this.col];
                for (int i = 0;i < this.col; i++)
                {
                    if (i1 == col)
                        i1++;
                    newAnchors[i] = this.anchors[i1];
                    i1++;
                }

                this.anchors = newAnchors;
            }

            public void AddRow(int row)
            {
                if (row < 0 || row > this.row)
                    return;
                this.row++;
                string[,] newContents = new string[this.row, this.col];
                int i1 = 0;
                bool isDone = false;
                for (int i = 0; i < this.row; i++)
                {
                    if (i == row && !isDone)
                    {
                        i1--;
                        for (int j = 0; j < this.col; j++)
                        {
                            newContents[i, j] = "";
                        }
                        isDone = true;
                    }
                    else
                    {
                        for (int j = 0; j < this.col; j++)
                        {
                            //Debug.Log(i1 + " " + j);
                            newContents[i, j] = this.contents[i1, j];
                        }
                    }
                    i1++;
                }
                this.contents = newContents;
            }

            public void AddCol(int col)
            {
                if (col < 0 || col > this.col)
                    return;
                this.col++;
                string[,] newContents = new string[this.row, this.col];
                int j1 = 0;
                bool isDone;
                for (int i = 0; i < this.row; i++)
                {
                    isDone = false;
                    for (int j = 0; j < this.col; j++)
                    {
                        if (j1 == col && !isDone)
                        {
                            j1--;
                            newContents[i, j] = "";
                            isDone = true;
                        }
                        else
                        {
                            //Debug.Log(i + " " + j1);
                            newContents[i, j] = this.contents[i, j1];
                        }
                        j1++;
                    }
                    j1 = 0;
                }
                this.contents = newContents;
                addColAnchor(col);
            }

            private void addColAnchor(int col)
            {
                int i1 = 0;
                TextAnchor[] newAnchors = new TextAnchor[this.col];
                bool isDone = false;
                for (int i = 0; i < this.col; i++)
                {
                    if (i1 == col && !isDone)
                    {
                        i1--;
                        newAnchors[i] = TextAnchor.MiddleLeft;
                        isDone = true;
                    }
                    else
                    {
                        newAnchors[i] = this.anchors[i1];
                    }
                    i1++;
                }

                this.anchors = newAnchors;
            }
        }

        public enum Mode
        {
            FixedWidth, FixedText
        }
    }
}
