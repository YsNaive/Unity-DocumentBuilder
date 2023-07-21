using NaiveAPI_Editor.drawer;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace NaiveAPI_Editor.window
{
    public class ISStyleMaskCounterWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/ISStyle Mask Counter")]
        public static void ShowWindow()
        {
            GetWindow<ISStyleMaskCounterWindow>("ISStyle Mask");
        }
        private void OnEnable()
        {
            minSize = new Vector2(200, 300);

        }

        int result;
        bool[] bools = new bool[12];
        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 95;
            EditorGUILayout.IntField("Result", result);
            EditorGUILayout.Space(15);
            result = 0;
            int index = 0;
            foreach(ISStyle.ISType type in System.Enum.GetValues(typeof(ISStyle.ISType)))
            {
                if(type == ISStyle.ISType.None) { continue; }
                bools[index] = EditorGUILayout.Toggle(type.ToString(),bools[index]);
                if (bools[index])
                    result += (int)type;
                index++;
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("ALL", GUILayout.Width(45)))
            {
                for (int i = 0; i < bools.Length; i++)
                {
                    bools[i] = true;
                }
            }
            if (GUILayout.Button("None", GUILayout.Width(45)))
            {
                for (int i = 0; i < bools.Length; i++)
                {
                    bools[i] = false;
                }
            }
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            if (GUILayout.Button("Invert", GUILayout.Width(45)))
            {
                for (int i = 0; i < bools.Length; i++)
                {
                    bools[i] = !bools[i];
                }
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}
