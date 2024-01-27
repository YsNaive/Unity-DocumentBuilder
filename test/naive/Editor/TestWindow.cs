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
        Dictionary<int, List<(string spaceName, string typeName)>> searchResult = new();
        var field = new DSTextField();
        var btn = new DSButton();
        List<(string spaceName, string typeName)> allName = new ();
        foreach (var type in TypeReader.ActiveTypes)
        {
            var typeName = TypeReader.GetName(type);
            var spaceName = type.Namespace;
            spaceName ??= "";
            allName.Add((spaceName, typeName));
        }
        
        field.RegisterValueChangedCallback(evt =>
        {
            searchResult.Clear();
            var input = field.value;
            Stopwatch levenshteinWatch = Stopwatch.StartNew();
            foreach (var name in allName)
            {
                if (Math.Abs(input.Length - name.typeName.Length) > 8) continue;
                var distance = input.LevenshteinDistance(name.typeName);
                var lname = name.typeName.ToLower();
                var linput = input.ToLower();
                if (lname.Contains(linput))
                    distance /= 2;
                if (lname.StartsWith(linput))
                    distance /= 2;
                if (!searchResult.ContainsKey(distance))
                    searchResult.Add(distance, new());
                searchResult[distance].Add(name);
            }
            levenshteinWatch.Stop();
            Stopwatch containsWatch = Stopwatch.StartNew();
            foreach (var name in allName)
            {
                _ = name.typeName.Contains(input);
            }
            containsWatch.Stop();
            sc.Clear();
            sc.Add(new DSTextElement($"Levenshtein\t {levenshteinWatch.Elapsed.Seconds + (levenshteinWatch.Elapsed.Milliseconds * 0.0001)} sec"));
            sc.Add(new DSTextElement($"Contains\t\t {containsWatch.Elapsed.Seconds + (containsWatch.Elapsed.Milliseconds * 0.0001)} sec"));
            var list = searchResult.OrderBy(m => { return m.Key; }).ToList();
            int i = 0;
            foreach (var pair in list)
            {
                foreach (var val in pair.Value)
                {
                    var hor = new DSHorizontal();
                    hor.Add(new DSTextElement(val.typeName));
                    var spaceName = new DSTextElement("  "+val.spaceName);
                    spaceName.style.opacity = 0.7f;
                    hor.Add(spaceName);
                    sc.Add(hor);
                    i++;
                    if (i > 25) break;
                }
                if (i > 25) break;
            }
        });

        root.Add(field);
        root.Add(btn);
        root.Add(sc);
    }
}
