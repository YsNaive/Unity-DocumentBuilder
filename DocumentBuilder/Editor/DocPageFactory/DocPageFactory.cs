using NaiveAPI.DocumentBuilder;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public abstract class DocPageFactory
    {
        public abstract string DisplayName { get; }
        public abstract SODocComponents InitComponents { get; }
        public abstract Texture2D InitIcon { get; }
        public abstract VisualElement CreateEditGUI();
        public abstract void AfterPageCreate(SODocPage createdPage);
        public abstract string IsValid();
        public SODocPage CreatePageAsset(SODocPage parent , string name)
        {
            var page = DocPageEditorUtils.CreatePageAsset(parent, name);
            if (InitComponents != null)
            {
                foreach (var com in InitComponents.Components)
                    page.Components.Add(com.Copy());
            }
            page.Icon = InitIcon;
            AfterPageCreate(page);
            EditorUtility.SetDirty(page);
            return page;
        }
    }
    public class DocPageFactoryField : VisualElement
    {
        static Dictionary<string, DocPageFactory> name2Factory = new();
        static List<string> choices = new();
        static DocPageFactoryField()
        {
            foreach (var type in TypeReader.FindAllTypesWhere(t =>
            {
                if (t.IsAbstract) return false;
                if(!t.IsSubclassOf(typeof(DocPageFactory))) return false;
                return true;
            })){
                DocPageFactory factory = (DocPageFactory)Activator.CreateInstance(type);
                name2Factory.Add(factory.DisplayName, factory);
                if(factory.DisplayName != "Empty")
                    choices.Add(factory.DisplayName);
            }
            choices.Insert(0, "Empty");
        }

        DSDropdown factoryDropdown =new DSDropdown("Page Factory") { choices = choices };
        VisualElement editGUIContainer = new VisualElement();
        DocPageFactory selecting;
        public DocPageFactory Selecting => selecting;
        public DocPageFactoryField()
        {
            factoryDropdown.RegisterValueChangedCallback(evt =>
            {
                selecting = null;
                editGUIContainer.Clear();
                if (name2Factory.ContainsKey(evt.newValue))
                {
                    selecting = name2Factory[evt.newValue];
                    editGUIContainer.Add(selecting.CreateEditGUI());
                }
            });

            Add(factoryDropdown);
            Add(editGUIContainer);

            factoryDropdown.value = "Empty";
            selecting = name2Factory["Empty"];
        }
    }
}