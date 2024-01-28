using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSParameterInfoElement : DSScriptAPIElement
    {
        public ParameterInfo Target => m_Target;
        private ParameterInfo m_Target;
        public DSTypeNameElement TypeText => m_TypeText;
        private DSTypeNameElement m_TypeText;
        public DSTextElement NameText => m_NameText;
        private DSTextElement m_NameText;
        public DSParameterInfoElement(ParameterInfo paramInfo)
            : base()
        {
            m_Target = paramInfo;
            var padding = DocStyle.Current.MainTextSize / 2f;

            m_TypeText = new DSTypeNameElement(paramInfo.ParameterType);

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

        public override IEnumerable<DSTypeNameElement> VisitTypeName()
        {
            foreach (var ve in m_TypeText.VisitTypeName())
                yield return ve;
        }

        public override IEnumerable<DSParameterInfoElement> VisitParameter()
        {
            yield return this;
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            yield return (Target, m_NameText);
        }
    }

}