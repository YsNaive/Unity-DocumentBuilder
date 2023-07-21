using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DocVisual
{
    public abstract VisualElement CreateEditGUI(DocComponent docComponent, int width); 
    public abstract VisualElement CreateViewGUI(DocComponent docComponent, int width);
    public abstract DocComponent SaveTo(VisualElement visualElement, ref DocComponent docComponent);
}

public class TestVisual : DocVisual
{
    public override VisualElement CreateEditGUI(DocComponent docComponent, int width)
    {
        throw new System.NotImplementedException();
    }

    public override VisualElement CreateViewGUI(DocComponent docComponent, int width)
    {
        Debug.Log(docComponent.JsonData);
        return null;
    }

    public override DocComponent SaveTo(VisualElement visualElement, ref DocComponent docComponent)
    {
        throw new System.NotImplementedException();
    }
}

