using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class DocEditView : VisualElement
{
    static List<string> ComponentNameList = new List<string>();
    static List<string> ComponentTypeStrList = new List<string>();

    public DocComponent DocComponent;
    public DocEditView(DocComponent docComponent,int width)
    {
        this.DocComponent = docComponent;
        if(ComponentNameList.Count <= 0) ReloadComponentList();

        Button reloadBtn = new Button();
        reloadBtn.clicked+=() =>
        {
            ReloadComponentList();
            Debug.Log(Screen.width);
        };
        reloadBtn.text = "Reload Choices";
        VisualElement editView = DocComponent.CreateEditGUI(width);
        DropdownField dropdownField = new DropdownField();
        dropdownField.choices = ComponentNameList;
        dropdownField.index = 0;
        dropdownField.label = "Doc Type";
        dropdownField.RegisterValueChangedCallback((val) =>
        {
            DocComponent.Type = ComponentTypeStrList[dropdownField.index];
            Remove(editView);
            editView = null;
            editView = DocComponent.CreateEditGUI(width);
            Add(editView);
        });
        Add(reloadBtn);
        Add(dropdownField);
        Add(editView);
    }

    static void ReloadComponentList()
    {
        ComponentNameList.Clear();
        ComponentTypeStrList.Clear();
        Type baseType = typeof(DocVisual);
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {       
                if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                {
                    ComponentNameList.Add(type.Name);
                    ComponentTypeStrList.Add(type.AssemblyQualifiedName);
                }
            }
        }
    }
}
