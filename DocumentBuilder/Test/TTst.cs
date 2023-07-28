using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TTst : MonoBehaviour
{
    UIDocument UIDocument;
    VisualElement root;
    void Start()
    {
        UIDocument = GetComponent<UIDocument>();
        root = UIDocument.rootVisualElement;
        IVisualElementScheduledItem item = null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
