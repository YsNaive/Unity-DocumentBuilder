using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Ttst : EditorWindow
{
    [MenuItem("Tools/howhoward_ttst")]
    public static void Open()
    {
        GetWindow<Ttst>("howhoward_ttst");
    }
    private void OnEnable()
    {
        GridView gridView = new GridView(3, 3, Color.black, GridView.AlignMode.FixedContent);
        gridView[0, 1].Add(new DSTextElement("Naive is naive"));
        gridView[1, 1].Add(new DSTextElement("Naive is naive and ugly, howhoward is cute"));
        gridView[2, 1].Add(new DSTextElement("Naive is naive and ugly, howhoward is cute\n"));
        gridView[2, 1].Add(new DSButton("howhoward is cute."));
        rootVisualElement.Add(gridView);
        //rootVisualElement.schedule.Execute(() => gridView.ResolveLayout()).Every(1000);
        rootVisualElement.RegisterCallback<GeometryChangedEvent>(gridView.ResolveLayout);
    }
}
