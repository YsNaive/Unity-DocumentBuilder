using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public abstract class CustomDropdown<T> : VisualElement
    {
        public TextElement LabelElement;
        public VisualElement PopupElement;
        public List<T> Choices;
        public virtual T Value 
        { 
            get => m_value;
            set
            {
                m_value = value;
                OnValueChanged?.Invoke(value);
                OnSelectChanged(value);
            }
        }
        public virtual int Index
        {
            get => Choices.IndexOf(m_value);
            set => Value = Choices[value];
        }

        public virtual string Label
        {
            get => LabelElement.text;
            set
            {
                LabelElement.text = value;
                if (value == "")
                {
                    LabelElement.style.minWidth = 0;
                    PopupElement.style.minWidth = Length.Percent(100);
                }
                else
                {
                    LabelElement.style.minWidth = DocStyle.Current.LabelWidth;
                    PopupElement.style.minWidth = DocStyle.Current.ContentWidth(PopupElement);
                }
            }
        }

        protected T m_value;
        public CustomDropdown(string label = "")
        {
            style.flexDirection = FlexDirection.Row;
            LabelElement = DocRuntime.NewTextElement("");
            PopupElement = CreatePopupElement();
            Add(LabelElement);
            Add(PopupElement);

            EventCallback<GeometryChangedEvent> createLabel = null;
            createLabel = e =>
            {
                Label = label;
                UnregisterCallback(createLabel);
            };
            RegisterCallback(createLabel);
            PopupElement.RegisterCallback<PointerDownEvent>(e =>
            {
                if (panel == null) return;
                VisualElement tempCover = new VisualElement();
                tempCover.style.width = Length.Percent(100);
                tempCover.style.height = Length.Percent(100);
                panel.visualTree.Add(tempCover);
                ScrollView selectableContainer = DocRuntime.NewScrollView();
                DocRuntime.ApplyMarginPadding(selectableContainer);
                selectableContainer.style.backgroundColor = DocStyle.Current.BackgroundColor;
                selectableContainer.style.left = PopupElement.worldBound.x;
                selectableContainer.style.top = PopupElement.worldBound.yMax;
                selectableContainer.style.width = PopupElement.worldBound.width;
                Action close = () => { 
                    if(panel.visualTree.Contains(tempCover))
                        panel.visualTree.Remove(tempCover); 
                };
                int i = 0;
                foreach(T val in Choices)
                {
                    VisualElement selectable = CreateSelectableItem(val, i);
                    selectable.RegisterCallback<PointerDownEvent>(e =>
                    {
                        Value = val;
                        close();
                    });
                    selectableContainer.Add(selectable);
                    i++;
                }
                tempCover.Add(selectableContainer);
                tempCover.RegisterCallback<PointerDownEvent>(e => { close(); });
            });

        }

        public event Action<T> OnValueChanged;
        public abstract VisualElement CreatePopupElement();
        public abstract VisualElement CreateSelectableItem(T choice,int i);
        public abstract void OnSelectChanged(T newValue);
    }
}
