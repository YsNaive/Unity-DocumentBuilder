using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    public UIDocument document;
    public SODocPage page;
    public DocComponent doc;
    void Start()
    {
        document.rootVisualElement.Add(DocRuntime.CreateVisual(doc));
    }

}
