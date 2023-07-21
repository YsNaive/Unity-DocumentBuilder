using NaiveAPI_Editor.window;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestWindow : EditorWindow
{
    static TestWindow window;
    [MenuItem("Tools/NaiveAPI/Test Window")]
    public static void ShowWindow()
    {
        if(window != null)
        {
            window.Close();
            window = null;
        }
        window = CreateWindow<TestWindow>("Test Window");
    }
    private float previousWidth;
    private void OnEnable()
    {
        previousWidth = position.width;
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if ((docEditView != null)&&(previousWidth != position.width))
        {
            rootVisualElement.Remove(docEditView);
            docEditView = null;
            docEditView = new DocEditView(docComponent, (int)position.width);
            rootVisualElement.Add(docEditView);
            previousWidth = position.width;
        }
    }
    DocEditView docEditView = null;
    DocComponent docComponent;
    private void CreateGUI()
    {
        docComponent = new DocComponent();
        docEditView = new DocEditView(docComponent, (int)position.width);
        rootVisualElement.Add(docEditView);
    }
}
