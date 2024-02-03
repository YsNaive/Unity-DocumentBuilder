using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public class PopupElement : VisualElement
    {
        public bool IsOpend => panel != null;
        public bool AutoClose = true;
        public event Action<IPanel> OnOpend;
        public event Action OnClosed;
        public VisualElement CoverMask;

        public PopupElement(bool autoClose = true)
        {
            CoverMask = new();
            CoverMask.style.width = Length.Percent(100);
            CoverMask.style.height = Length.Percent(100);
            CoverMask.style.position = Position.Absolute;
            CoverMask.Add(this);
            style.position = Position.Absolute;
            RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (panel == null) return;
                var self = worldBound;
                var area = CoverMask.worldBound;
                if (self.x < area.x)
                    style.left = area.x;
                if (self.y < area.y)
                    style.top = area.y;
                if (self.xMax > area.xMax)
                    style.left = area.width - self.width;
                if (self.yMax > area.yMax)
                    style.top = area.height - self.height;
            });
            CoverMask.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.target != CoverMask) return;
                if (AutoClose)
                    Close();
            });
            AutoClose = autoClose;
        }

        public void Open(VisualElement openFrom)
        {
            if (openFrom.panel == null) return;
            VisualElement root = null;
#if UNITY_EDITOR
            root = openFrom.panel.visualTree[openFrom.panel.visualTree.childCount - 1];
#else
            root = openFrom.panel.visualTree;
#endif
            root.Add(CoverMask);
            OnOpend?.Invoke(openFrom.panel);
        }
        public void Close()
        {
            if (CoverMask.parent != null)
            {
                CoverMask.parent.Remove(CoverMask);
                OnClosed?.Invoke();
            }
        }
        public void MoveBelow(VisualElement ve)
        {
            var parentBound = CoverMask.worldBound;
            style.left = ve.worldBound.x - parentBound.x;
            style.top = ve.worldBound.yMax - parentBound.y;
            style.width = ve.worldBound.width;
        }
    }

}