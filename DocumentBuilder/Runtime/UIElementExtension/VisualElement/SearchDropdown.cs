using NaiveAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public class SearchDropdown : VisualElement
    {
        List<string> displaying;
        List<string> allChoice;
        public TextField SearchInput;
        public DropdownField DropdownField;
        public SearchDropdown(string label = "", List<string> choice = null)
        {
            if (choice == null) { choice = new List<string>(); }
            allChoice = choice;
            displaying = new List<string>(allChoice);
            this.style.flexDirection = FlexDirection.Row;
            if (label != "")
            {
                SearchInput.label = label;
            }
            SearchInput = DocRuntime.NewTextField("", e =>
            {
                if (e.newValue == "")
                {
                    displaying = new List<string>(allChoice);
                }
                int i = 0;
                int index = 0;
                displaying.Clear();
                foreach (string str in allChoice)
                {
                    if (str.Contains(e.newValue))
                    {
                        displaying.Add(str);
                        if (DropdownField.value == str)
                            index = i;
                        i++;
                    }
                }
                DropdownField.choices = displaying;
                DropdownField.index = index;
            });
            SearchInput.style.width = Length.Percent(30);
            Add(SearchInput);
            DropdownField = DocRuntime.NewDropdownField("", displaying, e =>
            {

            });
            DropdownField.style.width = Length.Percent(80);
            DropdownField.index = 0;
            Add(DropdownField);
        }
    }
}
