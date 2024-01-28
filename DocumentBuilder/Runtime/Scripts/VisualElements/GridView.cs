using NaiveAPI_UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
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
                    element.style.position = Position.Absolute;
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

        public void ResolveLayout(GeometryChangedEvent e)
        {
            ResolveLayout();
        }
        public void ResolveLayout()
        {
            freeForExpand();
        }

        private void freeForExpand()
        {
            if (Mode != AlignMode.FixedWidth)
                this.style.width = StyleKeyword.Auto;
            this.style.height = StyleKeyword.Auto;
            for (int i = 0; i < Row; i++)
            {
                //base[i].style.width = StyleKeyword.Auto;
                base[i].style.height = StyleKeyword.Auto;
                for (int j = 0; j < Col; j++)
                {
                    childs[i, j].style.top = StyleKeyword.Auto;
                    childs[i, j].style.right = StyleKeyword.Auto;
                    childs[i, j].style.left = StyleKeyword.Auto;
                    childs[i, j].style.bottom = StyleKeyword.Auto;
                    if (Mode != AlignMode.FixedWidth)
                        childs[i, j].style.width = StyleKeyword.Auto;
                    childs[i, j].style.height = StyleKeyword.Auto;
                }
            }
            this.RegisterCallback<GeometryChangedEvent>(repaintMatrix);
        }

        private void repaintMatrix(GeometryChangedEvent e)
        {
            this.UnregisterCallback<GeometryChangedEvent>(repaintMatrix);

            float[] maxWidth = new float[Col];
            float[] maxHeight = new float[Row];
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    if (maxWidth[j] < childs[i, j].layout.width + 2)
                    {
                        maxWidth[j] = childs[i, j].layout.width + 2;
                    }
                    if (maxHeight[i] < childs[i, j].layout.height + 2)
                    {
                        maxHeight[i] = childs[i, j].layout.height + 2;
                    }
                }
            }

            float sumWidth = 0;
            float sumHeight = 0;

            for (int i = 0; i < Col; i++)
            {
                sumWidth += maxWidth[i];
            }

            for (int i = 0; i < Row; i++)
            {
                sumHeight += maxHeight[i];
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
            //sumWidth += (Col * 5);
            //sumHeight += (Row * 5);

            ISPosition position = new ISPosition();
            position.Position = Position.Absolute;
            position.Left = ISStyleLength.Percent(0);
            position.Top = ISStyleLength.Percent(0);

            for (int i = 0; i < Row; i++)
            {
                base[i].style.width = Length.Percent(100);
                base[i].style.height = Length.Percent(heightLength[i]);
                for (int j = 0; j < Col; j++)
                {
                    childs[i, j].style.SetIS_Style(position);
                    childs[i, j].style.width = Length.Percent(widthLength[j]);
                    position.Left += widthLength[j];
                }
                position.Left = ISStyleLength.Percent(0);
            }
            this.style.width = Mode switch
            {
                AlignMode.FixedWidth => Length.Percent(100),
                AlignMode.FixedContent => sumWidth,
                _ => throw new System.NotImplementedException(),
            };
            this.style.height = sumHeight;
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
}