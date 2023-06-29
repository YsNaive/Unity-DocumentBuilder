using DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    public UIDocument UIDocument;
    public NameAndUsageComponent nameAndUsageComponent;
    public DescriptionComponent descriptionComponent;
    public PictureComponent pictureComponent;
    public FuncDisplayComponent funcDisplayComponent;
    public MatrixComponent matrixComponent;

    // Start is called before the first frame update
    void Start()
    {
        UIDocument.rootVisualElement.Add(nameAndUsageComponent.CreateRuntimeGUI());
        UIDocument.rootVisualElement.Add(descriptionComponent.CreateRuntimeGUI());
        UIDocument.rootVisualElement.Add(pictureComponent.CreateRuntimeGUI());
        UIDocument.rootVisualElement.Add(funcDisplayComponent.CreateRuntimeGUI());
        UIDocument.rootVisualElement.Add(matrixComponent.CreateRuntimeGUI());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
