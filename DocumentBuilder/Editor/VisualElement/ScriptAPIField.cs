using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
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
                var createBtn = new DSButton("Create");
                var folderPath = DocCache.LoadData("CreateScriptAPIInfoPath.txt");
                var folderField = new DSAssetFolderField();
                if (!AssetDatabase.IsValidFolder(folderPath)) folderPath = "Assets";
                folderField.value = folderPath;
                folderField.RegisterValueChangedCallback(evt =>
                {
                    DocCache.SaveData("CreateScriptAPIInfoPath.txt", evt.newValue);
                });
                createBtn.clicked += () =>
                {
                    var info = ScriptableObject.CreateInstance<SOScriptAPIInfo>();
                    info.TargetType = type;
                    info.Description.Add(DocDescription.CreateComponent(""));
                    target = info;
                    var assetName = type.Name;
                    if (type.Namespace != null)
                        assetName = $"{type.Namespace}.{assetName}";
                    AssetDatabase.CreateAsset(info, $"{folderField.value}/{assetName}.asset");
                    AssetDatabase.Refresh();
                    ScriptAPIInfoHolder.Infos.Add(type, target);
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
            var obj = DocEditor.NewObjectField<SOScriptAPIInfo>("Editing");
            obj.style.opacity = 0.75f;
            obj.RegisterValueChangedCallback(evt => {  obj.SetValueWithoutNotify(evt.newValue); });
            obj.value = target;
            Add(obj);
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
            Func<MemberInfo, bool> displayCheck = attr =>
            {
                if (typeof(MethodBase).IsAssignableFrom(attr.GetType()))
                {
                    var method = attr as MethodBase;
                    if (method.Name[0] == '<') return false;
                    if (method.Name.StartsWith("op_")) return false;
                    if (method.Name.StartsWith("get_")) return false;
                    if (method.Name.StartsWith("set_")) return false;
                    if (method.Name.StartsWith("add_")) return false;
                    if (method.Name.StartsWith("remove_")) return false;
                }
                return true;
            };
            foreach (var pair in TypeReader.VisitDeclearedMember(target.TargetType, displayCheck))
            {
                var text = new DSTextElement(pair.memberType);
                text.style.marginTop = DocStyle.Current.LineHeight.Value / 2;
                text.style.opacity = 0.6f;
                TypeFieldScrollView.Add(text);
                foreach (var info in pair.members)
                    addMemberInfo(info);
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
                    EditPanelContainer.Add(Create(pair.memberInfo));
                    EditPanelContainer.Add(new ScriptAPIMemberField(SOScriptAPIInfo.GetMemberInfo(serializedObject, pair.memberInfo)));
                });
            }
        }
        void addMemberInfo(MemberInfo info)
        {
            var targetMemberInfo = SOScriptAPIInfo.GetMemberInfo(serializedObject, info);
            VisualElement container = new DSHorizontal();
            DSScriptAPIElement title = Create(info);
            scriptAPIElements.Add(title);
            DSToggle displayToggle = new DSToggle();
            displayToggle.style.alignItems = Align.FlexStart;
            displayToggle.value = targetMemberInfo.FindPropertyRelative("IsDisplay").boolValue;
            displayToggle.RegisterValueChangedCallback(evt =>
            {
                targetMemberInfo.FindPropertyRelative("IsDisplay").boolValue = evt.newValue;
                targetMemberInfo.serializedObject.ApplyModifiedProperties();
                title.SetEnabled(evt.newValue); 
            });
            title.SetEnabled(displayToggle.value);
            container.Add(displayToggle);
            container.Add(title);
            FieldInfoFields.Add(container);
            TypeFieldScrollView.Add(container);
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

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            foreach (var ve in scriptAPIElements)
            {
                foreach (var it in ve.VisitMember())
                    yield return it;
            }
        }
    }

}