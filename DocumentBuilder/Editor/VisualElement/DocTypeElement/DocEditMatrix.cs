using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Charts/Matrix", 3)]
    public class DocEditMatrix : DocEditVisual<DocMatrix.Data>
    {
        [Obsolete] public override string DisplayName => "Matrix";

        public override string VisualID => "3";

        private VisualElement rowColVisual, matrixVisual, anchorVisual;

        protected override void OnCreateGUI()
        {
            this.style.backgroundColor = DocStyle.Current.BackgroundColor;
            this.style.width = -1;

            init();
            rowColVisual = generateRowCol();
            this.Add(rowColVisual);
            anchorVisual = generateAnchors();
            this.Add(anchorVisual);
            matrixVisual = generateEditMatrixVisual();
            this.Add(matrixVisual);
        }

        public override string ToMarkdown(string dstPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder align = new StringBuilder();
            align.Append("|");
            for (int i = 0;i < visualData.col; i++)
            { 
                if (visualData.anchors[i] == TextAnchor.UpperLeft ||
                    visualData.anchors[i] == TextAnchor.MiddleLeft ||
                    visualData.anchors[i] == TextAnchor.LowerLeft)
                    align.Append(":-");
                else if (visualData.anchors[i] == TextAnchor.UpperCenter ||
                    visualData.anchors[i] == TextAnchor.MiddleCenter ||
                    visualData.anchors[i] == TextAnchor.LowerCenter)
                    align.Append(":-:");
                else
                    align.Append("-:");
                align.Append("|");
            }
            for (int i = 0;i < visualData.row; i++)
            {
                if (i == 1)
                    stringBuilder.AppendLine(align.ToString());
                stringBuilder.Append("|");
                for (int j = 0;j < visualData.col; j++)
                {
                    int index = i * visualData.col + j;
                    stringBuilder.Append(Target.TextData[index]);
                    stringBuilder.Append('|');
                }
                if (i != visualData.row - 1)
                    stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        private void init()
        {
            int length = visualData.row * visualData.col;
            while (length > Target.TextData.Count)
            {
                Target.TextData.Add(string.Empty);
            }

            visualData.SetContents(Target.TextData.ToList());
        }

        private void resetTargetData()
        {
            SaveDataToTarget();
            Target.TextData.Clear();
            for (int i = 0; i < visualData.row; i++)
            {
                for (int j = 0; j < visualData.col; j++)
                {
                    Target.TextData.Add(visualData.contents[i, j]);
                }
            }
        }

        private VisualElement generateRowCol()
        {
            float percent = 100f / 3;
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            TextField rowTextField = new DSTextField("row", (value) =>
            {
                if (int.TryParse(value.newValue, out int rowNum))
                {
                    this.Remove(matrixVisual);
                    visualData.ResizeContent(rowNum, visualData.col);
                    resetTargetData();
                    matrixVisual = generateEditMatrixVisual();
                    this.Add(matrixVisual);
                }
            });
            rowTextField.value = visualData.row + "";
            rowTextField.style.width = Length.Percent(percent);

            TextField colTextField = new DSTextField("col", (value) =>
            {
                if (int.TryParse(value.newValue, out int colNum))
                {
                    this.Remove(anchorVisual);
                    this.Remove(matrixVisual);
                    visualData.ResizeContent(visualData.row, colNum);
                    resetTargetData();
                    matrixVisual = generateEditMatrixVisual();
                    anchorVisual = generateAnchors();
                    this.Add(anchorVisual);
                    this.Add(matrixVisual);
                }
            });
            colTextField.value = visualData.col.ToString();
            colTextField.style.width = Length.Percent(percent);
            DocStyle.Current.EndLabelWidth();

            var enumField = new DSEnumField<DocMatrix.Mode>("", visualData.mode, value =>
            {
                visualData.mode = value.newValue;
                SaveDataToTarget();
            });
            enumField.style.width = Length.Percent(percent);

            return new DSHorizontal(1, rowTextField, colTextField, enumField);
        }

        private VisualElement generateAnchors()
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            float percent = 89f / visualData.col;
            Button addButton;
            for (int i = 0;i < visualData.col; i++)
            {
                int i1 = i;
                addButton = new DSButton("", DocStyle.Current.SuccessColor, () =>
                {
                    visualData.AddCol(i1);
                    SaveDataToTarget();
                    ((TextField)rowColVisual[1]).value = visualData.col.ToString();
                });
                addButton.style.ClearMargin();
                addButton.style.width = Length.Percent(percent / 2);
                if (i == 0)
                {
                    addButton.style.marginLeft = Length.Percent(5);
                    addButton.style.width = Length.Percent(percent / 4);
                }
                root.Add(addButton);
                var field = new DSEnumField<TextAnchor>("", visualData.anchors[i], (value =>
                {
                    visualData.anchors[i1] = value.newValue;
                    SaveDataToTarget();
                }));
                field.style.width = Length.Percent(percent / 2);
                root.Add(field);
            }

            addButton = new DSButton("", DocStyle.Current.SuccessColor, () =>
            {
                visualData.AddCol(visualData.col);
                SaveDataToTarget();
                ((TextField)rowColVisual[1]).value = visualData.col.ToString();
            });
            addButton.style.width = Length.Percent(percent / 4);
            addButton.style.ClearMargin();
            root.Add(addButton);

            return root;
        }

        private VisualElement generateEditMatrixVisual()
        {
            VisualElement root = new VisualElement();
            float percent = 89f / visualData.col;
            Button addButton;
            for (int i = 0; i < visualData.row; i++)
            {
                VisualElement child = new DSHorizontal();
                int i1 = i;
                addButton = new DSButton("", DocStyle.Current.SuccessColor, () =>
                {
                    visualData.AddRow(i1);
                    SaveDataToTarget();
                    ((TextField)rowColVisual[0]).value = visualData.row.ToString();
                });
                addButton.style.ClearMarginPadding();
                addButton.style.width = Length.Percent(5);
                child.Add(addButton);
                for (int j = 0; j < visualData.col; j++)
                {
                    int j1 = j;
                    TextField textField = new DSTextField("", value =>
                    {
                        Target.TextData[i1 * visualData.col + j1] = value.newValue;
                        visualData.contents[i1, j1] = value.newValue;
                    });
                    textField.multiline = true;
                    textField.value = visualData.contents[i, j];
                    textField.style.width = Length.Percent(percent);
                    child.Add(textField);
                }
                Button deleteButton = new DSButton("",DocStyle.Current.DangerColor);
                deleteButton.style.ClearMarginPadding();
                deleteButton.style.width = Length.Percent(5);
                deleteButton.clicked += () =>
                {
                    visualData.DeleteRow(i1);
                    SaveDataToTarget();
                    ((TextField)rowColVisual[0]).value = visualData.row.ToString();
                };
                if (visualData.row == 1)
                    deleteButton.SetEnabled(false);
                child.Add(deleteButton);
                root.Add(child);
            }

            VisualElement deleteColButton = new DSHorizontal();

            addButton = new DSButton("", DocStyle.Current.SuccessColor, () =>
            {
                visualData.AddRow(visualData.row);
                SaveDataToTarget();
                ((TextField)rowColVisual[0]).value = visualData.row.ToString();
            });
            addButton.style.ClearMarginPadding();
            addButton.style.width = Length.Percent(5);
            deleteColButton.Add(addButton);

            for (int i = 0;i < visualData.col; i++)
            {
                int i1 = i;
                DSButton deleteButton = new DSButton("",DocStyle.Current.DangerColor, () =>
                    {
                        visualData.DeleteCol(i1);
                        SaveDataToTarget();
                        ((TextField)rowColVisual[1]).value = visualData.col.ToString();
                    });
                deleteButton.style.width = Length.Percent(percent);
                deleteButton.style.ClearMarginPadding();
                if (visualData.col == 1)
                    deleteButton.SetEnabled(false);
                deleteColButton.Add(deleteButton);
            }

            root.Add(deleteColButton);

            root.RegisterCallback<GeometryChangedEvent>(geometryChanged);
            return root;
        }

        private void geometryChanged(GeometryChangedEvent e)
        {
            ISPosition position = new ISPosition();
            float height;
            int row = matrixVisual.childCount - 1;
            for (int i = 0;i < row; i++)
            {
                height = matrixVisual[i][1].resolvedStyle.height;
                position.Top = ISStyleLength.Pixel(-height / 2);
                matrixVisual[i][0].style.SetIS_Style(position);
            }
            height = matrixVisual[row][1].resolvedStyle.height;
            position.Top = ISStyleLength.Pixel(-height / 2);
            matrixVisual[row][0].style.SetIS_Style(position);
            matrixVisual.UnregisterCallback<GeometryChangedEvent>(geometryChanged);
        }
    }
}
