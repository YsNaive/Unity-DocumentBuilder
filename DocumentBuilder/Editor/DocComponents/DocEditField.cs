using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{

    public class DocEditField : VisualElement
    {
        public event Action<Type> OnDocTypeChange;
        public DocComponent Target;
        public DocEditField(DocComponent docComponent)
        {
            style.ClearMarginPadding();
            Target = docComponent;
            createDropfield();
            Repaint();
        }
        public DropdownField SelectVisualType;
        private void createDropfield()
        {
            SelectVisualType = new DropdownField();
            SelectVisualType.choices = DocEditor.NameList;
            SelectVisualType.style.ClearMarginPadding();
            string tName = string.Empty;
            DocEditor.ID2Name.TryGetValue(Target.VisualID, out tName);
            SelectVisualType.index = DocEditor.NameList.FindIndex(0, (str) => { return str == tName; });
            if (SelectVisualType.index == -1) SelectVisualType.index = 0;
            SelectVisualType.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.JsonData = string.Empty;
                Target.ObjsData.Clear();
                RemoveAt(1);
                if (val.newValue == "None")
                    Target.VisualID = string.Empty;
                else
                    Target.VisualID = DocEditor.Name2ID[val.newValue];
                Repaint();
            });
            SelectVisualType.value = DocEditor.NameList[SelectVisualType.index];
            Add(SelectVisualType);
        }
        VisualElement editFavoriteRoot;
        public void Repaint()
        {
            Type docType = null;
            Add(SelectVisualType);
            if (DocEditor.ID2Type.TryGetValue(Target.VisualID, out docType))
            {
                DocEditVisual doc = (DocEditVisual)Activator.CreateInstance(docType);
                doc.SetTarget(Target);
                Add(doc);
            }
            else
            {
                VisualElement visualElement = DocRuntime.NewEmptyHorizontal();
                visualElement.style.marginTop = 5;
                visualElement.style.alignItems = Align.Center;
                visualElement.style.flexWrap = Wrap.Wrap;
                Button editFavorite = DocRuntime.NewButton("", () =>
                {
                    if(editFavoriteRoot != null)
                    {
                        Remove(editFavoriteRoot);
                        editFavoriteRoot = null;
                    }
                    else
                    {
                        createEditFavorite();
                        Add(editFavoriteRoot);
                    }
                });
                editFavorite.style.backgroundImage = DocEditorData.Instance.WhiteStar;
                editFavorite.style.unityBackgroundImageTintColor = new Color(.8f, .6f, .2f);
                editFavorite.style.width = 18;
                editFavorite.style.height = 18;
                visualElement.Add(editFavorite);
                foreach (var id in DocEditorData.Instance.FavoriteDocVisualID)
                {
                    string name;
                    if(DocEditor.ID2Name.TryGetValue(id, out name))
                    {
                        if (name == "None") continue;

                        string displayName;
                        int i = name.LastIndexOf('/');
                        if(i != -1)
                            displayName = name.Substring(i+1);
                        else displayName = name;
                        Button button = DocRuntime.NewButton(displayName, () =>
                        {
                            SelectVisualType.value = name;
                        });
                        button.style.marginLeft = 5;
                        visualElement.Add(button);
                    }
                }
                Add(visualElement);
                if (editFavoriteRoot != null)
                    Add(editFavoriteRoot);
            }
            OnDocTypeChange?.Invoke(docType);
        }

        void createEditFavorite()
        {
            editFavoriteRoot = DocRuntime.NewEmpty();
            foreach(var name in DocEditor.NameList)
            {
                if (name == "None") continue;
                Toggle toggle = new Toggle();
                toggle.style.marginLeft = 10;
                toggle.text = name;
                toggle.value = false;
                foreach(var id in DocEditorData.Instance.FavoriteDocVisualID)
                {
                    string tName;
                    if (DocEditor.ID2Name.TryGetValue(id, out tName))
                    {
                        if(tName==name)
                            toggle.value = true;
                    }
                }
                toggle.RegisterValueChangedCallback(e =>
                {
                    string t;
                    if (e.newValue)
                    {
                        if (DocEditor.Name2ID.TryGetValue(name, out t))
                            DocEditorData.Instance.FavoriteDocVisualID.Add(t);
                    }
                    else
                    {
                        if(DocEditor.Name2ID.TryGetValue(name,out t))
                            DocEditorData.Instance.FavoriteDocVisualID.Remove(t);
                    }
                    EditorUtility.SetDirty(DocEditorData.Instance);
                    Clear();
                    Repaint();
                });
                editFavoriteRoot.Add(toggle);
            }
        }
    }
}
