using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ttst : MonoBehaviour
{
    public DocComponent doc = new DocComponent();
    public UIDocument UIDocument;
    // Start is called before the first frame update
    void Start()
    {
        doc.JsonData = "";
        DocMatrix docMatrix = new DocMatrix();
        UIDocument.rootVisualElement.Add(docMatrix.CreateEditGUI(doc, 400));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
