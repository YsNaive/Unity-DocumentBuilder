using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
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
        public DocComponent Target;
        public DocEditField(DocComponent docComponent)
        {
            style.ClearMarginPadding();
            Target = docComponent;
            createDropfield();
            repaintDocEdit();
        }
        private void createDropfield()
        {
            DropdownField dropdown = new DropdownField();
            dropdown.choices = DocEditor.NameList;
            dropdown.style.ClearMarginPadding();
            string tName = string.Empty;
            DocEditor.ID2Name.TryGetValue(Target.VisualID, out tName);
            dropdown.index = DocEditor.NameList.FindIndex(0, (str) => { return str == tName; });
            if (dropdown.index == -1) dropdown.index = 0;
            dropdown.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.JsonData = string.Empty;
                Target.ObjsData.Clear();
                RemoveAt(1);
                if (val.newValue == "None")
                    Target.VisualID = string.Empty;
                else
                    Target.VisualID = DocEditor.Name2ID[val.newValue];
                repaintDocEdit();
            });
            dropdown.value = DocEditor.NameList[dropdown.index];
            Add(dropdown);
        }
        private void repaintDocEdit()
        {
            Type docType = null;
            if (DocEditor.ID2Type.TryGetValue(Target.VisualID, out docType))
            {
                DocEditVisual doc = (DocEditVisual)System.Activator.CreateInstance(docType);
                doc.SetTarget(Target);
                Add(doc);
            }
            else
            {
                TextElement text = new TextElement();
                text.text = $"Not Fount EditVisual for ID \"{Target.VisualID}\"";
                Add(text);
            }
            OnDocTypeChange?.Invoke(docType);
        }
    }
}
