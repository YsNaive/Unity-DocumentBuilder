using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class LoadScriptToDocumentWindow : EditorWindow
    {
        public List<string> scripts = new List<string>();

        static LoadScriptToDocumentWindow window;
        [MenuItem("Tools/NaiveAPI/Debug/Test")]
        public static void ShowWindow()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }
            window = CreateWindow<LoadScriptToDocumentWindow>("Debug DocVisual");
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
            //rootVisualElement.Add(DocEditor.NewObjectField<MonoScript>("", null));
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
            
            string data = File.ReadAllText($"{Application.dataPath}/DocumentBuilder/RunTime/DocComponents/DocMatrix.cs");
            //string data = File.ReadAllText($"{Application.dataPath}/DocumentBuilder/Editor/ScriptableObject/SODocPageEditor.cs");
            //string data = File.ReadAllText($"{Application.dataPath}/DocumentBuilder/Test/test.cs");
            //data = File.ReadAllText("C:\\Users\\howar\\Desktop\\Unity\\Document Builder\\Assets\\DocumentBuilder\\Editor\\ScriptableObject\\SODocPageEditor.cs");

            ScrollView scrollView = new ScrollView();
            TextElement textElement = DocRuntime.NewTextElement(DocumentBuilderParser.CSharpParser(data));
            textElement.style.width = Length.Percent(100);
            textElement.style.height = StyleKeyword.Auto;
            textElement.style.whiteSpace = WhiteSpace.Normal;
            scrollView.Add(textElement);

            rootVisualElement.Add(scrollView);
        }

    }
}