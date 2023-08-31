using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public class CheckButton : VisualElement
    {
        public string ConfirmText = "YES";
        public string CancelText = "NO";
        public Color ConfirmColor = Color.green;
        public Color CancelColor = Color.red;
        public event Action Confirm;
        public event Action Cancel;
        public Button MainBtn;
        public Button ConfirmButton;
        public Button CancelButton;
        public string text
        {
            get { return MainBtn.text; }
            set { MainBtn.text = value; }
        }
        public CheckButton() {
            style.SetIS_Style(ISFlex.Horizontal);
            MainBtn = new Button();
            MainBtn.style.width = Length.Percent(100);
            MainBtn.clicked += () =>
            {
                Clear();
                Add(ConfirmButton);
                Add(CancelButton);

            };
            ConfirmButton = new Button();
            ConfirmButton.style.ClearMarginPadding();
            ConfirmButton.text = ConfirmText;
            ConfirmButton.style.backgroundColor = ConfirmColor;
            ConfirmButton.style.width = Length.Percent(49);
            ConfirmButton.clicked += () =>
            {
                Clear();
                Add(MainBtn);
                Confirm?.Invoke();
            };
            CancelButton = new Button();
            CancelButton.style.ClearMarginPadding();
            CancelButton.text = CancelText;
            CancelButton.style.backgroundColor = CancelColor;
            CancelButton.style.width = Length.Percent(49);
            CancelButton.style.marginLeft = Length.Percent(1);
            CancelButton.clicked += () =>
            {
                Clear();
                Add(MainBtn);
                Cancel?.Invoke();
            };
            Add(MainBtn);
        }
    }
}
