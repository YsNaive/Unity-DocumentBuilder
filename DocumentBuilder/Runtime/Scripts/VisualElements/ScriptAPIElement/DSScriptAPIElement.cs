using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public abstract class DSScriptAPIElement :DSHorizontal
    {
        public abstract IEnumerable<DSTypeNameElement> VisitTypeName();
        public abstract IEnumerable<DSParameterInfoElement> VisitParameter();
        public abstract IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember();
        public static DSScriptAPIElement Create(ICustomAttributeProvider info)
        {
            return info switch
            {
                FieldInfo asField => new DSFieldInfoElement(asField),
                PropertyInfo asProp => new DSPropertyInfoElement(asProp),
                MethodInfo asMethod => new DSMethodInfoElement(asMethod),
                ConstructorInfo asCtor => new DSMethodInfoElement(asCtor),
                ParameterInfo asParam => new DSParameterInfoElement(asParam),
                _ => null
            };
        }
    }
}
