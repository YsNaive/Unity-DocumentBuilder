using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSPropertyInfoElement : DSScriptAPIElement
    {
        public PropertyInfo Target => m_Target;
        private PropertyInfo m_Target;
        public DSTextElement AreaText => m_AreaText;
        private DSTextElement m_AreaText;
        public DSTypeNameElement TypeText => m_TypeText;
        private DSTypeNameElement m_TypeText;
        public DSTextElement NameText => m_NameText;
        private DSTextElement m_NameText;
        public DSPropertyInfoElement(PropertyInfo propertyInfo)
            : base()
        {
            m_Target = propertyInfo;
            var padding = DocStyle.Current.MainTextSize / 2f;
            var getText = new DSTextElement("get");
            getText.style.color = DocStyle.Current.PrefixColor;
            getText.style.opacity = propertyInfo.CanRead ? 1f : 0.4f;

            var setText = new DSTextElement("set");
            setText.style.color = DocStyle.Current.PrefixColor;
            setText.style.opacity = propertyInfo.CanWrite ? 1f : 0.4f;
            setText.style.marginLeft = padding;

            m_TypeText = new DSTypeNameElement(propertyInfo.PropertyType);
            m_TypeText.style.color = DocStyle.Current.TypeColor;
            m_TypeText.style.marginLeft = padding;

            m_NameText = new DSTextElement(propertyInfo.Name);
            m_NameText.style.color = DocStyle.Current.ArgsColor;
            m_NameText.style.marginLeft = padding;
            Add(getText);
            Add(setText);
            Add(m_AreaText);
            Add(m_TypeText);
            Add(m_NameText);
        }

        public override IEnumerable<DSTypeNameElement> VisitTypeName()
        {
            foreach(var ve in m_TypeText.VisitTypeName())
                yield return ve;
        }

        public override IEnumerable<DSParameterInfoElement> VisitParameter()
        {
            return Enumerable.Empty<DSParameterInfoElement>();
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            yield return (Target, NameText);
        }
    }
}
