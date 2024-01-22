using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIMemberField : VisualElement
    {
        ScriptAPIMemberInfo target;
        public ScriptAPIMemberField(ScriptAPIMemberInfo target)
        {
            this.target = target;
            initLayout();
        }
        void initLayout()
        {
            Add(new DSTextElement("Tooltip"));
            var tooltipField = new DocComponentsField(target.Tooltip);
            tooltipField.OnModify += evt => target.Tooltip = tooltipField.ToComponentsList();
            Add(tooltipField);

            Add(new DSTextElement("Description"));
            var descriptionField = new DocComponentsField(target.Description);
            descriptionField.OnModify += evt => target.Description = descriptionField.ToComponentsList();
            Add(descriptionField);
        }
    }
}
