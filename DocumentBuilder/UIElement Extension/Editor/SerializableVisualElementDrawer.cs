using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.drawer
{
    [CustomPropertyDrawer(typeof(SerializableVisualElement<>), true)]
    public class SerializableVisualElementDrawer : PropertyDrawer
    {
        SerializedProperty uid;
        SerializedProperty elementName;
        VisualElement element;
        GUIContent empty = new GUIContent();
        Rect rect;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            rect = position;
            uid = property.FindPropertyRelative("UIDocument");
            elementName = property.FindPropertyRelative("elementName");

            if (uid.objectReferenceValue != null) rect.width = EditorGUIUtility.labelWidth + 50;
            else rect.width = position.width;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, uid, label);
            if (EditorGUI.EndChangeCheck())
                element = null;
            rect.x = rect.xMax;
            rect.width = position.width - rect.x -5;
            if (uid.objectReferenceValue != null)
                element = (uid.objectReferenceValue as UIDocument).rootVisualElement?.Q(elementName.stringValue);
            element = visualElementPopup(rect, empty, element, uid.objectReferenceValue as UIDocument,
                                                       Type.GetType(property.FindPropertyRelative("typeName").stringValue));
            rect.x = position.xMax - 15;
            rect.width = 20;
            if (element != null)
            {
                elementName.stringValue = element.name;
                element.visible = GUI.Toggle(rect, element.visible,"");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18;
        }

        private static VisualElement visualElementPopup(Rect position, GUIContent label, VisualElement obj, UIDocument uid, Type filter = null)
        {
            if (uid == null) return default;
            if (uid.rootVisualElement == null) return default;
            if (uid.rootVisualElement.name == null) return default;

            List<VisualElement> selectableTargets = findElementTargets(uid.rootVisualElement, filter);
            string[] popup = new string[selectableTargets.Count];
            int index = 0;
            for (int i = 0; i < selectableTargets.Count; i++)
            {
                if (selectableTargets[i].name == "") continue;
                popup[i] = selectableTargets[i].name;
                if (obj == selectableTargets[i])
                    index = i;
            }
            index = EditorGUI.Popup(position, index, popup);

            return index < selectableTargets.Count ? selectableTargets[index] : null;
        }
        private static List<VisualElement> findElementTargets(VisualElement root, Type filter)
        {
            return findElementTargets(root, new List<VisualElement>(), filter);
        }
        private static List<VisualElement> findElementTargets(VisualElement root, List<VisualElement> list, Type filter = null)
        {
            if (filter == typeof(VisualElement) || root.GetType() == filter)
                list.Add(root);
            foreach (VisualElement ve in root.Children())
                findElementTargets(ve, list, filter);
            return list;
        }

    }
}
