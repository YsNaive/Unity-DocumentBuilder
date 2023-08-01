using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocStyleEditWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/DocumentBuilder/Style Editor")]
        public static void ShowWindow()
        {
            GetWindow<DocStyleEditWindow>("DocStyle Editor");
        }
        private void CreateGUI()
        {

        }
    }
}

