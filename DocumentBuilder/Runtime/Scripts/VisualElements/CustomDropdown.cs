using NaiveAPI_UI;
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
                    LabelElement.style.width = 0;
                }
                else
                {
                    LabelElement.style.width = labelWidth;
                }
            }
        }
        protected ISLength labelWidth;
        protected T m_value;
        public CustomDropdown(string label = "")
        {
            labelWidth = DocStyle.Current.LabelWidth;
            style.ClearMarginPadding();
            LabelElement = new DocTextElement("");
            LabelElement.style.minWidth = 0;
            PopupElement = CreatePopupElement();
            var hor = DocRuntime.NewHorizontalBar(LabelElement, PopupElement);
            hor.style.flexGrow = 1;
            hor.RegisterCallback<GeometryChangedEvent>(e =>
            {
                PopupElement.style.width = e.newRect.width - LabelElement.layout.width;
            });
            Add(hor);

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
                ScrollView selectableContainer = new DocScrollView();
                selectableContainer.style.ClearMarginPadding();
                selectableContainer.style.backgroundColor = DocStyle.Current.BackgroundColor;
                selectableContainer.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, DocStyle.Current.MainTextSize / 6f));
                selectableContainer.style.left = PopupElement.worldBound.x;
                selectableContainer.style.top = PopupElement.worldBound.yMax;
                selectableContainer.style.width = PopupElement.worldBound.width;
                selectableContainer.style.maxHeight = panel.visualTree.worldBound.height - PopupElement.worldBound.yMax;
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
