using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace NaiveAPI_Editor.DocumentBuilder
{

    public class DocEditField : VisualElement
    {
        public event Action<Type> OnDocTypeChange;
        DocComponent target;
        public DocEditField(DocComponent docComponent)
        {
            target = docComponent;
            createDropfield();
            repaintDocEdit();
        }
        private void createDropfield()
        {
            DropdownField dropdown = new DropdownField();
            dropdown.choices = DocEditor.NameList;
            string tName = string.Empty;
            DocEditor.ID2Name.TryGetValue(target.VisualID, out tName);
            dropdown.index = DocEditor.NameList.FindIndex(0, (str) => { return str == tName; });
            if (dropdown.index == -1) dropdown.index = 0;
            dropdown.RegisterValueChangedCallback((val) =>
            {
                target.TextData.Clear();
                target.JsonData = string.Empty;
                target.ObjsData.Clear();
                RemoveAt(1);
                if (val.newValue == "None")
                    target.VisualID = string.Empty;
                else
                    target.VisualID = DocEditor.Name2ID[val.newValue];
                repaintDocEdit();
            });
            dropdown.value = DocEditor.NameList[dropdown.index];
            Add(dropdown);
        }
        private void repaintDocEdit()
        {
            Type docType = null;
            if (DocEditor.ID2Type.TryGetValue(target.VisualID, out docType))
            {
                DocEditVisual doc = (DocEditVisual)System.Activator.CreateInstance(docType);
                doc.SetTarget(target);
                Add(doc);
            }
            else
            {
                TextElement text = new TextElement();
                text.text = $"Not Fount EditVisual for ID \"{target.VisualID}\"";
                Add(text);
            }
            OnDocTypeChange?.Invoke(docType);
        }
    }
}
