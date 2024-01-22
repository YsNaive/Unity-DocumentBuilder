using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIField : ScriptAPIElement
    {
        SOScriptAPIInfo target;
        DocComponentsField DescriptionField, TutorialField, TooltipField;
        List<VisualElement> FieldInfoFields = new();
        SplitView EditSplitView;
        DSScrollView EditPanelContainer;
        DSScrollView TypeFieldScrollView;
        List<ScriptAPIElement> scriptAPIElements = new();
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
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssetIfDirty(target);
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
            TooltipField = new DocComponentsField(target.Tooltip);
            TooltipField.OnModify += evt => { target.Tooltip = TooltipField.ToComponentsList(); EditorUtility.SetDirty(target); };
            TypeFieldScrollView.Add(TooltipField);
            TypeFieldScrollView.Add(new DSTextElement("Type Description"));
            DescriptionField = new DocComponentsField(target.Description);
            DescriptionField.OnModify += evt => { target.Tooltip = DescriptionField.ToComponentsList(); EditorUtility.SetDirty(target); };
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
            TypeFieldScrollView.Add(new DSTextElement("Type Tutorial"));
            TutorialField = new DocComponentsField(target.Description);
            TutorialField.OnModify += evt => { target.Tooltip = TutorialField.ToComponentsList(); EditorUtility.SetDirty(target); };
            TypeFieldScrollView.Add(TutorialField);
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
                    EditPanelContainer.Add(new ScriptAPIMemberField(target.GetMemberInfo(pair.id)));
                });
            }
        }
        void addMemberInfo(MemberInfo info)
        {
            var targetMemberInfo = target.GetMemberInfo(info);
            VisualElement container = new DSHorizontal();
            ScriptAPIElement title = getTitle(info);
            scriptAPIElements.Add(title);
            DSToggle displayToggle = new DSToggle();
            displayToggle.style.alignItems = Align.FlexStart;
            displayToggle.value = targetMemberInfo.IsDisplay;
            displayToggle.RegisterValueChangedCallback(evt =>
            {
                targetMemberInfo.IsDisplay = evt.newValue;
                title.SetEnabled(evt.newValue); 
                EditorUtility.SetDirty(target);
            });
            title.SetEnabled(displayToggle.value);
            container.Add(displayToggle);
            container.Add(title);
            FieldInfoFields.Add(container);
            TypeFieldScrollView.Add(container);
        }
        ScriptAPIElement getTitle(ICustomAttributeProvider info)
        {
            return info switch
            {
                FieldInfo asField => new FieldInfoElement(asField),
                PropertyInfo asProp => new PropertyInfoElement(asProp),
                MethodInfo asMethod => new MethodInfoElement(asMethod),
                ConstructorInfo asCtor => new MethodInfoElement(asCtor),
                ParameterInfo asParam => new ParameterInfoElement(asParam),
                _ => null
            };
        }
        public override IEnumerable<TypeNameElement> VisitTypeName()
        {
            foreach (var ve in scriptAPIElements)
            {
                foreach (var it in ve.VisitTypeName())
                    yield return it;
            }
        }

        public override IEnumerable<ParameterInfoElement> VisitParameter()
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