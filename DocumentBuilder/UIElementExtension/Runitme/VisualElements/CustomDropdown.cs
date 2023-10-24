using System.Collections;
using System.Collections.Generic;
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
        PopupElement popup = new PopupElement();
        VisualElement popupContainer;
        public CustomDropdown()
        {
            popupContainer = createPopupContainer();
            popup.Add(popupContainer);
            initLabelAndField();
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
                popup.Open(this);
                popup.MoveBelow(invokeDropdownElement);
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