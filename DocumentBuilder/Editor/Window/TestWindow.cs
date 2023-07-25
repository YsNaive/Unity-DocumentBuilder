using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class TestWindow : EditorWindow
    {
        static TestWindow window;
        [MenuItem("Tools/NaiveAPI/Test Window")]
        public static void ShowWindow()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }
            window = CreateWindow<TestWindow>("Test Window");
        }
        private void CreateGUI()
        {
            rootVisualElement.Add(new DocPageMenuVisual(AssetDatabase.LoadAssetAtPath<SODocPage>("Assets/DocumentBuilder/Test/P1.asset"),new PageMenuHandler()));
            Button btn = new Button();
            btn.text = "Debug State";
            btn.clicked += () => {
                var handle = ((DocPageMenuVisual)rootVisualElement[0]).MenuHandler;
                Debug.Log(handle.GetState());
                handle.SetState(":1\r\n0:0\r\n00:0\r\n1:0\r\n");
            };
            rootVisualElement.Add(btn);
        }
    }
}
