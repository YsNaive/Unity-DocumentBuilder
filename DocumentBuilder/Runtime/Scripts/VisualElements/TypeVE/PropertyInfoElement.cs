using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class PropertyInfoElement : ScriptAPIElement
    {
        public PropertyInfo Target => m_Target;
        private PropertyInfo m_Target;
        public DSTextElement AreaText => m_AreaText;
        private DSTextElement m_AreaText;
        public TypeNameElement TypeText => m_TypeText;
        private TypeNameElement m_TypeText;
        public DSTextElement NameText => m_NameText;
        private DSTextElement m_NameText;
        public PropertyInfoElement(PropertyInfo propertyInfo)
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

            m_TypeText = new TypeNameElement(propertyInfo.PropertyType);
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

        public override IEnumerable<TypeNameElement> VisitTypeName()
        {
            yield return m_TypeText;
        }

        public override IEnumerable<ParameterInfoElement> VisitParameter()
        {
            return Enumerable.Empty<ParameterInfoElement>();
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element, string id)> VisitMember()
        {
            yield return (Target, NameText, SOScriptAPIInfo.GetMemberID(Target));
        }
    }
}
