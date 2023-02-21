using System.Collections.Generic;
using UnityEngine;

namespace DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/Document Information")]
    public class SODocInformation : ScriptableObject
    {
        public enum DocumentType
        {
            Method,
            Class,
            Folder,
            QuestionMark,
            Text,
            Tips,
            Book,
            System,
            ItemElement,

            Custom
        }
        public string Name;
        public DocumentType DocType = DocumentType.Custom;
        [HideInInspector]
        public Texture2D MenuIcon;
        public List<SODocInformation> SubPages = new List<SODocInformation>();
        [HideInInspector]
        public List<DocComponent> Components = new List<DocComponent>();
    }
}
