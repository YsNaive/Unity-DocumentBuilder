using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField]
    DocComponent DocComponent;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(typeof(TestVisual).AssemblyQualifiedName);
        ((DocVisual)Activator.CreateInstance(Type.GetType(DocComponent.Type))).CreateViewGUI(DocComponent);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
