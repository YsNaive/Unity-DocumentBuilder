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

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocComponents))]
    public class SODocComponentsEditor : Editor
    {
        SODocComponents Target;
        DocComponentsField editField;
        public override void OnInspectorGUI()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.control)
                    editField.CtrlHotKeyAction(Event.current.keyCode);
            }
        }
        [SerializeField] private List<DocComponent> undoBuffer;
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = DocRuntime.NewEmpty();
            Target = target as SODocComponents;
            if(Target.Components == null) Target.Components = new List<DocComponent>();
            editField = new DocComponentsField(Target.Components);

            editField.OnModify += (doc) => {
                Undo.IncrementCurrentGroup();
                Undo.RegisterCompleteObjectUndo(this, "DocComponentsField");
                undoBuffer = editField.ToComponentsList();
            };
            Undo.undoRedoPerformed += ()=> { editField.Repaint(undoBuffer); };
            root.Add(new IMGUIContainer(OnInspectorGUI));
            root.Add(editField);
            return root;
        }
        void save()
        {
            if (editField == null) return;
            if (editField[0].childCount == 0)
                Target.Components.Clear();
            else
                Target.Components = editField.ToComponentsList();
            EditorUtility.SetDirty(target);
        }
        private void OnDisable()
        {
            save();
        }
    }
}
