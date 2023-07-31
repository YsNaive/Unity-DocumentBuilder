using NaiveAPI.DocumentBuilder;
using System.Collections;
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
        public Texture2D WhiteStar=>whiteStar;
        [SerializeField] Texture2D whiteStar;
    }

}