using DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    public UIDocument UIDocument;
    // Start is called before the first frame update
    void Start()
    {
        LabelComponent label = new LabelComponent();
        label.StrData.Add("Hello World");
        UIDocument.rootVisualElement.Add(label.CreateRuntimeGUI());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
