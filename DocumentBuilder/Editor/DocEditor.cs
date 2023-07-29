using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
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
        public static DocEditField CreateEditVisual(DocComponent docComponent)
        {
            return new DocEditField(docComponent);
        }

        public static ObjectField NewObjectField<T>(EventCallback<ChangeEvent<UnityEngine.Object>> valueChange = null) { return NewObjectField<T>("", valueChange); }
        public static ObjectField NewObjectField<T>(string label = "", EventCallback<ChangeEvent<UnityEngine.Object>> valueChange = null)
        {
            ObjectField objectField = new ObjectField();
            objectField.style.ClearPadding();
            DocRuntime.ApplyMargin(objectField);
            objectField.objectType = typeof(T);
            if(valueChange != null)
                objectField.RegisterValueChangedCallback(valueChange);
            objectField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            objectField[0].style.ClearMarginPadding();
            if(label != "")
            {
                objectField.label = label;
                objectField[0].style.SetIS_Style(DocStyle.Current.MainText);
                objectField[0].style.ClearMarginPadding();
            }
            objectField.style.height = DocStyle.Current.LineHeight;
            return  objectField;
        }
        
        public static IntegerField NewIntField(string label, EventCallback<ChangeEvent<int>> valueChange = null)
        {
            IntegerField integerField = new IntegerField();
            integerField.style.ClearPadding();
            DocRuntime.ApplyMargin(integerField);
            integerField[0].style.ClearMarginPadding();
            integerField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            integerField[0].style.minHeight = 18;
            integerField.style.height = 18;
            if (label != "")
            {
                integerField.label = label;
                integerField[0].style.ClearMarginPadding();
                integerField[0].style.minHeight = 18;
            }
            if (valueChange != null)
                integerField.RegisterValueChangedCallback(valueChange);
            return integerField;
        }
        public static EnumField NewEnumField(string label, Enum initValue, EventCallback<ChangeEvent<Enum>> valueChange = null)
        {
            EnumField enumField = new EnumField();
            enumField.style.ClearPadding();
            DocRuntime.ApplyMargin(enumField);
            enumField.style.ClearMarginPadding();
            enumField[0].style.ClearMarginPadding();
            enumField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            enumField[0].style.minHeight = 18;
            if (label != "")
            {
                enumField.label = label;
                enumField[0].style.ClearMarginPadding();
            }
            enumField.Init(initValue);
            if(valueChange != null) 
                enumField.RegisterValueChangedCallback(valueChange);
            return enumField;
        }
    }

}