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
    void Start()
    {
        UIDocument = GetComponent<UIDocument>();
        root = UIDocument.rootVisualElement;
        //var visual = new DocBookVisual(page);
        //root.Add(visual);
        root.Insert(0,DocRuntime.NewScrollView());
        root[0].style.height = 300;
        root[0].style.width = 300;
        root[0].Add(DocRuntime.NewEmpty());
        root[0][0].style.width = 900;
        root[0][0].style.height = 900;

    }
    // Update is called once per frame
    void Update()
    {
    }
}
