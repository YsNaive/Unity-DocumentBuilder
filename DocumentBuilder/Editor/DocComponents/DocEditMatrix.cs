using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditMatrix : DocEditVisual
    {
        public override string DisplayName => "Matrix";

        public override string VisualID => "3";

        private VisualElement rowColVisual, matrixVisual, anchorVisual;

        public override void OnCreateGUI()
        {
            this.style.backgroundColor = DocStyle.Current.BackgroundColor;
            this.style.width = Width;
            DocMatrix.Data data = setData(Target.JsonData, Target.TextData);

            rowColVisual = generateRowCol(data);
            this.Add(rowColVisual);
            anchorVisual = generateAnchors(data);
            this.Add(anchorVisual);
            matrixVisual = generateEditMatrixVisual(data, Width);
            this.Add(matrixVisual);
        }

        private DocMatrix.Data setData(string jsonData, List<string> texts)
        {
            DocMatrix.Data data = JsonUtility.FromJson<DocMatrix.Data>(jsonData);
            if (data == null)
                data = new DocMatrix.Data();
            Target.JsonData = JsonUtility.ToJson(data);
            data.contents = new string[data.row, data.col];
            for (int i = 0;i < data.row; i++)
            {
                for (int j = 0;j < data.col; j++)
                {
                    int index = i * data.col + j;
                    if (texts.Count > index)
                        data.contents[i, j] = texts[index];
                    else
                    {
                        data.contents[i, j] = string.Empty;
                        Target.TextData.Add(string.Empty);
                    }
                }
            }

            return data;
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
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            float percent = 100f / 3;
            TextField rowTextField = new TextField();
            rowTextField.label = "row";
            rowTextField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            rowTextField.value = data.row + "";
            rowTextField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            rowTextField.style.SetIS_Style(DocStyle.Current.MainText);
            rowTextField.style.SetIS_Style(ISMargin.None);
            rowTextField.style.SetIS_Style(ISPadding.Percent(5));
            rowTextField.style.width = Length.Percent(percent);
            rowTextField.RegisterValueChangedCallback((value) =>
            {
                int rowNum;
                if (int.TryParse(value.newValue, out rowNum))
                {
                    this.Remove(matrixVisual);
                    data.ResizeContent(rowNum, data.col);
                    resetTargetData(data);
                    matrixVisual = generateEditMatrixVisual(data, Width);
                    this.Add(matrixVisual);
                }
            });
            root.Add(rowTextField);

            TextField colTextField = new TextField();
            colTextField.label = "col";
            colTextField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            colTextField.value = data.col + "";
            colTextField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            colTextField.style.SetIS_Style(DocStyle.Current.MainText);
            colTextField.style.SetIS_Style(ISMargin.None);
            colTextField.style.SetIS_Style(ISPadding.Percent(5));
            colTextField.style.width = Length.Percent(percent);
            colTextField.RegisterValueChangedCallback((value) =>
            {
                int colNum;
                if (int.TryParse(value.newValue, out colNum))
                {
                    this.Remove(anchorVisual);
                    this.Remove(matrixVisual);
                    data.ResizeContent(data.row, colNum);
                    resetTargetData(data);
                    matrixVisual = generateEditMatrixVisual(data, Width);
                    anchorVisual = generateAnchors(data);
                    this.Add(anchorVisual);
                    this.Add(matrixVisual);
                }
            });
            root.Add(colTextField);
            EnumField enumField = new EnumField();
            enumField.Init(DocMatrix.Mode.FixedWidth);
            enumField.style.SetIS_Style(ISMargin.None);
            enumField.style.SetIS_Style(ISPadding.Percent(5));
            enumField.style.width = Length.Percent(percent);
            enumField.value = data.mode;
            enumField.RegisterValueChangedCallback(value =>
            {
                data.mode = (DocMatrix.Mode) value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            root.Add(enumField);
            return root;
        }

        private VisualElement generateAnchors(DocMatrix.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            float percent = 95f / data.col;
            for (int i = 0;i < data.col; i++)
            {
                EnumField field = new EnumField();
                field.Init(TextAnchor.MiddleLeft);
                field.value = data.anchors[i];
                int i1 = i;
                field.RegisterValueChangedCallback(value =>
                {
                    data.anchors[i1] = (TextAnchor) value.newValue;
                    Target.JsonData = JsonUtility.ToJson(data);
                });
                field.style.SetIS_Style(ISMargin.None);
                field.style.SetIS_Style(ISPadding.None);
                field.style.width = Length.Percent(percent);
                root.Add(field);
            }

            return root;
        }

        private VisualElement generateEditMatrixVisual(DocMatrix.Data data, float width)
        {
            VisualElement root = new VisualElement();
            root.style.width = width;
            float percent = 95f / data.col;
            for (int i = 0; i < data.row; i++)
            {
                VisualElement child = new VisualElement();
                child.style.SetIS_Style(ISFlex.Horizontal);
                int i1 = i;
                for (int j = 0; j < data.col; j++)
                {
                    TextField textField = new TextField();
                    textField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    textField.label = "";
                    textField.multiline = true;
                    textField.value = data.contents[i, j];
                    textField.style.width = Length.Percent(percent);
                    textField.style.SetIS_Style(ISMargin.None);
                    textField.style.SetIS_Style(ISPadding.None);
                    textField.style.SetIS_Style(DocStyle.Current.MainText);
                    int j1 = j;
                    textField.RegisterValueChangedCallback(value =>
                    {
                        Target.TextData[i1 * data.col + j1] = value.newValue;
                        data.contents[i1, j1] = value.newValue;
                    });
                    child.Add(textField);
                }
                Button deleteButton = new Button();
                deleteButton.style.SetIS_Style(ISMargin.None);
                deleteButton.style.SetIS_Style(ISPadding.None);
                deleteButton.style.width = Length.Percent(5);
                deleteButton.clicked += () =>
                {
                    data.DeleteRow(i1);
                    Target.JsonData = JsonUtility.ToJson(data);
                    resetTargetData(data);
                    ((TextField)rowColVisual[0]).value = data.row + "";
                };
                child.Add(deleteButton);
                root.Add(child);
            }

            VisualElement deleteColButton = new VisualElement();
            deleteColButton.style.SetIS_Style(ISFlex.Horizontal);
            for (int i = 0;i < data.col; i++)
            {
                Button deleteButton = new Button();
                int i1 = i;
                deleteButton.clicked += () =>
                {
                    data.DeleteCol(i1);
                    Target.JsonData = JsonUtility.ToJson(data);
                    resetTargetData(data);
                    ((TextField)rowColVisual[1]).value = data.col + "";
                };
                deleteButton.style.SetIS_Style(ISMargin.None);
                deleteButton.style.SetIS_Style(ISPadding.None);
                deleteButton.style.width = Length.Percent(percent);
                deleteColButton.Add(deleteButton);
            }

            root.Add(deleteColButton);
            return root;
        }
    }
}