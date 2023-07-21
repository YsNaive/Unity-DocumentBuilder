using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class DocComponent
{
    public string Type = "";
    public List<Object> ObjData = new List<Object>();
    public string JsonData = "";

    public DocVisual DocVisual
    {
        get
        {
            if (Type == "") return null;
            return (DocVisual)System.Activator.CreateInstance(System.Type.GetType(Type));
        }
    }
    public VisualElement CreateEditGUI(int width)
    {
        if(DocVisual == null) { return new VisualElement(); }
        return DocVisual.CreateEditGUI(this, width);
    }
    public VisualElement CreateViewGUI(int width)
    {
        if (DocVisual == null) { return new VisualElement(); }
        return DocVisual.CreateViewGUI(this, width);
    }
}
