using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocPageFactory_MethodAPI : DocPageFactory
    {
        public override string DisplayName => "Method";

        public override SODocComponents InitComponents => null;

        public override Texture2D InitIcon => DocEditorData.Icon.MethodIcon;

        public override void AfterPageCreate(SODocPage createdPage)
        {
            if (typeField.value == null) return;
            TargetMethod = typeField.value.GetMethod(methodDropdown.value);
            if (TargetMethod == null) return;
            createdPage.Components.Add(DocEditFuncDisplay.LoadMethod(TargetMethod)); 
        }

        DSTypeField typeField = new DSTypeField("Target Type");
        DSDropdown methodDropdown = new DSDropdown("Target Method") { choices = new() };
        public MethodInfo TargetMethod = null;
        public override VisualElement CreateEditGUI()
        {
            TargetMethod = null;
            var root = new VisualElement();
            root.Add(typeField);
            typeField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null)
                {
                    methodDropdown.choices = new();
                    methodDropdown.value = "";
                }
                var flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
                List<string> choices = new();
                foreach (var method in evt.newValue.GetMethods(flag).OrderBy(item => item.Name[0]))
                {
                    choices.Add(method.Name);
                }
                methodDropdown.choices = choices;
                methodDropdown.value = "";
            });
            root.Add(methodDropdown);
            return root;
        }

        public override string IsValid()
        {
            StringBuilder sb = new();
            if (typeField.value == null)
                sb.AppendLine("* Type not select");
            if(methodDropdown.value == "")
                sb.AppendLine("* Method not select");
            return sb.ToString();
        }
    }

}