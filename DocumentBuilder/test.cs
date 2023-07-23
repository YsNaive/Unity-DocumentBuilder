using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    public UIDocument UIDocument;
    public SODocPage Page;
    // Start is called before the first frame update
    void Start()
    {
        UIDocument.rootVisualElement.Add(new DocPageVisual(Page));
        TextElement element = new TextElement();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
