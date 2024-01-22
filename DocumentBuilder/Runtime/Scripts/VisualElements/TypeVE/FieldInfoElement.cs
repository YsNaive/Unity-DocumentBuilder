using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class FieldInfoElement : ScriptAPIElement
    {
        public FieldInfo Target => m_Target;
        private FieldInfo m_Target;
        public TypeNameElement TypeText => m_TypeText;
        private TypeNameElement m_TypeText;
        public DSTextElement NameText => m_NameText;
        private DSTextElement m_NameText;
        public FieldInfoElement(FieldInfo fieldInfo)
            : base()
        {
            m_Target = fieldInfo;
            var areaText = new DSTextElement(TypeReader.GetAccessLevel(fieldInfo));
            areaText.style.color = DocStyle.Current.PrefixColor;

            m_TypeText = new TypeNameElement(fieldInfo.FieldType);
            m_TypeText.style.marginLeft = DocStyle.Current.MainTextSize / 2f;

            m_NameText = new DSTextElement(fieldInfo.Name);
            m_NameText.style.color = DocStyle.Current.ArgsColor;
            m_NameText.style.marginLeft = DocStyle.Current.MainTextSize/2f;

            Add(areaText);
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
            yield return (Target, this, SOScriptAPIInfo.GetMemberID(Target));
        }
    }

}