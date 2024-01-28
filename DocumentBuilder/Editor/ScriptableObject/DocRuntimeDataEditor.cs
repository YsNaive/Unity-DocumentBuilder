using NaiveAPI.DocumentBuilder;
using UnityEditor;
using UnityEngine.UIElements;
using NaiveAPI_UI;
using System;
using System.Linq;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(DocRuntimeData))]
    public class DocRuntimeDataEditor : Editor
    {
        DSScrollView root;
        VisualElement mainUI;
        VisualElement ignoreAsmEditUI;
        SerializedObject so;
        public override VisualElement CreateInspectorGUI()
        {
            

            so = new SerializedObject(target);
            root = new DSScrollView();
            root.style.SetIS_Style(ISPadding.Pixel(10));
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            initMain();
            initIgnoreAsmEdit();
            root.Add(mainUI);
            return root;
        }

        void initMain()
        {
            mainUI = new VisualElement();

            var curStyle = so.FindProperty("CurrentStyle");
            var curStyleField = DocEditor.NewObjectField<SODocStyle>("Current Style", evt => {
                curStyle.objectReferenceValue = evt.newValue; 
                so.ApplyModifiedProperties();
            });
            curStyleField.value = curStyle.objectReferenceValue;
            mainUI.Add(curStyleField);

            var ignoreAsmHor = new DSHorizontal();
            var ignoreAsmLabel = new DSTextElement("Ignore Assembly");
            ignoreAsmLabel.style.width = DocStyle.Current.LabelWidth;
            var ignoreAsmBtn = new DSButton("Edit", () =>
            {
                root.Clear();
                root.Add(ignoreAsmEditUI);
            });
            ignoreAsmBtn.style.flexGrow = 1;
            ignoreAsmHor.Add(ignoreAsmLabel);
            ignoreAsmHor.Add(ignoreAsmBtn);
            mainUI.Add(ignoreAsmHor);
        }
        void initIgnoreAsmEdit()
        {
            var ignoreAsm = so.FindProperty("m_IgnoreAssemblyName");
            ignoreAsmEditUI = new VisualElement();
            var backBtn = new DSButton("Back", () =>
            {
                root.Clear();
                root.Add(mainUI);
            });
            ignoreAsmEditUI.Add(backBtn);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().OrderBy(asm => { return asm.GetName().Name; }))
            {
                var name = asm.GetName().Name;
                var isIgnore = false;
                for(int i = 0,imax = ignoreAsm.arraySize; i < imax; i++)
                {
                    if(name == ignoreAsm.GetArrayElementAtIndex(i).stringValue)
                    {
                        isIgnore = true; 
                        break;
                    }
                }

                var toggle = new DSToggle(name);
                toggle.value = isIgnore;
                toggle.Insert(0, toggle[toggle.childCount - 1]);
                toggle[0].style.SetIS_Style(DocStyle.Current.ArrowIcon.Size);
                toggle[0].style.flexGrow = 0;
                toggle[0].style.marginRight = 7;
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if((evt.newValue != evt.previousValue))
                    {
                        if (evt.newValue)
                        {
                            var i = ignoreAsm.arraySize;
                            ignoreAsm.InsertArrayElementAtIndex(i);
                            ignoreAsm.GetArrayElementAtIndex(i).stringValue = name;
                        }
                        else
                        {
                            for (int i = 0, imax = ignoreAsm.arraySize; i < imax; i++)
                            {
                                if (name == ignoreAsm.GetArrayElementAtIndex(i).stringValue)
                                {
                                    ignoreAsm.DeleteArrayElementAtIndex(i);
                                    break;
                                }
                            }
                        }
                        so.ApplyModifiedProperties();
                    }
                });
                ignoreAsmEditUI.Add(toggle);
            }
        }
    }
}