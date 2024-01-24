using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridView : VisualElement
{
    public int Row, Col;
    public Color BorderColor;
    public AlignMode Mode;

    private VisualElement[,] childs;

    public GridView(int row, int col, Color borderColor, AlignMode mode)
    {
        Row = row;
        Col = col;
        BorderColor = borderColor;
        Mode = mode;
        childs = new VisualElement[Row, Col];

        for (int i = 0; i < row; i++)
        {
            VisualElement child = new VisualElement();
            child.style.flexDirection = FlexDirection.Row;
            for (int j = 0; j < col; j++)
            {
                VisualElement element = new VisualElement();
                veBorderLine(element, i, j);
                element.style.paddingLeft = 5;
                element.style.paddingRight = 5;
                child.Add(element);
                childs[i, j] = element;
            }
            base.Add(child);
        }

        this.RegisterCallback<GeometryChangedEvent>(repaintMatrix);
    }

    private void repaintMatrix(GeometryChangedEvent e)
    {
        VisualElement matrixRoot = this;
        float[] maxWidth = new float[Col];
        float[] maxHeight = new float[Row];
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
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

        for (int i = 0; i < Col; i++)
        {
            sumWidth += maxWidth[i];
            //Debug.Log("width " + maxWidth[i]);
        }

        for (int i = 0; i < Row; i++)
        {
            sumHeight += maxHeight[i];
            //Debug.Log("height " + maxHeight[i]);
        }
        float[] widthLength = new float[Col];
        float[] heightLength = new float[Row];

        for (int i = 0; i < Col; i++)
        {
            widthLength[i] = maxWidth[i] / sumWidth * 100;
        }

        for (int i = 0; i < Row; i++)
        {
            heightLength[i] = maxHeight[i] / sumHeight * 100;
        }
        sumWidth += (Col * 5);
        sumHeight += (Row * 5);

        ISPosition position = new ISPosition();
        position.Position = Position.Absolute;
        position.Left = ISStyleLength.Percent(0);
        position.Top = ISStyleLength.Percent(0);

        for (int i = 0; i < Row; i++)
        {
            matrixRoot[i].style.width = Length.Percent(100);
            matrixRoot[i].style.height = Length.Percent(heightLength[i]);
            for (int j = 0; j < Col; j++)
            {
                matrixRoot[i][j].style.SetIS_Style(position);
                matrixRoot[i][j].style.width = Length.Percent(widthLength[j]);
                position.Left += widthLength[j];
            }
            position.Left = ISStyleLength.Percent(0);
        }
        if (Mode == AlignMode.FixedWidth)
            matrixRoot.style.width = Length.Percent(100);
        else if (Mode == AlignMode.FixedContent)
            matrixRoot.style.width = sumWidth;
        matrixRoot.style.height = sumHeight;
        matrixRoot.UnregisterCallback<GeometryChangedEvent>(repaintMatrix);
    }

    private void veBorderLine(VisualElement ve, int i, int j)
    {
        ve.style.borderLeftWidth = 1;
        ve.style.borderLeftColor = BorderColor;
        ve.style.borderTopWidth = 1;
        ve.style.borderTopColor = BorderColor;
        if (i == Row - 1)
        {
            ve.style.borderBottomWidth = 1;
            ve.style.borderBottomColor = BorderColor;
        }
        if (j == Col - 1)
        {
            ve.style.borderRightWidth = 1;
            ve.style.borderRightColor = BorderColor;
        }
    }

    public new void Add(VisualElement ve)
    {
        Debug.LogError("Can not call Add in GridView. Instead use this[row, col].Add().");
    } 

    public VisualElement this[int row, int col]
    {
        get { return childs[row, col]; }
    }

    public new VisualElement this[int index]
    {
        get 
        {
            int row = index / Col;
            int col = index % Col;
            return childs[row, col]; 
        }
    }

    public new IEnumerable<VisualElement> Children()
    {
        foreach (VisualElement ve in childs)
        {
            yield return ve;
        }
    }

    public new void Clear()
    {
        foreach (VisualElement ve in Children())
        {
            ve.Clear();
        }
    }

    public enum AlignMode
    {
        FixedWidth, FixedContent
    }
}
