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
        public static List<string> NameList = new List<string>();
        public static Dictionary<string,string> Name2ID = new Dictionary<string,string>();
        public static Dictionary<string,string> ID2Name = new Dictionary<string,string>();
        public static Dictionary<string, Type> ID2Type = new Dictionary<string, Type>();

        public static void Reload()
        {
            ID2Type.Clear();
            Name2ID.Clear();
            ID2Name.Clear();
            NameList.Clear();
            NameList.Add("None");
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
                        NameList.Add(doc.DisplayName);
                        Name2ID.Add(doc.DisplayName, doc.VisualID);
                        ID2Type.Add(doc.VisualID, type);
                        ID2Name.Add(doc.VisualID, doc.DisplayName);
                    }
                }
            }
        }
        public static VisualElement CreateEditVisual(DocComponent docComponent)
        {
            return new DocEditField(docComponent);
        }
    }

}