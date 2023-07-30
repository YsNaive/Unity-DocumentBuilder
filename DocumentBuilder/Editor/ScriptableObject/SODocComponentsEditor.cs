using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocComponents))]
    public class SODocComponentsEditor : Editor
    {
        IMGUIContainer imgui;
        VisualElement root;
        SODocComponents Target;
        public override VisualElement CreateInspectorGUI()
        {
            Target = target as SODocComponents;
            root = new DocComponentsField(Target.Components);
            return root;
        }
        void save()
        {
            if (root == null) return;
            if (root[0].childCount == 0)
                Target.Components.Clear();
            else
                Target.Components = ((DocComponentsField)root).ToComponentsList();
            EditorUtility.SetDirty(target);
        }
        private void OnDisable()
        {
            save();
        }
    }
}
