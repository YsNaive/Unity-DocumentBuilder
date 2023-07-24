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

        private static ISMargin margin = ISMargin.None;
        private static ISPadding padding = ISPadding.None;
        private float space = 1;
        private VisualElement matrixVisual, anchorVisual;

        public override void OnCreateGUI()
        {
            this.style.backgroundColor = DocStyle.Current.BackgroundColor;
            this.style.width = Width;
            DocMatrix.Data data = setData(Target.JsonData, Target.TextData);
            TextField rowTextField = new TextField();
            rowTextField.label = "row";
            rowTextField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            rowTextField.value = data.row + "";
            rowTextField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            rowTextField.style.SetIS_Style(DocStyle.Current.MainText);
            rowTextField.style.SetIS_Style(margin);
            rowTextField.style.SetIS_Style(ISPadding.Percent((int)space));
            rowTextField.style.width = new Length(100 - 2 * space, LengthUnit.Percent);
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
            this.Add(rowTextField);

            TextField colTextField = new TextField();
            colTextField.label = "col";
            colTextField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            colTextField.value = data.col + "";
            colTextField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            colTextField.style.SetIS_Style(DocStyle.Current.MainText);
            colTextField.style.SetIS_Style(margin);
            colTextField.style.SetIS_Style(ISPadding.Percent((int)space));
            colTextField.style.width = new Length(100 - 2 * space, LengthUnit.Percent);
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
            this.Add(colTextField);
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

        private VisualElement generateAnchors(DocMatrix.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            for (int i = 0;i < data.col; i++)
            {
                EnumField field = new EnumField();
                field.Init(TextAnchor.MiddleLeft);
                Debug.Log(i);
                field.value = data.anchors[i];
                int i1 = i;
                field.RegisterValueChangedCallback(value =>
                {
                    data.anchors[i1] = (TextAnchor) value.newValue;
                    Target.JsonData = JsonUtility.ToJson(data);
                });
                field.style.SetIS_Style(margin);
                field.style.SetIS_Style(padding);
                field.style.width = Length.Percent(100f /  data.col);
                root.Add(field);
            }

            return root;
        }

        private VisualElement generateEditMatrixVisual(DocMatrix.Data data, float width)
        {
            ISPosition position = new ISPosition();
            position.Position = Position.Relative;
            position.Left = ISStyleLength.Percent(space);
            position.Top = ISStyleLength.Percent(space);
            VisualElement root = new VisualElement();
            root.style.width = width;
            float childWidth = ((width - space) / data.col) - space;
            float percent = 100f / data.col;
            for (int i = 0; i < data.row; i++)
            {
                VisualElement child = new VisualElement();
                child.style.SetIS_Style(ISFlex.Horizontal);
                for (int j = 0; j < data.col; j++)
                {
                    TextField textField = new TextField();
                    textField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    textField.label = "";
                    textField.multiline = true;
                    textField.value = data.contents[i, j];
                    textField.style.width = Length.Percent(percent);
                    textField.style.SetIS_Style(margin);
                    textField.style.SetIS_Style(padding);
                    textField.style.SetIS_Style(DocStyle.Current.MainText);
                    int i1 = i, j1 = j;
                    textField.RegisterValueChangedCallback(value =>
                    {
                        Target.TextData[i1 * data.col + j1] = value.newValue;
                        data.contents[i1, j1] = value.newValue;
                    });
                    child.Add(textField);
                }
                root.Add(child);
            }
            return root;
        }
    }
}
