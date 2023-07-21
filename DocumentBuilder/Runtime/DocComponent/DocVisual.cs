using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DocVisual
{
    public abstract VisualElement CreateEditGUI(ref DocComponent docComponent); 
    public abstract VisualElement CreateViewGUI(DocComponent docComponent);
}

public class TestVisual : DocVisual
{
    public override VisualElement CreateEditGUI(ref DocComponent docComponent)
    {
        throw new System.NotImplementedException();
    }

    public override VisualElement CreateViewGUI(DocComponent docComponent)
    {
        Debug.Log(docComponent.JsonData);
        return null;
    }
}

