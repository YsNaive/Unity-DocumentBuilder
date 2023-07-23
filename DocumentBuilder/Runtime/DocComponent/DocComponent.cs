using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class DocComponent
{
    public string Type = "";
    public List<Object> ObjData = new List<Object>();
    public string TextData = "";
    public string JsonData = "";
    public VisualElement CreateEditGUI(int width)
    {
        if (Type == "") return new VisualElement();
        return DocData.ComponentInstanceDict[Type].CreateEditGUI(this, width);
    }
    public VisualElement CreateViewGUI(int width)
    {
        if (Type == "") return new VisualElement();
        return DocData.ComponentInstanceDict[Type].CreateViewGUI(this, width);
    }
    public string ToMarkdown(VisualElement visual)
    {
        if (Type == "") return "";
        return DocData.ComponentInstanceDict[Type].ToMarkdown(visual);
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("DOCCOMPONENT");
        sb.Append("%SPLITE%");
        sb.Append(Type);
        sb.Append("%SPLITE%");
        sb.Append(JsonData);
        return sb.ToString();
    }
    public void FromVisual(VisualElement visual)
    {
        if (Type == "") return;
        DocData.ComponentInstanceDict[Type].SaveTo(visual, this);
    }
    public bool FromString(string str)
    {
        string[] data = str.Split("%SPLITE%");
        if (data[0] == "DOCCOMPONENT")
        {
            return false;
        }
        Type = data[1];
        JsonData = data[2];
        return true;
    }

    private static List<Object> objClipBoard;
    public void CopyToClipBoard()
    {
        GUIUtility.systemCopyBuffer = ToString();
        objClipBoard = new List<Object>(ObjData);
    }
    public void PasteFromClipBoard()
    {
        if (!FromString(GUIUtility.systemCopyBuffer))
            return;
        if (objClipBoard != null)
            ObjData = objClipBoard;
        else
            ObjData = new List<Object>();
        objClipBoard = null;
    }
}
