using NaiveAPI.DocumentBuilder;
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
        public Color ConfirmColor = DocStyle.Current.SuccessColor;
        public Color CancelColor = DocStyle.Current.DangerColor;
        public event Action Confirm;
        public event Action Cancel;
        public Button MainBtn;
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
                Button confirm = new Button();
                confirm.style.ClearMarginPadding();
                confirm.text = ConfirmText;
                confirm.style.backgroundColor = ConfirmColor;
                confirm.style.width = Length.Percent(49);
                confirm.clicked += () =>
                {
                    Clear();
                    Add(MainBtn);
                    Confirm?.Invoke();
                };
                Add(confirm);
                Button cancel = new Button();
                cancel.style.ClearMarginPadding();
                cancel.text = CancelText;
                cancel.style.backgroundColor = CancelColor;
                cancel.style.width = Length.Percent(49);
                cancel.style.marginLeft = Length.Percent(1);
                cancel.clicked += () =>
                {
                    Clear();
                    Add(MainBtn);
                    Cancel?.Invoke();
                };
                Add(cancel);

            };
            Add(MainBtn);
        }
    }
}
