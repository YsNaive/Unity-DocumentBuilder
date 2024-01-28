using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DSTypeElement : DSScriptAPIElement
    {
        List<DSScriptAPIElement> scriptAPIElements = new();
        public SOScriptAPIInfo APIInfo => apiInfo;
        SOScriptAPIInfo apiInfo;
        List<GridView> gridViews = new();
        public DSTypeElement(Type type)
        {
            style.flexDirection = FlexDirection.Column;
            ScriptAPIInfoHolder.Infos.TryGetValue(type, out apiInfo);

            var fontSize = DocStyle.Current.MainTextSize;
            DocStyle.Current.MainTextSize = (int)(DocStyle.Current.MainTextSize * 1.5f);
            var typeElement = new DSTypeNameElement(type);
            Add(typeElement);
            if( type.BaseType != null &&
                type.BaseType != typeof(object) &&
                type.BaseType != typeof(ValueType))
            {
                scriptAPIElements.Add(new DSTypeNameElement(type.BaseType));
                scriptAPIElements[^1].style.marginLeft = DocStyle.Current.MainTextSize / 2;
                typeElement.Add(new DSTextElement(" :"));
                typeElement.Add(scriptAPIElements[^1]);
            }
            DocStyle.Current.MainTextSize = fontSize;

            var abstractText = new DSTextElement(TypeReader.GetPrefix(type));
            abstractText.style.color = DocStyle.Current.PrefixColor;
            Add(abstractText);

            var fullname = new DSTextElement($"{type.Namespace}{{ {TypeReader.GetName(type)} }} in {type.Assembly.GetName().Name}");
            fullname.style.opacity = 0.6f;
            Add(fullname);
            Func<ICustomAttributeProvider, bool> displayCheck = attr =>
            {
                if (typeof(MethodBase).IsAssignableFrom(attr.GetType()))
                {
                    var method = attr as MethodBase;
                    if (method.Name[0] == '<') return false;
                    if (method.Name.StartsWith("op_")) return false;
                    if (method.Name.StartsWith("get_")) return false;
                    if (method.Name.StartsWith("set_")) return false;
                    if (method.Name.StartsWith("add_")) return false;
                    if (method.Name.StartsWith("remove_")) return false;
                }
                if (apiInfo == null)
                {
                    if (attr.IsDefined(typeof(ObsoleteAttribute), false))
                        return false;
                    return attr switch
                    {
                        MethodBase asMethod => asMethod.IsPublic && !(
                            asMethod.Name.StartsWith("Equals")      |
                            asMethod.Name.StartsWith("ToString")    |
                            asMethod.Name.StartsWith("GetHashCode")
                        ),
                        FieldInfo asField => asField.IsPublic,
                        PropertyInfo asProp => asProp.GetAccessors().Any(accessor => { return accessor.IsPublic; }),
                        _ => false
                    };
                }
                return apiInfo.GetMemberInfo(attr).IsDisplay;
            };
            if (apiInfo != null)
            {
                var description = new VisualElement();
                description.style.marginLeft = DocStyle.Current.LineHeight;
                foreach (var com in apiInfo.Description)
                    description.Add(DocVisual.Create(com));
                Add(description);
            }

            if (!type.IsEnum)
            {
                Add(DocVisual.Create(DocDividline.CreateComponent()));
                foreach (var pair in TypeReader.VisitDeclearedMember(type, displayCheck))
                    Add(createMemberInfos(pair.memberType, pair.members));
            }
            else
            {
                var vals = type.GetEnumNames();
                var com = DocItems.CreateComponent(vals, DocItems.Mode.Disorder);
                Add(DocVisual.Create(com));
            }
        }
        VisualElement createMemberInfos(string title, IEnumerable<MemberInfo> infos)
        {
            var root = new VisualElement();
            var titleText = new DSTextElement(title);
            titleText.style.opacity = 0.6f;
            root.Add(titleText);
            var grid = new GridView(infos.Count(), 2, DocStyle.Current.SubBackgroundColor, GridView.AlignMode.FixedContent);
            grid.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
            grid.style.marginLeft = DocStyle.Current.LineHeight.Value;
            int i = 0;
            foreach (var info in infos)
            {
                grid[i, 0].Add(createMemberInfo(info));
                if (apiInfo != null)
                {
                    foreach (var com in apiInfo.GetMemberInfo(info).Tooltip)
                        grid[i, 1].Add(DocVisual.Create(com));
                }
                i++;
            }
            root.Add(grid);
            gridViews.Add(grid);
            return root;
        }
        VisualElement createMemberInfo(MemberInfo info)
        {
            VisualElement container = new DSHorizontal();
            DSScriptAPIElement title = DSScriptAPIElement.Create(info);
            scriptAPIElements.Add(title);
            return title;
        }
        public override IEnumerable<DSTypeNameElement> VisitTypeName()
        {
            foreach (var ve in scriptAPIElements)
            {
                foreach (var it in ve.VisitTypeName())
                    yield return it;
            }
        }
        public override IEnumerable<DSParameterInfoElement> VisitParameter()
        {
            foreach (var ve in scriptAPIElements)
            {
                foreach (var it in ve.VisitParameter())
                    yield return it;
            }
        }
        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            foreach (var ve in scriptAPIElements)
            {
                foreach (var it in ve.VisitMember())
                    yield return it;
            }
        }
    }
}
