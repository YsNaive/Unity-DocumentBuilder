using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIMemberField : VisualElement
    {
        SerializedProperty target;
        public ScriptAPIMemberField(SerializedProperty target)
        {
            this.target = target;
            initLayout();
        }
        void initLayout()
        {
            Add(new DSTextElement("Tooltip"));
            var tooltipField = new DocComponentsField(target.FindPropertyRelative("Tooltip"));
            Add(tooltipField);

            Add(new DSTextElement("Description"));
            var descriptionField = new DocComponentsField(target.FindPropertyRelative("Description"));
            Add(descriptionField);
        }
    }
}
