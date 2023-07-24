using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI
{
    public static class DocRuntime
    {
        static DocRuntime()
        {
            Reload();
        }
        public static Dictionary<string, Type> VisualID_Dict;

        public static void Reload()
        {
            VisualID_Dict.Clear();
            Type baseType = typeof(DocVisual);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                    {
                        DocVisual doc = (DocVisual)System.Activator.CreateInstance(type);
                        VisualID_Dict.Add(doc.VisualID, type);
                    }
                }
            }
        }
        public static VisualElement CreateVisual(DocComponent docComponent) 
        {
            Type t;
            if (!VisualID_Dict.TryGetValue(docComponent.VisualID, out t))
                return new VisualElement();
            DocVisual doc = (DocVisual)System.Activator.CreateInstance(t);
            doc.Target = docComponent;
            return doc;
        }
    }

}