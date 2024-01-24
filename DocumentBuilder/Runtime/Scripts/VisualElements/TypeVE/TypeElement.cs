using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class TypeElement : ScriptAPIElement
    {
        List<ScriptAPIElement> scriptAPIElements = new();
        SOScriptAPIInfo info;
        PopupElement tooltipPopup;
        public TypeElement(Type type)
        {
            style.flexDirection = FlexDirection.Column;
            tooltipPopup = new PopupElement(false);
            tooltipPopup.pickingMode = PickingMode.Ignore;
            tooltipPopup.CoverMask.pickingMode = PickingMode.Ignore;
            tooltipPopup.style.backgroundColor = DocStyle.Current.BackgroundColor;
            tooltipPopup.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1f));
            tooltipPopup.style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize/4));
            ScriptAPIInfoHolder.Infos.TryGetValue(type, out info);
            Add(new TypeNameElement(type));
            if (info != null)
            {
                var description = new VisualElement();
                description.style.marginLeft = DocStyle.Current.LineHeight;
                foreach (var com in info.Description)
                    description.Add(DocVisual.Create(com));
                Add(description);
            }
            var ctors = type.GetConstructors();
            if (ctors.Length != 0)
            {
                Add(DocVisual.Create(DocDividline.CreateComponent()));
                Add(new DSTextElement("Constructor"));
                foreach (var constructor in ctors)
                    addMemberInfo(constructor);
            }
            var publicField = type.GetFields(TypeReader.DeclaredPublicInstance);
            if (publicField.Length != 0)
            {
                Add(DocVisual.Create(DocDividline.CreateComponent()));
                Add(new DSTextElement("Public Field"));
                foreach (var field in publicField)
                {
                    addMemberInfo(field);
                }
            }
            var privateField = type.GetFields(TypeReader.DeclaredPrivateInstance);
            if (privateField.Length != 0)
            {
                Add(DocVisual.Create(DocDividline.CreateComponent()));
                Add(new DSTextElement("Private Field"));
                foreach (var field in privateField)
                    addMemberInfo(field);
            }
            var publicProp = type.GetProperties(TypeReader.DeclaredPublicInstance);
            if (publicProp.Length != 0)
            {
                Add(DocVisual.Create(DocDividline.CreateComponent()));
                Add(new DSTextElement("Public Property"));
                foreach (var prop in publicProp)
                    addMemberInfo(prop);
            }
            var privateProp = type.GetProperties(TypeReader.DeclaredPrivateInstance);
            if (privateProp.Length != 0)
            {
                Add(DocVisual.Create(DocDividline.CreateComponent()));
                Add(new DSTextElement("Private Property"));
                foreach (var prop in privateProp)
                    addMemberInfo(prop);
            }
            var publicMethod = type.GetMethods(TypeReader.DeclaredPublicInstance);
            if (publicMethod.Length != 0)
            {
                Add(DocVisual.Create(DocDividline.CreateComponent()));
                Add(new DSTextElement("Public Method"));
                foreach (var method in publicMethod)
                {
                    if (method.Name[0] == '<') continue;
                    if (method.Name.StartsWith("get_")) continue;
                    if (method.Name.StartsWith("set_")) continue;
                    addMemberInfo(method);
                }
            }

            if(info != null)
            {
                foreach(var pair in VisitMember())
                {
                    var memberInfo = info.GetMemberInfo(pair.id);
                    if(memberInfo.Tooltip.Count != 0)
                    {
                        pair.element.RegisterCallback<PointerEnterEvent>(evt =>
                        {
                            tooltipPopup.Clear();
                            //tooltipPopup.Add(getTitle(pair.memberInfo));
                            foreach (var com in memberInfo.Tooltip)
                                tooltipPopup.Add(DocVisual.Create(com));
                            tooltipPopup.Open(this);
                            pair.element.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                        });
                        pair.element.RegisterCallback<PointerLeaveEvent>(evt =>
                        {
                            tooltipPopup.Close();
                            pair.element.style.backgroundColor = Color.clear;
                        });
                        pair.element.RegisterCallback<PointerMoveEvent>(evt =>
                        {
                            var pos = tooltipPopup.CoverMask.WorldToLocal(evt.position);
                            tooltipPopup.transform.position =
                            new Vector3(pos.x + DocStyle.Current.MainTextSize*2,
                                        pos.y - tooltipPopup.layout.height - 5,
                                        tooltipPopup.transform.position.z);
                        });
                    }
                }
            }
        }
        void addMemberInfo(ICustomAttributeProvider info)
        {
            if (this.info != null)
            {
                if (!this.info.GetMemberInfo(info).IsDisplay)
                    return;
            }
            VisualElement container = new DSHorizontal();
            ScriptAPIElement title = getTitle(info);
            scriptAPIElements.Add(title);
            Add(title);
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
