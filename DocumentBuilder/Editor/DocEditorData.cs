using NaiveAPI.DocumentBuilder;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditorData : ScriptableObject
    {
        public static DocEditorData Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.LoadAll<DocEditorData>("")[0];
                return instance;
            }
        }
        private static DocEditorData instance;
        public SODocPage EditingDocPage;
        public DefaultAsset DocTemplateFolder;
        public List<Texture2D> BuildinIcon;
        public SODocPage DocumentBuilderDocsRoot;

        [SerializeField] private Texture2D classIcon;
        [SerializeField] private Texture2D structIcon;
        [SerializeField] private Texture2D methodIcon;
        [SerializeField] private Texture2D starIcon;
        [SerializeField] private Texture2D eyeIcon;
        [SerializeField] private Texture2D closeEyeIcon;
        [SerializeField] private Texture2D penIcon;

        public static class Icon
        {
            public static Texture2D ClassIcon => Instance.classIcon;
            public static Texture2D StructIcon => Instance.structIcon;
            public static Texture2D MethodIcon => Instance.methodIcon;
            public static Texture2D StarIcon => Instance.starIcon;
            public static Texture2D EyeIcon => Instance.eyeIcon;
            public static Texture2D CloseEyeIcon => Instance.closeEyeIcon;
            public static Texture2D PenIcon => Instance.penIcon;
        }
    }

}