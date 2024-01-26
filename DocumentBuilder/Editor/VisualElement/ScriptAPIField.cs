using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIField : DSScriptAPIElement
    {
        SOScriptAPIInfo target;
        DocComponentsField DescriptionField, TooltipField;
        List<VisualElement> FieldInfoFields = new();
        SplitView EditSplitView;
        DSScrollView EditPanelContainer;
        DSScrollView TypeFieldScrollView;
        List<DSScriptAPIElement> scriptAPIElements = new();
        SerializedObject serializedObject;
        public ScriptAPIField(Type type)
        {
            style.flexDirection = FlexDirection.Column;
            if (ScriptAPIInfoHolder.Infos.TryGetValue(type, out target))
            {
                init();
            }
            else
            {
                Add(new DSTextElement("Create a SOScriptAPIInfo to start edit."));
                var dir = "";
                var createBtn = new DSButton("Create");
                createBtn.SetEnabled(false);
                var folderField = DocEditor.NewObjectField<DefaultAsset>(evt =>
                {
                    if (evt.newValue == null) return;
                    dir = AssetDatabase.GetAssetPath(evt.newValue);
                    createBtn.SetEnabled(AssetDatabase.IsValidFolder(dir));
                });
                createBtn.clicked += () =>
                {
                    var info = ScriptableObject.CreateInstance<SOScriptAPIInfo>();
                    info.TargetType = type;
                    info.MakeValid();
                    target = info;
                    AssetDatabase.CreateAsset(info, $"{dir}/{TypeReader.GetGenericName(type)}.asset");
                    AssetDatabase.Refresh();
                    init();
                };
                Add(folderField);
                Add(createBtn);
            }
        }
        public ScriptAPIField(SOScriptAPIInfo target)
        {
            style.flexDirection = FlexDirection.Column;
            this.target = target;
            init();
        }

        void init()
        {
            Clear();
            serializedObject = new SerializedObject(target);
            EditPanelContainer = new DSScrollView();
            EditPanelContainer.style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize / 2));
            EditPanelContainer.style.backgroundColor = DocStyle.Current.BackgroundColor;
            TypeFieldScrollView = new DSScrollView();
            EditSplitView = new SplitView( FlexDirection.Row, 25);
            EditSplitView.MinViewWidthPx = DocStyle.Current.MainTextSize * 10;
            EditSplitView.style.position = Position.Absolute;
            EditSplitView.style.width  = Length.Percent(100);
            EditSplitView.style.height = Length.Percent(100);
            VisualElement leftCover = new VisualElement();
            leftCover.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            leftCover.style.opacity = 0.25f;
            leftCover.RegisterCallback<PointerDownEvent>(evt =>
            {
                EditSplitView.parent.Remove(EditSplitView);
                serializedObject.ApplyModifiedProperties();
            });
            EditSplitView.Add(leftCover);
            EditSplitView.Add(EditPanelContainer);
            target.MakeValid();
            repaintType();
            Add(TypeFieldScrollView);
        }
        void repaintType()
        {
            TypeFieldScrollView.Clear();
            TypeFieldScrollView.Add(new DSTextElement("Type Tooltip"));
            TooltipField = new DocComponentsField(serializedObject.FindProperty("Tooltip"));
            TypeFieldScrollView.Add(TooltipField);
            TypeFieldScrollView.Add(new DSTextElement("Type Description"));
            DescriptionField = new DocComponentsField(serializedObject.FindProperty("Description"));
            TypeFieldScrollView.Add(DescriptionField);
            var ctors = target.TargetType.GetConstructors();
            if (ctors.Length != 0)
            {
                TypeFieldScrollView.Add(new DSTextElement("Constructor"));
                foreach (var constructor in ctors)
                    addMemberInfo(constructor);
            }
            var publicField = target.TargetType.GetFields(TypeReader.DeclaredPublicInstance);
            if (publicField.Length != 0)
            {
                TypeFieldScrollView.Add(new DSTextElement("Public Field"));
                foreach (var field in publicField)
                    addMemberInfo(field);
            }
            var privateField = target.TargetType.GetFields(TypeReader.DeclaredPrivateInstance);
            if (privateField.Length != 0)
            {
                TypeFieldScrollView.Add(new DSTextElement("Private Field"));
                foreach (var field in privateField)
                    addMemberInfo(field);
            }
            var publicProp = target.TargetType.GetProperties(TypeReader.DeclaredPublicInstance);
            if (publicProp.Length != 0)
            {
                TypeFieldScrollView.Add(new DSTextElement("Public Property"));
                foreach (var prop in publicProp)
                    addMemberInfo(prop);
            }
            var privateProp = target.TargetType.GetProperties(TypeReader.DeclaredPrivateInstance);
            if (privateProp.Length != 0)
            {
                TypeFieldScrollView.Add(new DSTextElement("Private Property"));
                foreach (var prop in privateProp)
                    addMemberInfo(prop);
            }
            var publicMethod = target.TargetType.GetMethods(TypeReader.DeclaredPublicInstance);
            if (publicMethod.Length != 0)
            {
                TypeFieldScrollView.Add(new DSTextElement("Public Method"));
                foreach (var method in publicMethod)
                {
                    if (method.Name[0] == '<') continue;
                    if (method.Name.StartsWith("get_")) continue;
                    if (method.Name.StartsWith("set_")) continue;
                    addMemberInfo(method);
                }
            }
            var privateMethod = target.TargetType.GetMethods(TypeReader.DeclaredPrivateInstance);
            if (privateMethod.Length != 0)
            {
                TypeFieldScrollView.Add(new DSTextElement("Private Method"));
                foreach (var method in privateMethod)
                {
                    if (method.Name[0] == '<') continue;
                    if (method.Name.StartsWith("get_")) continue;
                    if (method.Name.StartsWith("set_")) continue;
                    addMemberInfo(method);
                }
            }
            var highlight = new ISBorder(DocStyle.Current.SubFrontgroundColor, 0) { Left = DocStyle.Current.MainTextSize / 6 };
            var clear = new ISBorder(Color.clear, 0);
            foreach(var pair in VisitMember())
            {
                pair.element.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    pair.element.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    pair.element.style.SetIS_Style(highlight);
                });
                pair.element.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    pair.element.style.backgroundColor = Color.clear;
                    pair.element.style.SetIS_Style(clear);
                });
                pair.element.RegisterCallback<PointerDownEvent>(evt =>
                {
                    Add(EditSplitView);
                    EditPanelContainer.Clear();
                    EditPanelContainer.Add(getTitle(pair.memberInfo));
                    EditPanelContainer.Add(new ScriptAPIMemberField(SOScriptAPIInfo.GetMemberInfo(serializedObject, pair.id)));
                });
            }
        }
        void addMemberInfo(MemberInfo info)
        {
            var targetMemberInfo = SOScriptAPIInfo.GetMemberInfo(serializedObject,SOScriptAPIInfo.GetMemberID(info));
            VisualElement container = new DSHorizontal();
            DSScriptAPIElement title = getTitle(info);
            scriptAPIElements.Add(title);
            DSToggle displayToggle = new DSToggle();
            displayToggle.style.alignItems = Align.FlexStart;
            displayToggle.value = targetMemberInfo.FindPropertyRelative("IsDisplay").boolValue;
            displayToggle.RegisterValueChangedCallback(evt =>
            {
                targetMemberInfo.FindPropertyRelative("IsDisplay").boolValue = evt.newValue;
                title.SetEnabled(evt.newValue); 
            });
            title.SetEnabled(displayToggle.value);
            container.Add(displayToggle);
            container.Add(title);
            FieldInfoFields.Add(container);
            TypeFieldScrollView.Add(container);
        }
        DSScriptAPIElement getTitle(ICustomAttributeProvider info)
        {
            return info switch
            {
                FieldInfo asField => new DSFieldInfoElement(asField),
                PropertyInfo asProp => new DSPropertyInfoElement(asProp),
                MethodInfo asMethod => new DSMethodInfoElement(asMethod),
                ConstructorInfo asCtor => new DSMethodInfoElement(asCtor),
                ParameterInfo asParam => new DSParameterInfoElement(asParam),
                _ => null
            };
        }
        public override IEnumerable<DSTypeNameElement> VisitTypeName()
        {
            foreach (var ve in scriptAPIElements)
            {
                foreach (var it in ve.VisitTypeName())
                    yield return it;
            }
        }

        public override IEnumerable<DSParameterInfoElement> VisitParameter()
        {
            foreach (var ve in scriptAPIElements)
            {
                foreach (var it in ve.VisitParameter())
                    yield return it;
            }
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element, string id)> VisitMember()
        {
            foreach (var ve in scriptAPIElements)
            {
                foreach (var it in ve.VisitMember())
                    yield return it;
            }
        }
    }

}