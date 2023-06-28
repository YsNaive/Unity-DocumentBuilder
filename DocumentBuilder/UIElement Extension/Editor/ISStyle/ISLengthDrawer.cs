using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.drawer
{
    [CustomPropertyDrawer(typeof(ISLength), true)]
    public class ISLengthDrawer : PropertyDrawer
    {
        SerializedProperty unit;
        SerializedProperty value;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            unit = property.FindPropertyRelative("Unit");
            value = property.FindPropertyRelative("Value");
            Rect rect = position;
            rect.width = position.width - 65;
            EditorGUI.PropertyField(rect, value, label);
            rect.x = rect.xMax+5;
            rect.width = 60;
            unit.enumValueIndex = GUI.SelectionGrid(rect, unit.enumValueIndex, new string[] { "Px", "%" }, 2);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18;
        }
    }
}
