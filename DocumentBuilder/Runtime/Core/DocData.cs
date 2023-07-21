using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public class DocData
    {
        public static List<string> ComponentNames
        {
            get
            {
                if (m_componentNameList.Count == 0)
                    ReloadComponentList();
                return m_componentNameList ;
            }
        }
        public static List<string> ComponentTypeNames
        {
            get
            {
                if (m_componentTypeStrList.Count == 0)
                    ReloadComponentList();
                return m_componentTypeStrList;
            }
        }
        public static Dictionary<string, DocVisual> ComponentInstanceDict
        {
            get
            {
                if (m_componentNameList.Count == 0)
                    ReloadComponentList();
                return m_componentInstanceDict;
            }
        }

        private static List<string> m_componentNameList = new List<string>();
        private static List<string> m_componentTypeStrList = new List<string>();
        private static Dictionary<string, DocVisual> m_componentInstanceDict = new Dictionary<string, DocVisual>();
        public static void ReloadComponentList()
        {
            m_componentNameList.Clear();
            m_componentTypeStrList.Clear();
            m_componentInstanceDict.Clear();
            m_componentNameList.Add("None");
            m_componentTypeStrList.Add("");
            Type baseType = typeof(DocVisual);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                    {
                        DocVisual doc = (DocVisual)System.Activator.CreateInstance(type);
                        m_componentNameList.Add(doc.DisplayName);
                        m_componentTypeStrList.Add(type.AssemblyQualifiedName);
                        m_componentInstanceDict.Add(type.AssemblyQualifiedName, doc);
                    }
                }
            }
        }
    }
}
