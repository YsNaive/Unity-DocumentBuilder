using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    public UIDocument UIDocument;

    // Start is called before the first frame update
    void Start()
    {
        UIDocument.rootVisualElement.Add(new DocEditView(new DocComponent(),Screen.width));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}