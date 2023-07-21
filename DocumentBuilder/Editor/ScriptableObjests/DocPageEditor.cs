using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.editor
{
    [CustomEditor(typeof(SODocPage))]
    public class DocPageEditor : Editor
    {
        SODocPage Target;
        VisualElement root;
        VisualElement docVisual;
        public override VisualElement CreateInspectorGUI()
        {
            Target = target as SODocPage;
            root = new VisualElement();
            root.style.width = new Length(100, LengthUnit.Percent);
            root.RegisterCallback<GeometryChangedEvent>(layout);
            return root;
        }
        void layout(GeometryChangedEvent e)
        {
            root.Add(new IMGUIContainer(() =>
            {
                var pages = serializedObject.FindProperty("SubPages");
                EditorGUILayout.PropertyField(pages);
            }));
            docVisual = new VisualElement();
            foreach (var doc in Target.Components)
            {
                var ve = new DocEditVisual(doc, (int)root.layout.width);
                ve.style.marginTop = 10;
                docVisual.Add(ve);
            }
            root.Add(docVisual);
            root.UnregisterCallback<GeometryChangedEvent>(layout);
        }

        private void OnDisable() { save(); }

        void save()
        {
            if (root == null) return;
            for(int i = 0; i < docVisual.childCount; i++) 
            {
                Target.Components[i].FromVisual(docVisual[i][1]);
            }
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
