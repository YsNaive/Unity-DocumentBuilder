using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public static class DocEditor
    {
        static DocEditor()
        {
            Reload();
        }
        public static List<string> VisualNameList = new List<string>();
        public static Dictionary<string,string> VisualName2ID = new Dictionary<string,string>();
        public static Dictionary<string, Type> VisualID2Type = new Dictionary<string, Type>();

        public static void Reload()
        {
            VisualID2Type.Clear();
            VisualName2ID.Clear();
            VisualNameList.Clear();
            VisualNameList.Add("None");
            Type baseType = typeof(DocEditVisual);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                    {
                        DocEditVisual doc = (DocEditVisual)System.Activator.CreateInstance(type);
                        VisualName2ID.Add(doc.DisplayName, doc.VisualID);
                        VisualID2Type.Add(doc.VisualID, type);
                        VisualNameList.Add(doc.DisplayName);
                    }
                }
            }
        }
        public static VisualElement CreateEditVisual(DocComponent docComponent)
        {
            VisualElement root = new VisualElement();
            DropdownField dropdown = new DropdownField();
            dropdown.label = "Type";
            dropdown.choices = VisualNameList;
            dropdown.index = VisualNameList.FindIndex(0, (str) => { return str == docComponent.VisualID; });
            dropdown.RegisterValueChangedCallback((val) =>
            {
                docComponent.VisualID = val.newValue;
                if (val.previousValue != "None") root.RemoveAt(1);
                if (val.newValue == "None") return;
                DocEditVisual doc = (DocEditVisual)System.Activator.CreateInstance(VisualID2Type[VisualName2ID[val.newValue]]);
                doc.Target = docComponent;
                root.Add(doc);
            });
            root.Add(dropdown);
            Type t;
            if (!VisualID2Type.TryGetValue(docComponent.VisualID, out t))
                root.Add(new VisualElement());
            else
            {
                DocEditVisual doc = (DocEditVisual)System.Activator.CreateInstance(t);
                doc.Target = docComponent;
                root.Add(doc);
            }
            return root;
        }
    }

}