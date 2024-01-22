using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class ParameterInfoElement : ScriptAPIElement
    {
        public ParameterInfo Target => m_Target;
        private ParameterInfo m_Target;
        public TypeNameElement TypeText => m_TypeText;
        private TypeNameElement m_TypeText;
        public DSTextElement NameText => m_NameText;
        private DSTextElement m_NameText;
        public ParameterInfoElement(ParameterInfo paramInfo)
            : base()
        {
            m_Target = paramInfo;
            var padding = DocStyle.Current.MainTextSize / 2f;

            m_TypeText = new TypeNameElement(paramInfo.ParameterType);

            m_NameText = new DSTextElement(paramInfo.Name);
            m_NameText.style.color = DocStyle.Current.ArgsColor;
            m_NameText.style.marginLeft = padding;

            DSTextElement areaText = new();
            if (paramInfo.IsIn)
                areaText = new DSTextElement("in");
            else if(paramInfo.IsOut)
                areaText = new DSTextElement("out");
            if(areaText.text != "")
            {
                areaText.style.color = DocStyle.Current.PrefixColor;
                Add(areaText);
                m_TypeText.style.marginLeft = padding;
            }

            Add(m_TypeText);
            Add(m_NameText);
        }

        public override IEnumerable<TypeNameElement> VisitTypeName()
        {
            yield return m_TypeText;
        }

        public override IEnumerable<ParameterInfoElement> VisitParameter()
        {
            yield return this;
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element, string id)> VisitMember()
        {
            yield return (Target, this, SOScriptAPIInfo.GetMemberID(Target));
        }
    }

}