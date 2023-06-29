using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DocumentBuilder
{
    [Serializable]
    public class MatrixComponent : DocComponent
    {
        public override VisualElement CreateEditorGUI()
        {
            throw new System.NotImplementedException();
        }

        public override VisualElement CreateRuntimeGUI()
        {
            MatrixVisual output = new MatrixVisual(this);
            return output;
        }
    }

    public class MatrixVisual : VisualElement
    {
        public int row, column;
        public List<VisualElement> rowChildren = new List<VisualElement>();
        ISBorder elementBorder = new ISBorder { RightColor = Color.red, Right = 2, BottomColor = Color.red, Bottom = 2 };
        public MatrixVisual(MatrixComponent data)
        {
            this.style.SetIS_Style(new ISBorder(Color.red, 2));
            row = int.Parse(data.StrData[0]);
            column = int.Parse(data.StrData[1]);
            for (int i = 0;i < row;i++)
            {
                VisualElement rowChild = new VisualElement();
                rowChild.style.SetIS_Style(ISFlex.Horizontal);
                string[] strData = data.StrData[i + 2].Split("%SPACE%");
                for (int j = 0;j < column;j++)
                {
                    TextElement textElement = new TextElement();
                    textElement.style.SetIS_Style(elementBorder);
                    textElement.style.width = new Length(100 / (column == 0 ? 1 : column), LengthUnit.Percent);
                    textElement.text = strData[j];
                    rowChild.Add(textElement);
                }
                rowChildren.Add(rowChild);
                this.Add(rowChild);
            }
        }
    }
}
