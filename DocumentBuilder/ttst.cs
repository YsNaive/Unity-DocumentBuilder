using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI.DocumentBuilder;

public class ttst : MonoBehaviour
{
    public DocComponent doc = new DocComponent();
    public UIDocument UIDocument;
    // Start is called before the first frame update
    void Start()
    {
        doc.JsonData = "";
        UIDocument.rootVisualElement.Add(new DocEditView(doc, Screen.width));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
