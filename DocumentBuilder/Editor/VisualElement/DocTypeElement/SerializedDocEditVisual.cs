using NaiveAPI.DocumentBuilder;
using UnityEditor;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class SerializedDocEditVisual : VisualElement
    {
        SerializedProperty sp;
        SerializedProperty id,json,texts,objs;
        DocComponent doc;
        public SerializedDocEditVisual(SerializedProperty serializedProperty)
        {
            sp = serializedProperty;
            id = sp.FindPropertyRelative("VisualID");
            json = sp.FindPropertyRelative("JsonData");
            texts = sp.FindPropertyRelative("TextData");
            objs = sp.FindPropertyRelative("ObjsData");
            Add(new DocComponentField(new DocComponentProperty(fromSO()), true));
        }
        DocComponent fromSO()
        {
            doc = new DocComponent();
            doc.VisualID = id.stringValue;
            doc.JsonData = json.stringValue;
            for(int i = 0; i < texts.arraySize; i++)
            {
                doc.TextData.Add(texts.GetArrayElementAtIndex(i).stringValue);
            }
            for (int i = 0; i < objs.arraySize; i++)
            {
                doc.ObjsData.Add(objs.GetArrayElementAtIndex(i).objectReferenceValue);
            }
            return doc;
        }
        public void ApplyChange()
        {
            id.stringValue = doc.VisualID;
            json.stringValue = doc.JsonData;
            texts.ClearArray();
            int i = 0;
            foreach(var str in doc.TextData)
            {
                texts.InsertArrayElementAtIndex(i);
                texts.GetArrayElementAtIndex(i).stringValue = str;
                i++;
            }
            i = 0;
            foreach(var obj in doc.ObjsData)
            {
                objs.InsertArrayElementAtIndex(i);
                objs.GetArrayElementAtIndex(i).objectReferenceValue = obj;
                i++;
            }
            sp.serializedObject.ApplyModifiedProperties();
        }
    }
}
