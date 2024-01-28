using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public class DocRuntimeData : ScriptableObject
    {
        public static DocRuntimeData Instance
        {
            get
            {
                if (instance == null)
                    instance = (DocRuntimeData)Resources.Load("DocRuntimeData");
                return instance;
            }
        }
        private static DocRuntimeData instance;

        public SODocStyle CurrentStyle;
        public IEnumerable<string> IgnoreAssemblyName => m_IgnoreAssemblyName;
        [SerializeField] List<string> m_IgnoreAssemblyName;
    }
}
