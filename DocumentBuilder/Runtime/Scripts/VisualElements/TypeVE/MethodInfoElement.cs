using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class MethodInfoElement : ScriptAPIElement
    {
        public MethodBase Target => m_Target;
        private MethodBase m_Target;
        public TypeNameElement ReturnTypeText => m_ReturnTypeText;
        private TypeNameElement m_ReturnTypeText;
        public DSTextElement NameText => m_NameText;
        private DSTextElement m_NameText;
        public ParameterInfoElement[] ParamTexts => m_ParamTexts;
        private ParameterInfoElement[] m_ParamTexts;
        public MethodInfoElement(MethodBase info)
            : base()
        {
            m_Target = info;
            style.flexWrap = Wrap.Wrap;
            var padding = DocStyle.Current.MainTextSize / 2f;
            var areaText = new DSTextElement(TypeReader.GetAccessLevel(info));
            areaText.style.color = DocStyle.Current.PrefixColor;

            m_ReturnTypeText = new TypeNameElement(info switch
            {
                MethodInfo asMethod => asMethod.ReturnType,
                ConstructorInfo asConstructor => asConstructor.ReflectedType,
                _ => null
            });
            m_ReturnTypeText.style.marginLeft = padding;


            var paramInfos = info.GetParameters();
            m_ParamTexts = new ParameterInfoElement[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; i++)
                m_ParamTexts[i] = new ParameterInfoElement(paramInfos[i]);

            Add(areaText);
            Add(m_ReturnTypeText);
            if (!info.IsConstructor)
            {
                m_NameText = new DSTextElement(info.Name);
                m_NameText.style.color = DocStyle.Current.FuncColor;
                m_NameText.style.marginLeft = padding;
                Add(m_NameText);
            }
            Add(new DSTextElement("("));
            if(m_ParamTexts.Length != 0)
            {
                Add(m_ParamTexts[0]);
                for (int i = 1; i < paramInfos.Length; i++)
                {
                    var comma = new DSTextElement(",");
                    var param = m_ParamTexts[i];
                    param.style.marginLeft = padding;
                    Add(comma);
                    Add(param);
                }
            }
            Add(new DSTextElement(")"));
        }

        public override IEnumerable<TypeNameElement> VisitTypeName()
        {
            yield return m_ReturnTypeText;
        }

        public override IEnumerable<ParameterInfoElement> VisitParameter()
        {
            foreach(var param in m_ParamTexts)
                yield return param;
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element, string id)> VisitMember()
        {
            yield return (Target, (Target.IsConstructor ? m_ReturnTypeText : m_NameText), SOScriptAPIInfo.GetMemberID(Target));
            foreach (var param in m_ParamTexts)
                yield return (param.Target, param, SOScriptAPIInfo.GetMemberID(param.Target));

        }
    }
}