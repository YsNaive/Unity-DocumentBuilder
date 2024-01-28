using NaiveAPI.DocumentBuilder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomPropertyDrawer(typeof(DocComponent))]
    public class DocComponentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string name;
            DocEditVisual.Dict.ID2Name.TryGetValue(property.FindPropertyRelative("VisualID").stringValue, out name);
            EditorGUI.LabelField(position, DocRuntime.VisualID_Dict[label.text].Name);
            position.x = position.width - 75;
            position.width = 75;
            if (GUI.Button(position, "edit"))
            {
                SerializedDocComponentEditWindow.Create(property, label.text);
            }
        }
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SerializedDocEditVisual(property);
        }
    }

}