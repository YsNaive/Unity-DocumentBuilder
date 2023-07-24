using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{

    public class DocEditField : VisualElement
    {
        public event Action<Type> OnDocTypeChange;
        public DocEditField(DocComponent docComponent)
        {
            DropdownField dropdown = new DropdownField();
            dropdown.label = "Type";
            dropdown.choices = DocEditor.VisualNameList;
            dropdown.index = DocEditor.VisualNameList.FindIndex(0, (str) => { return str == docComponent.VisualID; });
            if (dropdown.index == -1) dropdown.index = 0;
            dropdown.RegisterValueChangedCallback((val) =>
            {
                docComponent.TextData = string.Empty;
                docComponent.JsonData = string.Empty;
                docComponent.ObjsData.Clear();
                if (val.previousValue != "None") RemoveAt(1);
                if (val.newValue == "None")
                {
                    docComponent.VisualID = string.Empty;
                    OnDocTypeChange?.Invoke(null);
                    return;
                }
                docComponent.VisualID = DocEditor.VisualName2ID[val.newValue];
                Type docType = null;
                if (DocEditor.VisualID2Type.TryGetValue(docComponent.VisualID, out docType))
                {
                    DocEditVisual doc = (DocEditVisual)System.Activator.CreateInstance(docType);
                    doc.SetTarget(docComponent);
                    Add(doc);
                }
                else
                {
                    TextElement text = new TextElement();
                    text.text = $"Not Fount EditVisual for ID \"{docComponent.VisualID}\"";
                    Add(text);
                }
                OnDocTypeChange?.Invoke(docType);
            });
            dropdown.value = DocEditor.VisualNameList[dropdown.index];
            Add(dropdown);
        }
    }
}
