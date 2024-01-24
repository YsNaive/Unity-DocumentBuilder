using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

        root.Add(sc);
    }
}
