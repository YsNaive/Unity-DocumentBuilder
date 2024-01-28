using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSFieldInfoElement : DSScriptAPIElement
    {
        public FieldInfo Target => m_Target;
        private FieldInfo m_Target;
        public DSTypeNameElement TypeText => m_TypeText;
        private DSTypeNameElement m_TypeText;
        public DSTextElement NameText => m_NameText;
        private DSTextElement m_NameText;
        public DSFieldInfoElement(FieldInfo fieldInfo)
            : base()
        {
            m_Target = fieldInfo;
            //var areaText = new DSTextElement(TypeReader.GetAccessLevel(fieldInfo));
            //areaText.style.color = DocStyle.Current.PrefixColor;

            m_TypeText = new DSTypeNameElement(fieldInfo.FieldType);
            //m_TypeText.style.marginLeft = DocStyle.Current.MainTextSize / 2f;

            m_NameText = new DSTextElement(fieldInfo.Name);
            m_NameText.style.color = DocStyle.Current.ArgsColor;
            m_NameText.style.marginLeft = DocStyle.Current.MainTextSize/2f;

            //Add(areaText);
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
            return Enumerable.Empty<DSParameterInfoElement>();
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            yield return (Target, m_NameText);
        }
    }

}