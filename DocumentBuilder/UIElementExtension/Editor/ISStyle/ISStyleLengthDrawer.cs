using NaiveAPI_UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.drawer
{
    [CustomPropertyDrawer(typeof(ISStyleLength), true)]
    public class ISStyleLengthDrawer : PropertyDrawer
    {
        SerializedProperty keyword;
        SerializedProperty unit;
        SerializedProperty value;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            keyword = property.FindPropertyRelative("Keyword");
            unit = property.FindPropertyRelative("Value").FindPropertyRelative("Unit");
            value = property.FindPropertyRelative("Value").FindPropertyRelative("Value");
            Rect rect = position;
            rect.width = position.width - 125;
            rect.x = rect.xMax + 5;
            rect.width = 120;
            int index;
            if (keyword.enumValueIndex == 2)
                index = 2;
            else if (keyword.enumValueIndex == 4)
                index = 3;
            else
                index = unit.enumValueIndex;
            index = keyword.enumValueIndex = GUI.SelectionGrid(rect, index, new string[] { "Px", "%", "Au", "In" }, 4);
            if (index <= 1)
            {
                keyword.enumValueIndex = 0;
                unit.enumValueIndex = index;
            }
            else
            {
                if(index == 2)
                    keyword.enumValueIndex = 2;
                else
                    keyword.enumValueIndex = 4;
            }


            rect = position;
            rect.width = position.width - 125;
            if(keyword.enumValueIndex != 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                    if (keyword.enumValueIndex == 2)
                        EditorGUI.TextField(rect, label, "Auto");
                    else
                        EditorGUI.TextField(rect, label, "Initial");
                EditorGUI.EndDisabledGroup();
            }
            else
                EditorGUI.PropertyField(rect, value, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18;
        }
    }
}
