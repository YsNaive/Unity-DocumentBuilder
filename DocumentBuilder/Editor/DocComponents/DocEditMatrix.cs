using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.UIElements;
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
            rowColVisual = generateRowCol(visualData);
            this.Add(rowColVisual);
            anchorVisual = generateAnchors(visualData);
            this.Add(anchorVisual);
            matrixVisual = generateEditMatrixVisual(visualData, -1);
            this.Add(matrixVisual);
        }

        public override string ToMarkdown(string dstPath)
        {
            DocMatrix.Data data = JsonUtility.FromJson<DocMatrix.Data>(Target.JsonData);
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder align = new StringBuilder();
            align.Append("|");
            for (int i = 0;i < data.col; i++)
            { 
                if (data.anchors[i] == TextAnchor.UpperLeft ||
                    data.anchors[i] == TextAnchor.MiddleLeft ||
                    data.anchors[i] == TextAnchor.LowerLeft)
                    align.Append(":-");
                else if (data.anchors[i] == TextAnchor.UpperCenter ||
                    data.anchors[i] == TextAnchor.MiddleCenter ||
                    data.anchors[i] == TextAnchor.LowerCenter)
                    align.Append(":-:");
                else
                    align.Append("-:");
                align.Append("|");
            }
            for (int i = 0;i < data.row; i++)
            {
                if (i == 1)
                    stringBuilder.Append(align.ToString()).AppendLine();
                stringBuilder.Append("|");
                for (int j = 0;j < data.col; j++)
                {
                    int index = i * data.col + j;
                    stringBuilder.Append(Target.TextData[index]);
                    stringBuilder.Append('|');
                }
                if (i != data.row - 1)
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

            visualData.SetContents(Target.TextData);
        }

        private void resetTargetData(DocMatrix.Data data)
        {
            Target.JsonData = JsonUtility.ToJson(data);
            Target.TextData.Clear();
            for (int i = 0; i < data.row; i++)
            {
                for (int j = 0; j < data.col; j++)
                {
                    Target.TextData.Add(data.contents[i, j]);
                }
            }
        }

        private VisualElement generateRowCol(DocMatrix.Data data)
        {
            float percent = 100f / 3;
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            TextField rowTextField = new DocTextField("row", (value) =>
            {
                if (int.TryParse(value.newValue, out int rowNum))
                {
                    this.Remove(matrixVisual);
                    data.ResizeContent(rowNum, data.col);
                    resetTargetData(data);
                    matrixVisual = generateEditMatrixVisual(data, -1);
                    this.Add(matrixVisual);
                }
            });
            rowTextField.value = data.row + "";
            rowTextField.style.width = Length.Percent(percent);

            TextField colTextField = new DocTextField("col", (value) =>
            {
                if (int.TryParse(value.newValue, out int colNum))
                {
                    this.Remove(anchorVisual);
                    this.Remove(matrixVisual);
                    data.ResizeContent(data.row, colNum);
                    resetTargetData(data);
                    matrixVisual = generateEditMatrixVisual(data, -1);
                    anchorVisual = generateAnchors(data);
                    this.Add(anchorVisual);
                    this.Add(matrixVisual);
                }
            });
            colTextField.value = data.col + "";
            colTextField.style.width = Length.Percent(percent);
            DocStyle.Current.EndLabelWidth();

            EnumField enumField = DocEditor.NewEnumField("", data.mode, value =>
            {
                data.mode = (DocMatrix.Mode)value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            enumField.style.width = Length.Percent(percent);

            return DocRuntime.NewHorizontalBar(1, rowTextField, colTextField, enumField);
        }

        private VisualElement generateAnchors(DocMatrix.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            float percent = 89f / data.col;
            Button addButton;
            for (int i = 0;i < data.col; i++)
            {
                int i1 = i;
                addButton = DocRuntime.NewButton("", DocStyle.Current.SuccessColor, () =>
                {
                    data.AddCol(i1);
                    Target.JsonData = JsonUtility.ToJson(data);
                    ((TextField)rowColVisual[1]).value = data.col + "";
                });
                addButton.style.width = Length.Percent(percent / 2);
                if (i == 0)
                {
                    addButton.style.marginLeft = Length.Percent(5);
                    addButton.style.width = Length.Percent(percent / 4);
                }
                root.Add(addButton);
                EnumField field = DocEditor.NewEnumField("", data.anchors[i], (value =>
                {
                    data.anchors[i1] = (TextAnchor)value.newValue;
                    Target.JsonData = JsonUtility.ToJson(data);
                }));
                field.style.width = Length.Percent(percent / 2);
                root.Add(field);
            }

            addButton = DocRuntime.NewButton("", DocStyle.Current.SuccessColor, () =>
            {
                data.AddCol(data.col);
                Target.JsonData = JsonUtility.ToJson(data);
                ((TextField)rowColVisual[1]).value = data.col + "";
            });
            addButton.style.width = Length.Percent(percent / 4);
            root.Add(addButton);

            return root;
        }

        private VisualElement generateEditMatrixVisual(DocMatrix.Data data, float width)
        {
            VisualElement root = new VisualElement();
            root.style.width = width;
            float percent = 89f / data.col;
            Button addButton;
            for (int i = 0; i < data.row; i++)
            {
                VisualElement child = new VisualElement();
                child.style.SetIS_Style(ISFlex.Horizontal);
                int i1 = i;
                addButton = DocRuntime.NewButton("", DocStyle.Current.SuccessColor, () =>
                {
                    data.AddRow(i1);
                    Target.JsonData = JsonUtility.ToJson(data);
                    ((TextField)rowColVisual[0]).value = data.row + "";
                });
                addButton.style.ClearMarginPadding();
                addButton.style.width = Length.Percent(5);
                child.Add(addButton);
                for (int j = 0; j < data.col; j++)
                {
                    int j1 = j;
                    TextField textField = new DocTextField("", value =>
                    {
                        Target.TextData[i1 * data.col + j1] = value.newValue;
                        data.contents[i1, j1] = value.newValue;
                    });
                    textField.multiline = true;
                    textField.value = data.contents[i, j];
                    textField.style.width = Length.Percent(percent);
                    child.Add(textField);
                }
                Button deleteButton = new Button();
                deleteButton.style.SetIS_Style(ISMargin.None);
                deleteButton.style.SetIS_Style(ISPadding.None);
                deleteButton.style.backgroundColor = DocStyle.Current.DangerColor;
                deleteButton.style.width = Length.Percent(5);
                deleteButton.clicked += () =>
                {
                    data.DeleteRow(i1);
                    Target.JsonData = JsonUtility.ToJson(data);
                    ((TextField)rowColVisual[0]).value = data.row + "";
                };
                child.Add(deleteButton);
                root.Add(child);
            }

            VisualElement deleteColButton = new VisualElement();
            deleteColButton.style.SetIS_Style(ISFlex.Horizontal);

            addButton = DocRuntime.NewButton("", DocStyle.Current.SuccessColor, () =>
            {
                data.AddRow(data.row);
                Target.JsonData = JsonUtility.ToJson(data);
                ((TextField)rowColVisual[0]).value = data.row + "";
            });
            addButton.style.ClearMarginPadding();
            addButton.style.width = Length.Percent(5);
            deleteColButton.Add(addButton);

            for (int i = 0;i < data.col; i++)
            {
                int i1 = i;
                Button deleteButton = DocRuntime.NewButton(DocStyle.Current.DangerColor, () =>
                {
                    data.DeleteCol(i1);
                    Target.JsonData = JsonUtility.ToJson(data);
                    ((TextField)rowColVisual[1]).value = data.col + "";
                });
                deleteButton.style.width = Length.Percent(percent);
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
