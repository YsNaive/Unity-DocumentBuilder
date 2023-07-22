using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class DocEditVisual : VisualElement
{
    public DocComponent DocComponent;
    public DocEditVisual(DocComponent docComponent,int width)
    {
        this.DocComponent = docComponent;

        VisualElement editView = DocComponent.CreateEditGUI(width);
        DropdownField dropdownField = new DropdownField();
        dropdownField.choices = DocData.ComponentNames;
        dropdownField.index = DocData.ComponentTypeNames.FindIndex((obj) =>
        {
            return docComponent.Type == obj;
        });
        if (dropdownField.index < 0) dropdownField.index = 0;
        dropdownField.label = "Doc Type";
        dropdownField[0].style.minWidth = 70;
        dropdownField.RegisterValueChangedCallback((val) =>
        {
            DocComponent.Type = DocData.ComponentTypeNames[dropdownField.index];
            if(editView != null)
                Remove(editView);
            editView = null;
            editView = DocComponent.CreateEditGUI(width);
            Add(editView);
        });
        Add(dropdownField);
        Add(editView);
    }
}
