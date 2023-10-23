using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public abstract class CustomDropdown<DType> : VisualElement, INotifyValueChanged<DType>
    {
        protected abstract VisualElement invokeDropdownElement { get; }
        public DType value
        {
            get => m_value;
            set
            {
                using (var evt = ChangeEvent<DType>.GetPooled(m_value, value))
                {
                    m_value = value;
                    this.SendEvent(evt);
                    onValueChanged();
                }
            }
        }
        protected DType m_value;

        public List<DType> choices;
        VisualElement popupContainer;
        VisualElement popupMask = new();
        public CustomDropdown()
        {
            popupContainer = createPopupContainer();
            popupContainer.style.maxHeight = Length.Percent(60);
            initLabelAndField();

            popupMask.Add(popupContainer);
            popupMask.style.position = Position.Absolute;
            popupMask.style.width = Length.Percent(100);
            popupMask.style.height = Length.Percent(100);
            popupContainer.style.position = Position.Absolute;
            popupContainer.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if(popupContainer.worldBound.yMax > popupMask.worldBound.yMax)
                    popupContainer.style.top = popupMask.worldBound.yMax - popupContainer.worldBound.height;
            });


            invokeDropdownElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (this.parent == null) return;
                if (this.panel == null) return;

                popupContainer.Clear();
                if (choices == null) return;
                foreach (var obj in choices)
                {
                    var ve = createMenuItem(obj);
                    ve.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        value = obj;
                    });
                    popupContainer.Add(ve);
                }
                this.panel.visualTree[panel.visualTree.childCount-1].Add(popupMask);
                var parentBound = popupMask.parent.worldBound;
                popupContainer.style.left = invokeDropdownElement.worldBound.x - parentBound.x;
                popupContainer.style.top = invokeDropdownElement.worldBound.yMax - parentBound.y;
                popupContainer.style.width = invokeDropdownElement.worldBound.width;
            });
            popupMask.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (popupMask.parent == null) return;
                popupMask.parent.Remove(popupMask);
            });
        }
        protected abstract void initLabelAndField();
        protected abstract void onValueChanged();
        protected abstract VisualElement createPopupContainer();
        protected abstract VisualElement createMenuItem(DType choice);

        public void SetValueWithoutNotify(DType newValue)
        {
            m_value = newValue;
        }
    }

}