using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Debug;

namespace NaiveAPI_Editor.window
{
    public class StyleSheetEditorWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/StyleSheet Editor")]
        public static void ShowWindow()
        {
            GetWindow<StyleSheetEditorWindow>("StyleSheet Editor");
        }

        private void OnEnable()
        {
            minSize = new Vector2(450, 300);
            types.Clear();
            opts.Clear();
            namespaces.Clear();
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in ass.GetTypes().Where(t => t.IsSubclassOf(typeof(VisualElement))))
                {
                    if (t.Namespace != null)
                    {
                        if (t.IsGenericType) continue;
                        if (t.Namespace.Length >= 6 && t.Namespace.Substring(0, 6) == "Unity.") continue;
                        if (t.Namespace.Contains("Experimental")) continue;
                        if (t.Namespace.Contains("UnityEditor") && !t.Namespace.Contains("UIElements")) continue;
                        namespaces.Add(t.Namespace);
                    }
                    types.Add(t);
                    opts.Add(t.Name);
                }
            }
        }
        int i = 0; int ni = 0;
        VisualElement ve;
        List<Type> types = new List<Type>();
        List<string> opts = new List<string>();
        HashSet<string> namespaces = new HashSet<string>();
        private void OnGUI()
        {
        }

        private void CreateGUI()
        {
            rootVisualElement.style.SetIS_Style(ISFlex.Horizontal);
            VisualElement ver = new VisualElement();
            ver.style.SetIS_Style(ISFlex.Vertical);
            ver.style.backgroundColor = Color.black;
            ver.Add(new IMGUIContainer(imGUI));
            rootVisualElement.Add(ver);

            VisualElement ver2 = new VisualElement();
            ver2.style.SetIS_Style(new ISSize{ Height = ISStyleLength.Percent(100), Width = ISStyleLength.Percent(20) });
            VisualElement preview = new VisualElement();
            preview.style.backgroundColor= Color.white;
            preview.style.SetIS_Style(ISSize.Percent(100, 50));
            ver2.Add(preview);
            rootVisualElement.Add(ver2);

        }
        public static SerializedProperty intTest;
        private void imGUI()
        {
            ni = EditorGUILayout.Popup(i, opts.ToArray());
            if (ni != i)
            {
                i = ni;
                if(ve != null)
                    ve.parent.Remove(ve);
                ve = (VisualElement)Activator.CreateInstance(types[i]);
                rootVisualElement.Add(ve);
            }
            EditorGUILayout.Popup(0, namespaces.ToArray());

            if (intTest != null)
            {
                EditorGUILayout.PropertyField(intTest);
                intTest.serializedObject.ApplyModifiedProperties();
            }

        }
    }
}
