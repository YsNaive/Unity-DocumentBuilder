using NaiveAPI_Editor.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocSample : DocVisual
{
    public override string VisualID => "Your Visual ID";

    // this will invoke after the DocVisual's value set-up
    protected override void OnCreateGUI()
    {
        // Add some visual you want
    }
}
public class DocEditSample : DocEditVisual
{
    public override string DisplayName => "Name to show";

    public override string VisualID => // same ID as in DocVisual

    protected override void OnCreateGUI()
    {
        // Add some visual to modify contents
    }
}
