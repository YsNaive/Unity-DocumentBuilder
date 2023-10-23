using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocPageFactory_ClassAPI : DocPageFactory
    {

        static SODocComponents m_InitComponents;
        static SODocComponents m_FuncTemplate;
        static Texture2D m_InitIcon;
        static Texture2D m_FuncIcon;
        public override string DisplayName => "ClassAPI";
        public override SODocComponents InitComponents
        {
            get
            {
                if (m_InitComponents == null)
                    m_InitComponents = AssetDatabase.LoadAssetAtPath<SODocComponents>(AssetDatabase.GUIDToAssetPath("b38098c599befe54a884c2ab9ed4d15a"));
                return m_InitComponents;
            }
        }
        public override Texture2D InitIcon
        {
            get
            {
                if (m_InitIcon == null)
                    m_InitIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath("77fc49cad09df0142a9bbebdffc823a5"));
                return m_InitIcon;
            }
        }
        public SODocComponents FuncTemplate
        {
            get
            {
                if (m_FuncTemplate == null)
                    m_FuncTemplate = AssetDatabase.LoadAssetAtPath<SODocComponents>(AssetDatabase.GUIDToAssetPath("a29c8bd9f7af3ba49a843d24eddbf17d"));
                return m_FuncTemplate;
            }
        }
        public Texture2D FuncIcon
        {
            get
            {
                if (m_FuncIcon == null)
                    m_FuncIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath("116bd5b02a236df4d9e4d81f16c51c48"));
                return m_FuncIcon;
            }
        }

        public override void AfterPageCreate(SODocPage createdPage)
        {
            createdPage.Components[0].TextData[0] = DocumentBuilderParser.CalGenericTypeName(targetType);

            var instancePublicResult = new DocComponentsTypeInfoReader(targetType, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (instancePublicResult.PropertiesMatrixComponents != null)
            {
                createdPage.Components.Add(DocDividline.CreateComponent());
                createdPage.Components.Add(DocLabel.CreateComponent("Properties"));
                createdPage.Components.Add(instancePublicResult.PropertiesMatrixComponents);
            }
            if (instancePublicResult.FieldsMatrixComponents != null)
            {
                createdPage.Components.Add(DocDividline.CreateComponent());
                createdPage.Components.Add(DocLabel.CreateComponent("Fields"));
                createdPage.Components.Add(instancePublicResult.FieldsMatrixComponents);
            }
            if (instancePublicResult.MethodListComponent != null)
            {
                createdPage.Components.Add(DocDividline.CreateComponent());
                createdPage.Components.Add(DocLabel.CreateComponent("Methods"));
                createdPage.Components.Add(instancePublicResult.MethodListComponent);
            }

            var staticPublicResult = new DocComponentsTypeInfoReader(targetType, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            if (staticPublicResult.PropertiesMatrixComponents != null)
            {
                createdPage.Components.Add(DocDividline.CreateComponent());
                createdPage.Components.Add(DocLabel.CreateComponent("Static Properties"));
                createdPage.Components.Add(staticPublicResult.PropertiesMatrixComponents);
            }
            if (staticPublicResult.FieldsMatrixComponents != null)
            {
                createdPage.Components.Add(DocDividline.CreateComponent());
                createdPage.Components.Add(DocLabel.CreateComponent("Static Fields"));
                createdPage.Components.Add(staticPublicResult.FieldsMatrixComponents);
            }
            if (staticPublicResult.MethodListComponent != null)
            {
                createdPage.Components.Add(DocDividline.CreateComponent());
                createdPage.Components.Add(DocLabel.CreateComponent("Static Methods"));
                createdPage.Components.Add(staticPublicResult.MethodListComponent);
            }

            var inheritPublicResult = new DocComponentsTypeInfoReader(targetType, BindingFlags.Public | BindingFlags.Instance);
            var component = inheritPublicResult.MethodListComponent;
            if(component != null)
            {
                List<string> todelete = new();
                if(instancePublicResult.MethodListComponent != null)
                {
                    foreach (var text in component.TextData)
                    {
                        foreach (var text2 in instancePublicResult.MethodListComponent.TextData)
                        {
                            if (text == text2)
                            {
                                todelete.Add(text);
                                break;
                            }
                        }
                    }
                }
                foreach (var text in todelete)
                {
                    component.TextData.Remove(text);
                }
                component.JsonData = JsonUtility.ToJson(new DocItems.Data() { num = component.TextData.Count }) ;
                if (component.TextData.Count > 0)
                {
                    createdPage.Components.Add(DocDividline.CreateComponent());
                    createdPage.Components.Add(DocLabel.CreateComponent("Inherited Methods"));
                    createdPage.Components.Add(component);
                }
            }
        }


        Type targetType;
        public override VisualElement CreateEditGUI()
        {
            var root = DocRuntime.NewEmpty();
            var searchField = new DSTextField("Type: ");
            var selectContainer = new VisualElement();
            selectContainer.style.paddingLeft = DocStyle.Current.LabelWidth;
            searchField.RegisterValueChangedCallback(evt =>
            {
                selectContainer.Clear();
                if (evt.newValue == "") return;
                List<Type> matchedType = new();
                foreach(var asmdef in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in asmdef.GetTypes())
                    {
                        if (type.Name.Contains(evt.newValue))
                        {
                            matchedType.Add(type);
                        }
                    }
                }
                int i  = 0;
                foreach (var type in matchedType.OrderBy(e => { return e.Name.Length; }))
                {
                    var btn = DocRuntime.NewButton(type.Name);
                    btn.clicked += () =>
                    {
                        targetType = type;
                        searchField.value = "";
                        searchField.label = $"Type: {type.Name}";
                    };
                    selectContainer.Add(btn);
                    if (i++ > 7) break;
                }
            });
            root.Add(searchField);
            root.Add(selectContainer);


            return root;
        }
    
        
    }

}