using NaiveAPI.DocumentBuilder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocComponents))]
    public class SODocComponentsEditor : Editor
    {
        DocComponentsField editField;
        public override VisualElement CreateInspectorGUI()
        {
            var so = new SerializedObject(target);
            VisualElement root = new VisualElement();
            editField = new DocComponentsField(so.FindProperty("Components"));
            root.Add(new IMGUIContainer(OnInspectorGUI));
            root.Add(editField);
            return root;
        }
    }
}
