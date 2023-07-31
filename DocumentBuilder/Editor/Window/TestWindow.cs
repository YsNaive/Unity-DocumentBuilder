using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

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

        ScrollView show;
        Toggle isPrivate, isPublic, isInstance, isStatic;
        ObjectField selectScript;
        DropdownField selectType;
        private void CreateGUI()
        {
            show = DocRuntime.NewScrollView();
            var root = rootVisualElement;
            selectType = DocRuntime.NewDropdownField("get", new List<string> { "Constructor", "Methods", "Members" });
            selectType.index = 1;
            isPrivate = new Toggle("Private");
            isPrivate.RegisterValueChangedCallback(recalFlag);
            isPublic = new Toggle("Public");
            isPublic.value = true;
            isPublic.RegisterValueChangedCallback(recalFlag);
            isInstance = new Toggle("Instance");
            isInstance.value = true;
            isInstance.RegisterValueChangedCallback(recalFlag);
            isStatic = new Toggle("Static");
            isStatic.RegisterValueChangedCallback(recalFlag);
            selectScript = DocEditor.NewObjectField<MonoScript>("Target", (e) =>
            {
                if (e.newValue == null) return;
                repaint();
            });
            root.Add(selectScript);
            root.Add(selectType);
            root.Add(isPrivate);
            root.Add(isPublic);
            root.Add(isInstance);
            root.Add(isStatic);
            root.Add(show);
        }
        BindingFlags flag = BindingFlags.DeclaredOnly;
        void recalFlag(ChangeEvent<bool> e)
        {
            flag = BindingFlags.DeclaredOnly;
            if(isPublic.value) flag |= BindingFlags.Public;
            if(isPrivate.value) flag |= BindingFlags.NonPublic;
            if(isInstance.value) flag |= BindingFlags.Instance;
            if(isStatic.value) flag |= BindingFlags.Static;
            repaint();
        }
        void repaint()
        {
            if (selectScript.value == null) return;
            show.Clear();
            Type targetType = ((MonoScript)selectScript.value).GetClass();
            if(selectType.value == "Constructor")
            {
                foreach (var cons in targetType.GetConstructors(flag))
                {
                    show.Add(DocRuntime.NewTextElement("Cons: " + cons.Name));
                }
            }
            if(selectType.value == "Methods")
            {
                foreach (var mods in targetType.GetMethods(flag))
                {
                    show.Add(DocEditor.CreateComponentField(DocEditFuncDisplay.LoadMethod(mods)));
                }
            }
            if(selectType.value == "Members")
            {
                foreach (var mems in targetType.GetMembers(flag))
                {
                    show.Add(DocRuntime.NewTextElement("Mems: " + mems.Name));
                }
            }
        }
    }
}
