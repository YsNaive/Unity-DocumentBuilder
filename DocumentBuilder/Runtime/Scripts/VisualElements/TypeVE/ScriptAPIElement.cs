using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public abstract class ScriptAPIElement :DSHorizontal
    {
        public abstract IEnumerable<TypeNameElement> VisitTypeName();
        public abstract IEnumerable<ParameterInfoElement> VisitParameter();
        public abstract IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element, string id)> VisitMember();
    }
}
