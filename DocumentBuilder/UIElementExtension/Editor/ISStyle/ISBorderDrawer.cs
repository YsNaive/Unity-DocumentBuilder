using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.drawer
{
    [CustomPropertyDrawer(typeof(ISBorder), true)]
    public class ISBorderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = position;
            rect.height = 18;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
            if (!property.isExpanded) return;
            rect.x+=15;
            rect.width-=15;
            rect.width /=2;
            Rect colorRect = rect;
            colorRect.x = colorRect.xMax;
            colorRect.x += 10;
            colorRect.width -= 10;
            EditorGUIUtility.labelWidth = 40;
            rect.y = rect.yMax;
            colorRect.y = colorRect.yMax;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Left"));
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("LeftColor"), GUIContent.none);
            rect.y = rect.yMax;
            colorRect.y = colorRect.yMax;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Top"));
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("TopColor"), GUIContent.none);
            rect.y = rect.yMax;
            colorRect.y = colorRect.yMax;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Right"));
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("RightColor"), GUIContent.none);
            rect.y = rect.yMax;
            colorRect.y = colorRect.yMax;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Bottom"));
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("BottomColor"), GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded? 90:18;
        }
    }
}
