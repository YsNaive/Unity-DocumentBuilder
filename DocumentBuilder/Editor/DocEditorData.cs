using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditorData : ScriptableSingleton<DocEditorData>
    {
        static DocEditorData()
        {
        }
        public SODocPage EditingDocPage;
        public string EditingState;
    }

}