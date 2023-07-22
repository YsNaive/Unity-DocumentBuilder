using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DocInspectorWindow : EditorWindow
{
    [MenuItem("Tools/NaiveAPI/Doc Inspector")]
    public static void ShowWindow()
    {
        GetWindow<DocInspectorWindow>("Doc Inspector");
    }
    private void CreateGUI()
    {
        ObjectField page = new ObjectField("Page");
        page.objectType = typeof(SODocPage);
        rootVisualElement.style.SetIS_Style(ISPadding.Pixel(10));
        rootVisualElement.Add(page);
        page.RegisterValueChangedCallback((value) =>
        {
            for(int i=1;i<rootVisualElement.childCount;i++)
                rootVisualElement.RemoveAt(i);
            rootVisualElement.Add(Editor.CreateEditor(value.newValue).CreateInspectorGUI());
        });
    }
}
