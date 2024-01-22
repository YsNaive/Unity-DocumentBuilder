using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public static class TypeReader
    {
        public const BindingFlags DeclaredPublicInstance  = BindingFlags.Public    | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public const BindingFlags DeclaredPublicStatic    = BindingFlags.Public    | BindingFlags.Static   | BindingFlags.DeclaredOnly;
        public const BindingFlags DeclaredPrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public const BindingFlags DeclaredPrivateStatic   = BindingFlags.NonPublic | BindingFlags.Static   | BindingFlags.DeclaredOnly;

        public static readonly Dictionary<Type, string> TypeNameTable = new Dictionary<Type, string> {
            { typeof(string) , "string"},
            { typeof(void), "void" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(short), "short" },
            { typeof(double), "double" },
            { typeof(long), "long" },
            { typeof(bool), "bool" }};
        public static string GetAccessLevel(FieldInfo fieldInfo)
        {
            if (fieldInfo == null) return "";
            if (fieldInfo.IsPublic)
            {
                return "public";
            }
            else if (fieldInfo.IsFamily)
            {
                return "protected";
            }
            else if (fieldInfo.IsPrivate)
            {
                return "private";
            }
            else if (fieldInfo.IsAssembly)
            {
                return "internal";
            }
            else if (fieldInfo.IsFamilyAndAssembly)
            {
                return "protected internal";
            }
            else if (fieldInfo.IsFamilyOrAssembly)
            {
                return "protected internal";
            }

            return "";
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
        public static string CalNestedName(Type type)
        {
            if (type == null) return "";
            string name = type.FullName;
            name = name.Substring(name.LastIndexOf('.') + 1).Replace('+', '.');
            return name;
        }
        public static string GetGenericName(Type type)
        {
            if (type == null) return "";
            string name = type.Name;
            int i = name.IndexOf('`');
            if (i != -1)
                name = name.Substring(0, i);
            else
                return GetName(type);
            i = 0;
            name += "<";
            foreach (var arg in type.GetGenericArguments())
            {
                name += ((i != 0) ? ", " : "") + GetName(arg);
                i++;
            }
            name += ">";
            return name;
        }
        public static string GetName(Type type)
        {
            if (type == null) return "";
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

    }

}