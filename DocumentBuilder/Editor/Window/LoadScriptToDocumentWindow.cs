using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class LoadScriptToDocumentWindow : EditorWindow
    {
        public List<string> scripts = new List<string>();

        [MenuItem("Tools/NaiveAPI/Debug/Test")]
        public static void ShowWindow()
        {
            GetWindow<LoadScriptToDocumentWindow>("Debug DocVisual");
        }

        public void CreateGUI()
        {
            /*
            DocComponent doc;
            Type type = typeof(DocEditFuncDisplay);
            MethodInfo info = type.GetMethod("LoadMethod", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            doc = DocEditFuncDisplay.LoadMethod(info);
            rootVisualElement.Add(DocRuntime.CreateVisual(doc));*/
            Type baseType = typeof(DocVisual);
            rootVisualElement.Add(DocEditor.NewObjectField<MonoScript>("", null));
            /*
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                scripts.Add(assembly.FullName);
                /*
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    scripts.Add(type.Name);
                }*//*
            }
            Debug.Log(scripts.Count);
            SearchDropdown searchDropdown = new SearchDropdown("", scripts);
            rootVisualElement.Add(searchDropdown);*/
        }
    }
}
