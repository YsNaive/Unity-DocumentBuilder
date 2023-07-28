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
        var visual = new DocPageVisual(page);
        root.Add(visual);
        visual.PlayIntro(() => {
                visual.PlayOuttro(() =>
                {
                    visual.Repaint();
                });
        }) ;
        
    }
    // Update is called once per frame
    void Update()
    {
    }
}
