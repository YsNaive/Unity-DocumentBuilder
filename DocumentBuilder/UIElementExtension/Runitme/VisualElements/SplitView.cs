using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public class SplitView : VisualElement
    {
        public override VisualElement contentContainer => childContainer;
        VisualElement dragLine;
        VisualElement childContainer;
        public FlexDirection Direction
        {
            get => m_Direction;
            set
            {
                if(value == FlexDirection.Row)
                {
                    style.flexDirection = FlexDirection.Row;
                    childContainer.style.flexDirection = FlexDirection.Row;
                    dragLine.style.width = dragLineWidth;
                    dragLine.style.height = StyleKeyword.Auto;
                    dragLine.style.left = dragLine.style.top;
                    dragLine.style.top = StyleKeyword.Initial;
                }
                else
                {
                    style.flexDirection = FlexDirection.Column;
                    childContainer.style.flexDirection = FlexDirection.Column;
                    dragLine.style.width = StyleKeyword.Auto;
                    dragLine.style.height = dragLineWidth;
                    dragLine.style.top = dragLine.style.left;
                    dragLine.style.left = StyleKeyword.Initial;
                }
                m_Direction = value;
            }
        }
        FlexDirection m_Direction;
        float dragLineWidth = 3.5f;
        public float MinViewWidthPx = 50;
        float m_SplitPercent;
        public Color DragLineColor = new Color(.2f, .2f, .2f);
        public Color DragLineHoverColor = new Color(.65f, .65f, .65f);
        public float SplitPercent
        {
            get => m_SplitPercent;
            set
            {
                setDragLinePixel(percent2Position(value));
                m_SplitPercent = value;
            }
        }
        public float SplitPosition
        {
            get
            {
                var pos = percent2Position(SplitPercent);
                return (m_Direction == FlexDirection.Column) ? pos.y : pos.x;
            }
            set
            {
                setDragLinePixel(new Vector2(value, value));
            }
        }
        public SplitView() : this(FlexDirection.Row, 0) { }
        public SplitView(FlexDirection direction) : this(direction, 0) { }
        public SplitView(float initSplitPercent) : this(FlexDirection.Row, initSplitPercent) { }
        public SplitView(FlexDirection direction, float initSplitPercent)
        {
            style.flexGrow = 1f;

            initChildContainer();
            initDragLine();
                
            hierarchy.Add(childContainer);
            hierarchy.Add(dragLine);
            Direction = direction;
            m_SplitPercent = initSplitPercent;
            contentContainer.RegisterCallback<GeometryChangedEvent>(initLayout);
        }

        void initLayout(GeometryChangedEvent evt)
        {
            setContentsLayout(m_SplitPercent);
            setDragLinePercent(m_SplitPercent);
            contentContainer.UnregisterCallback<GeometryChangedEvent>(initLayout);
        }
        void initDragLine()
        {
            dragLine = new VisualElement();
            dragLine.style.backgroundColor = DragLineColor;
            dragLine.style.left = 50;

            CapturePointerManipulator manipulator = new CapturePointerManipulator();
            manipulator.ActiveMoveEvent += e =>
            {
                setDragLinePixel(e.position);
            };
            manipulator.OnHoverIN += () => { dragLine.style.backgroundColor = DragLineHoverColor; };
            manipulator.OnHoverOUT += () => { dragLine.style.backgroundColor = DragLineColor; };
            dragLine.AddManipulator(manipulator);
        }
        void initChildContainer()
        {
            childContainer = new VisualElement();
            childContainer.style.width = Length.Percent(100);
            childContainer.style.height = Length.Percent(100);
            childContainer.style.position = Position.Absolute;
        }
        Vector2 percent2Position(float percent)
        {
            Vector2 pos = new Vector2(percent, percent);
            pos /= 100f;
            pos.x *= worldBound.width;
            pos.y *= worldBound.height;
            pos.x += worldBound.x;
            pos.y += worldBound.y;
            return pos;
        }
        void setDragLinePixel(Vector2 position)
        {
            var min = worldBound.x ;
            var max = worldBound.xMax ;
            var pos = position.x;
            if (m_Direction == FlexDirection.Column) 
            {
                min = worldBound.y;
                max = worldBound.yMax;
                pos = position.y;
            }
            if ((max - min) < MinViewWidthPx * 2f) 
            {
                m_SplitPercent = 50;
            }
            else
            {
                pos = Mathf.Clamp(pos, min + MinViewWidthPx, max - MinViewWidthPx);
                m_SplitPercent = ((pos - min - dragLineWidth * 0.5f) / (max - min)) * 100f;
            }
            setDragLinePercent(m_SplitPercent);
        }
        void setDragLinePercent(float percent)
        {
            if (m_Direction == FlexDirection.Column)
                dragLine.style.top = Length.Percent(percent);
            else
                dragLine.style.left = Length.Percent(percent);
            setContentsLayout(m_SplitPercent);
        }
        void setContentsLayout(float percent)
        {
            if(childCount != 2)
            {
                Debug.LogError("DocSplitView needs 2 children.");
                return;
            }

            if(m_Direction == FlexDirection.Column)
            {
                childContainer[0].style.height = Length.Percent(percent);
                childContainer[1].style.height = Length.Percent(100f-percent);
                childContainer[1].style.borderTopWidth = dragLineWidth;
                childContainer[1].style.left = 0;
            }
            else
            {
                childContainer[0].style.width = Length.Percent(percent);
                childContainer[1].style.width = Length.Percent(100f-percent);
                childContainer[1].style.borderLeftWidth = dragLineWidth;
                childContainer[1].style.top = 0;
            }
        }
    }
}
