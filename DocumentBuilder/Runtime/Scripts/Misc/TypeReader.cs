using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NaiveAPI.DocumentBuilder
{
    public static class TypeReader
    {
        public static IEnumerable<Type> ActiveTypes => m_ActiveTypes;
        private static List<Type> m_ActiveTypes = new List<Type>();
        static TypeReader()
        {
            foreach(var asm in AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(asm => { return !DocRuntimeData.Instance.IgnoreAssemblyName.Contains(asm.GetName().Name); }))
            {
                foreach(var type in asm
                    .GetTypes()
                    .Where(type => { return !type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true); }))
                {
                    m_ActiveTypes.Add(type);
                }
            }
        }

        public const string Str_PublicConstructor = "Public Constructor";
        public const string Str_NonPublicConstructor = "Private Constructor";
        public const string Str_PublicField = "Public Field";
        public const string Str_NonPublicField = "Private Field";
        public const string Str_PublicStaticField = "Public Static Field";
        public const string Str_NonPublicStaticField = "Private Static Field";
        public const string Str_PublicProperty = "Public Property";
        public const string Str_NonPublicProperty = "Private Property";
        public const string Str_PublicStaticProperty = "Public Static Property";
        public const string Str_NonPublicStaticProperty = "Private Static Property";
        public const string Str_PublicMethod = "Public Method";
        public const string Str_NonPublicMethod = "Private Method";
        public const string Str_PublicStaticMethod = "Public Static Method";
        public const string Str_NonPublicStaticMethod = "Private Static Method";

        public const BindingFlags DeclaredPublicInstance  = BindingFlags.Public    | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public const BindingFlags DeclaredPublicStatic    = BindingFlags.Public    | BindingFlags.Static   | BindingFlags.DeclaredOnly;
        public const BindingFlags DeclaredPrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public const BindingFlags DeclaredPrivateStatic   = BindingFlags.NonPublic | BindingFlags.Static   | BindingFlags.DeclaredOnly;

        public static readonly Dictionary<Type, string> TypeNameTable = new Dictionary<Type, string> {
            { typeof(void)  , "void"    },
            { typeof(string), "string"  },
            { typeof(float) , "float"   },
            { typeof(int)   , "int"     },
            { typeof(uint)  , "uint"    },
            { typeof(short) , "short"   },
            { typeof(ushort), "ushort"  },
            { typeof(double), "double"  },
            { typeof(long)  , "long"    },
            { typeof(ulong) , "ulong"   },
            { typeof(bool)  , "bool"    },
            { typeof(object), "object" }};
        public static string GetPrefix(Type type)
        {
            var result = "";
            if (type == null) return result;
            if (type.IsPublic || type.IsNestedPublic)
                result += "public ";

            if (type.IsAbstract)
                result += "abstract ";
            else if (type.IsSealed)
                result += "sealed ";

            if (type.IsClass)
                result += "class ";
            else if (type.IsInterface)
                result += "interface ";
            else if (type.IsEnum)
                result += "enum ";
            else if (type.IsValueType)
                result += "struct ";

            return result;
        }
        public static string GetAccessLevel(FieldInfo fieldInfo)
        {
            var result = "";
            if (fieldInfo == null) return result;
            if (fieldInfo.IsPublic)
            {
                result+= "public ";
            }
            else if (fieldInfo.IsFamily)
            {
                result += "protected ";
            }
            else if (fieldInfo.IsPrivate)
            {
                result += "private ";
            }
            else if (fieldInfo.IsAssembly)
            {
                result += "internal ";
            }
            else if (fieldInfo.IsFamilyAndAssembly)
            {
                result += "protected internal ";
            }
            else if (fieldInfo.IsFamilyOrAssembly)
            {
                result += "protected internal ";
            }
            if(fieldInfo.IsLiteral)
                result += "const ";
            else if (fieldInfo.IsStatic)
                result += "static ";
            if (fieldInfo.IsInitOnly)
                result += "readonly ";
            return result;
        }
        public static string GetAccessLevel(MethodBase methodInfo)
        {
            if (methodInfo == null) return "";
            if (methodInfo.IsPublic)
            {
                return "public";
            }
            else if (methodInfo.IsFamily)
            {
                return "protected";
            }
            else if (methodInfo.IsPrivate)
            {
                return "private";
            }
            else if (methodInfo.IsAssembly)
            {
                return "internal";
            }
            else if (methodInfo.IsFamilyAndAssembly)
            {
                return "protected internal";
            }
            else if (methodInfo.IsFamilyOrAssembly)
            {
                return "protected internal";
            }

            return "";
        }
        public static string GetNestedName(Type type)
        {
            if (type == null) return "";
            string name = type.FullName;
            if (string.IsNullOrEmpty(name)) return "";
            name = name.Substring(name.LastIndexOf('.') + 1).Replace('+', '.');
            return name;
        }
        public static string GetGenericName(Type type)
        {
            if (type == null) return "";
            string name = type.Name;
            var index = name.IndexOf('`');
            if (index != -1)
                name = name.Substring(0, index);
            else
                return name;
            int i = 0;
            name += "<";
            foreach (var arg in type.GetGenericArguments())
            {
                name = $"{name}{((i != 0) ? ", " : "")}{GetName(arg)}";
                i++;
            }
            name += ">";
            return name;
        }
        public static string GetName(Type type)
        {
            if (type == null) return "";
            if(type.IsGenericParameter)return type.Name;
            if(type.IsGenericType)return GetGenericName(type);
            if(type.IsNested) return GetNestedName(type);
            string postfix = "";

            int index = type.Name.IndexOf("[");
            if (index != -1)
                postfix = type.Name.Substring(index);
            while (type.GetElementType() != null)
            {
                type = type.GetElementType();
            }

            if (TypeNameTable.ContainsKey(type))
            {
                return TypeNameTable[type] + postfix;
            }

            return type.Name + postfix;
        }
        public static string GetSignature(MethodBase methodInfo)
        {
            if (methodInfo == null) return "";
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(GetAccessLevel(methodInfo));
            if (methodInfo.IsStatic)
                stringBuilder.Append(" static");
            stringBuilder.Append(" ");
            if (methodInfo is MethodInfo)
            {
                stringBuilder.Append(GetGenericName(((MethodInfo)methodInfo).ReturnType)).Append(" ");
                stringBuilder.Append(methodInfo.Name);
            }
            else if (methodInfo is ConstructorInfo)
            {
                stringBuilder.Append(GetGenericName(((ConstructorInfo)methodInfo).DeclaringType)).Append(" ");
                stringBuilder.Append(GetGenericName(methodInfo.DeclaringType));
            }
            stringBuilder.Append('(');

            ParameterInfo[] parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                stringBuilder.Append(GetGenericName(parameters[i].ParameterType));
                stringBuilder.Append(" ");
                stringBuilder.Append(parameters[i].Name);
                if (i != parameters.Length - 1)
                    stringBuilder.Append(", ");
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }
        public static List<Type> FindAllTypesWhere(Func<Type, bool> where)
        {
            List<Type> ret = new List<Type>();
            foreach (var type in ActiveTypes)
            {
                if (where(type))
                    ret.Add(type);
            }
            return ret;
        }
        public static IEnumerable<(string memberType, IEnumerable<MemberInfo> members)> VisitDeclearedMember(Type type, Func<MemberInfo, bool> match = null)
        {
            if (match == null) match = m => true;
            foreach (var pair in new (string memberType, IEnumerable<MemberInfo> members)[]{
                new(Str_PublicConstructor,      type.GetConstructors(DeclaredPublicInstance)  .Where(match).OrderBy(selector=>{ return selector.Name.LevenshteinDistance("aeiouegh"); })),
                new(Str_NonPublicConstructor,   type.GetConstructors(DeclaredPrivateInstance) .Where(match).OrderBy(selector=>{ return selector.Name.LevenshteinDistance("aeiouegh"); })),
                new(Str_PublicField,            type.GetFields      (DeclaredPublicInstance)  .Where(match).OrderBy(selector=>{ return selector.ReflectedType; })),
                new(Str_NonPublicField,         type.GetFields      (DeclaredPrivateInstance) .Where(match).OrderBy(selector=>{ return selector.ReflectedType; })),
                new(Str_PublicProperty,         type.GetProperties  (DeclaredPublicInstance)  .Where(match).OrderBy(selector=>{ return selector.ReflectedType; })),
                new(Str_NonPublicProperty,      type.GetProperties  (DeclaredPrivateInstance) .Where(match).OrderBy(selector=>{ return selector.ReflectedType; })),
                new(Str_PublicMethod,           type.GetMethods     (DeclaredPublicInstance)  .Where(match).OrderBy(selector=>{ return selector.Name.LevenshteinDistance("aeiouegh"); })),
                new(Str_NonPublicMethod,        type.GetMethods     (DeclaredPrivateInstance) .Where(match).OrderBy(selector=>{ return selector.Name.LevenshteinDistance("aeiouegh"); })),
                new(Str_PublicStaticField,      type.GetFields      (DeclaredPublicStatic)    .Where(match).OrderBy(selector=>{ return selector.ReflectedType; })),
                new(Str_NonPublicStaticField,   type.GetFields      (DeclaredPrivateStatic)   .Where(match).OrderBy(selector=>{ return selector.ReflectedType; })),
                new(Str_PublicStaticProperty,   type.GetProperties  (DeclaredPublicStatic)    .Where(match).OrderBy(selector=>{ return selector.ReflectedType; })),
                new(Str_NonPublicStaticProperty,type.GetProperties  (DeclaredPrivateStatic)   .Where(match).OrderBy(selector=>{ return selector.ReflectedType; })),
                new(Str_PublicStaticMethod,     type.GetMethods     (DeclaredPublicStatic)    .Where(match).OrderBy(selector=>{ return selector.Name.LevenshteinDistance("aeiouegh"); })),
                new(Str_NonPublicStaticMethod,  type.GetMethods     (DeclaredPrivateStatic)   .Where(match).OrderBy(selector=>{ return selector.Name.LevenshteinDistance("aeiouegh"); })),
            })
            {
                if (pair.members.Any())
                {
                    yield return pair;
                }
            }
        }
    }
}