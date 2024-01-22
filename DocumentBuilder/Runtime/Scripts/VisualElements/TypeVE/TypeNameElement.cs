using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class TypeNameElement : ScriptAPIElement
    {
        public Type TargetType => m_TargetType;
        private Type m_TargetType;
        public DSTextElement NameText => m_NameText;
        private DSTextElement m_NameText;
        private TypeNameElement[] genericTypeName;
        public TypeNameElement(Type type) 
            : base()
        {
            m_TargetType = type;
            m_NameText = new DSTextElement();
            if (TypeReader.TypeNameTable.ContainsKey(type))
                m_NameText.style.color = DocStyle.Current.PrefixColor;
            else if (type.IsValueType)
                m_NameText.style.color = DocStyle.Current.ValueTypeColor;
            else
                m_NameText.style.color = DocStyle.Current.TypeColor;
            Add(m_NameText);

            if (type.IsGenericType)
            {
                var i = type.Name.IndexOf('`');
                if (i != -1) m_NameText.text = type.Name.Substring(0, i);
                else m_NameText.text = type.Name;
                Add(new DSTextElement("<"));
                var margin = DocStyle.Current.MainTextSize / 2f;
                var args = type.GetGenericArguments();
                genericTypeName = new TypeNameElement[args.Length];
                i = 0;
                foreach (var t in args)
                {
                    genericTypeName[i] = new TypeNameElement(t);
                    Add(genericTypeName[i]);
                    i++;
                    var element = new DSTextElement(",");
                    element.style.marginRight = margin;
                    Add(element);
                }
                var last = ((DSTextElement)this[childCount - 1]);
                last.text = ">";
                last.style.marginRight = 0;
            }
            else
            {
                m_NameText.text = DocumentBuilderParser.GetTypeName(type);
            }
            
        }

        public override IEnumerable<TypeNameElement> VisitTypeName()
        {
            yield return this;
            if (genericTypeName != null)
            {
                foreach (var element in genericTypeName)
                {
                    foreach (var subElement in element.VisitTypeName())
                        yield return subElement;
                }
            }
        }

        public override IEnumerable<ParameterInfoElement> VisitParameter()
        {
            return Enumerable.Empty<ParameterInfoElement>();
        }
        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element, string id)> VisitMember()
        {
            yield return (TargetType, this, SOScriptAPIInfo.GetMemberID(TargetType));
            if (genericTypeName != null)
            {
                foreach (var element in genericTypeName)
                {
                    foreach (var subElement in element.VisitMember())
                        yield return subElement;
                }
            }
        }
    }
}
