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
        private bool isHoverOnPopup = false;
        public event Action<IPanel> OnOpend;
        public event Action OnClosed;
        protected VisualElement mask;

        public PopupElement(bool autoClose = true)
        {
            mask = new();
            mask.style.width = Length.Percent(100);
            mask.style.height = Length.Percent(100);
            mask.style.position = Position.Absolute;
            mask.Add(this);
            style.position = Position.Absolute;
            RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (panel == null) return;
                var self = worldBound;
                var area = mask.worldBound;
                if (self.x < area.x)
                    style.left = area.x;
                if (self.y < area.y)
                    style.top = area.y;
                if (self.xMax > area.xMax)
                    style.left = area.width - self.width;
                if (self.yMax > area.yMax)
                    style.top = area.height - self.height;
            });
            mask.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.target != mask) return;
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
            root.Add(mask);
            OnOpend?.Invoke(openFrom.panel);
        }
        public void Close()
        {
            if (mask.parent != null)
            {
                mask.parent.Remove(mask);
                OnClosed?.Invoke();
            }
        }
        public void MoveBelow(VisualElement ve)
        {
            var parentBound = mask.worldBound;
            style.left = ve.worldBound.x - parentBound.x;
            style.top = ve.worldBound.yMax - parentBound.y;
            style.width = ve.worldBound.width;
        }
    }

}