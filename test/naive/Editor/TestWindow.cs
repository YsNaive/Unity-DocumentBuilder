using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class TestWindow : EditorWindow
{
    [MenuItem("Tools/TestWindow", priority = 99)]
    public static void Open()
    {
        GetWindow<TestWindow>("Test");
    }
    public void OnEnable()
    {
        GC.Collect();
    }
    public void OnDestroy()
    {
    }
    private void CreateGUI()
    {
        var root = rootVisualElement;
        root.style.backgroundColor = DocStyle.Current.BackgroundColor;
        root.style.SetIS_Style(ISPadding.Pixel(10));
        var sc = new DSScrollView();
        var field = new ObjectField();
        field.objectType = typeof(MonoScript);
        var btn = new DSButton();
        btn.clicked += () =>
        {
            AssetDatabase.OpenAsset(field.value, 50);
        };
        root.Add(btn);
        root.Add(sc);
    }
}
