using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaiveAPI_UI;

namespace NaiveAPI_Editor.drawer
{
    [CustomPropertyDrawer(typeof(ISBackground))]
    public class ISBackgroundDrawer : PropertyDrawer
    {
        private static string[] options = new string[] { "Sprite", "Texture", "RenderTexture", "VectorImage" };
        private SerializedProperty[] propertys = new SerializedProperty[options.Length];
        private int index = -1;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            propertys[0] = property.FindPropertyRelative("sprite");
            propertys[1] = property.FindPropertyRelative("texture");
            propertys[2] = property.FindPropertyRelative("renderTexture");
            propertys[3] = property.FindPropertyRelative("vectorImage");
            Rect rect = position;
            rect.height = 20;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
            if (!property.isExpanded) return;
            rect.x+= 15;
            rect.width -= 15;
            rect.y = rect.yMax;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Color"));
            rect.y = rect.yMax;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("ImageTint"));
            rect.y = rect.yMax;
            Rect imageRect = rect;
            imageRect.y = imageRect.yMax;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("ScaleMode"));
            imageRect.width -= 85;

            if (index == -1)
            {
                index++;
                foreach (var sp in propertys)
                {
                    if (sp.objectReferenceValue != null)
                        break;
                    index++;
                }
                if (index >= options.Length) index = 0;
            }
            EditorGUI.PropertyField(imageRect, propertys[index]);
            imageRect.x = imageRect.xMax;
            imageRect.width = 85;
            int newindex = EditorGUI.Popup(imageRect, index, options);
            if (index != newindex)
            {
                index = newindex;
                foreach (var sp in propertys)
                    sp.objectReferenceValue = null;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? 100 : 18;
        }
    }

}