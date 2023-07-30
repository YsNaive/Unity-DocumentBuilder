using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TTst : MonoBehaviour
{
    UIDocument UIDocument;
    VisualElement root;
    public SODocPage page;
    public DocComponent doc;
    public DocComponent doc2;
    void Start()
    {
        UIDocument = GetComponent<UIDocument>();
        root = UIDocument.rootVisualElement;
        var visual = new DocBookVisual(page);
        root.Add(visual);

    }
    // Update is called once per frame
    void Update()
    {
    }
}
