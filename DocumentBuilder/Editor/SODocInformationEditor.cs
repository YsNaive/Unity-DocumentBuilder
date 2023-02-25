using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static DocumentBuilder.EditorGUITool;
namespace DocumentBuilder
{
    [CustomEditor(typeof(SODocInformation))]
    public class SODocInformationEditor : Editor
    {
        #region var and onEnable
        SODocInformation m_target;
        private bool isSaveTemplate = false;
        private bool isLoadTemplate = false;
        private bool isEditTemplate = false;
        private bool doubleCheck = false;
        private bool hotkeyState = false;
        private int isAutoCreateSubpage = -1;
        private string autoCreateSubpageName = "";
        private string templateName = "";
        private string[] allTemplate;
        private DocComponent backup = null;
        private void OnEnable()
        {
            isSaveTemplate = false;
            isLoadTemplate = false;
            isEditTemplate = false;
            doubleCheck = false;
        }
        SODocInformation.DocumentType lastType;
        private int selectingComponent = -1;
        private int lastSelectingComponent = -1;
        Rect selectingRect = new Rect();
        #endregion

        public override void OnInspectorGUI()
        {

            if (m_target == null)
                m_target = target as SODocInformation;
            #region Hotkey
            if (Event.current.control && !hotkeyState)
            {
                EditorGUIUtility.editingTextField = false;
                if (Event.current.keyCode == KeyCode.S)
                {
                    selectingComponent = -1;
                    hotkeyState = true;
                }
                if (Event.current.keyCode == KeyCode.Q)
                    if (lastSelectingComponent != -1)
                    {
                        var temp = new DocComponent(backup);
                        backup = new DocComponent(m_target.Components[lastSelectingComponent]);
                        m_target.Components[lastSelectingComponent] = temp;
                        hotkeyState = true;
                    }
                if(Event.current.keyCode == KeyCode.UpArrow)
                    if(selectingComponent > 0)
                    {
                        var temp = m_target.Components[selectingComponent];
                        m_target.Components[selectingComponent] = m_target.Components[selectingComponent - 1];
                        m_target.Components[selectingComponent - 1] = temp;
                        selectingComponent--;
                        hotkeyState = true;
                    }
                if(Event.current.keyCode == KeyCode.DownArrow)
                    if (selectingComponent < m_target.Components.Count-1)
                    {
                        var temp = m_target.Components[selectingComponent];
                        m_target.Components[selectingComponent] = m_target.Components[selectingComponent + 1];
                        m_target.Components[selectingComponent + 1] = temp;
                        selectingComponent++;
                        hotkeyState = true;
                    }
            }

            if (Event.current.type == EventType.KeyUp)
                hotkeyState = false;
            m_target.Name = EditorGUILayout.TextField("Name", m_target.Name);
            m_target.DocType = (SODocInformation.DocumentType)EditorGUILayout.EnumPopup("Document Icon Type", m_target.DocType);

            #endregion

            #region load menu icon
            if (lastType != m_target.DocType)
            {
                switch (m_target.DocType)
                {
                    case SODocInformation.DocumentType.Method:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/Method.png");
                        break;
                    case SODocInformation.DocumentType.Class:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/Class.png");
                        break;
                    case SODocInformation.DocumentType.Folder:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/Folder.png");
                        break;
                    case SODocInformation.DocumentType.QuestionMark:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/QuestionMark.png");
                        break;
                    case SODocInformation.DocumentType.Text:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/Text.png");
                        break;
                    case SODocInformation.DocumentType.Tips:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/Tips.png");
                        break;
                    case SODocInformation.DocumentType.Book:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/Book.png");
                        break;
                    case SODocInformation.DocumentType.System:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/System.png");
                        break;
                    case SODocInformation.DocumentType.ItemElement:
                        m_target.MenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/ItemElement.png");
                        break;
                    case SODocInformation.DocumentType.Custom:
                        m_target.MenuIcon = null;
                        break;
                }
                lastType = m_target.DocType;
            }
            if (m_target.DocType == SODocInformation.DocumentType.Custom)
                m_target.MenuIcon = (Texture2D)EditorGUILayout.ObjectField("Menu Icon", m_target.MenuIcon, typeof(Texture2D), false);
            #endregion

            #region SubPage

            EditorGUILayout.LabelField("Sub Pages");
            for (int i = 0; i < m_target.SubPages.Count; i++)
            {
                HorizontalGroup(() =>
                {
                    if (i == 0)
                    {
                        EditorGUILayout.LabelField("", GUILayout.Width(20));
                    }
                    else
                    {
                        if (GUILayout.Button("▲", GUILayout.Width(20)))
                        {
                            SODocInformation t = m_target.SubPages[i - 1];
                            m_target.SubPages[i - 1] = m_target.SubPages[i];
                            m_target.SubPages[i] = t;
                        }
                    }
                    if (i < m_target.SubPages.Count - 1)
                    {
                        if (GUILayout.Button("▼", GUILayout.Width(20)))
                        {
                            SODocInformation t = m_target.SubPages[i + 1];
                            m_target.SubPages[i + 1] = m_target.SubPages[i];
                            m_target.SubPages[i] = t;
                        }
                    }
                    else
                        EditorGUILayout.LabelField("", GUILayout.Width(20));

                    m_target.SubPages[i] = (SODocInformation)EditorGUILayout.ObjectField(m_target.SubPages[i], typeof(SODocInformation), false);

                    if (isAutoCreateSubpage == -1)
                    {
                        if (m_target.SubPages[i] == null)
                        {
                            if (GUILayout.Button("Auto Create"))
                            {
                                isAutoCreateSubpage = i;
                                autoCreateSubpageName = "NewPage";
                            }
                        }
                    }
                    else if (i == isAutoCreateSubpage)
                    {
                        string path = AssetDatabase.GetAssetPath(target);
                        path = path.Substring(0, path.Length - target.name.Length - 7);
                        autoCreateSubpageName = EditorGUILayout.TextField(autoCreateSubpageName);
                        bool nameCheck = false;

                        if (autoCreateSubpageName == "" || autoCreateSubpageName == "NewPage")
                            nameCheck = true;
                        foreach (var obj in AssetDatabase.LoadAllAssetsAtPath($"{path}\\{target.name}SubPage"))
                        {
                            Debug.Log(obj.name);
                            if (obj.name == autoCreateSubpageName)
                            {
                                nameCheck = true;
                                break;
                            }
                        }
                        DisableGroup(nameCheck, () =>
                         {
                             if (GUILayout.Button("Create"))
                             {
                                 if (!AssetDatabase.IsValidFolder($"{path}\\{target.name}SubPage"))
                                     AssetDatabase.CreateFolder(path, $"{target.name}SubPage");

                                 SODocInformation newDoc = CreateInstance<SODocInformation>();
                                 newDoc.name = autoCreateSubpageName;
                                 AssetDatabase.CreateAsset(newDoc, $"{path}\\{target.name}SubPage\\{autoCreateSubpageName}.asset");
                                 m_target.SubPages[i] = newDoc;
                                 isAutoCreateSubpage = -1;
                             }
                         });
                        if (GUILayout.Button("Cancel"))
                        {
                            isAutoCreateSubpage = -1;
                        }
                    }
                    GUILayout.Space(10);
                    Rect btnRect = GUILayoutUtility.GetRect(20, 20,GUILayout.Width(20));
                    btnRect.width = 20;
                    if (TextureButton(btnRect, Icon.Delete))
                    {
                        m_target.SubPages.RemoveAt(i);
                        i--;
                    }
                });
            }
            ColorRegion(true, () =>
            {
                if (GUILayout.Button("AddSubpage", GUILayout.Width(100)))
                {
                    m_target.SubPages.Add(null);
                }
            });

            #endregion

            #region template
            EditorGUILayout.Space(10);
            if (isSaveTemplate)
            {
                HorizontalGroup(() =>
                {
                    if (doubleCheck)
                    {
                        EditorGUILayout.HelpBox("There Exist a Template with same name !\nDo you want to Override ?", MessageType.Warning);
                        HorizontalGroup(() =>
                        {
                            ColorRegion(ColorSet.Danger, () =>
                             {
                                 if (GUILayout.Button("Yes"))
                                 {
                                     DocumentBuilderData.SerializableList<DocComponent> serializableList = new DocumentBuilderData.SerializableList<DocComponent>();
                                     serializableList.list = m_target.Components;
                                     DocumentBuilderData.SaveString(DocumentBuilderData.Path.DocTemplate, (templateName + ".txt"), JsonUtility.ToJson(serializableList), true);
                                     doubleCheck = false;
                                     isSaveTemplate = false;
                                 }
                             });
                            ColorRegion(ColorSet.Success, () =>
                             {
                                 if (GUILayout.Button("No"))
                                 {
                                     doubleCheck = false;
                                 }
                             });
                        });
                    }
                    else
                    {
                        GUILayout.Label("Name", GUILayout.Width(100));
                        templateName = EditorGUILayout.TextField(templateName);
                        if (GUILayout.Button("Save", GUILayout.Width(80)))
                        {
                            allTemplate = Directory.GetFiles(DocumentBuilderData.Path.ProjectRoot + '\\' + DocumentBuilderData.Path.DocTemplate);
                            foreach (string file in allTemplate)
                            {
                                if ((templateName + ".txt") == file.Substring(file.LastIndexOf("\\") + 1))
                                {
                                    doubleCheck = true;
                                    break;
                                }
                            }

                            if (!doubleCheck)
                            {
                                DocumentBuilderData.SerializableList<DocComponent> serializableList = new DocumentBuilderData.SerializableList<DocComponent>();
                                serializableList.list = m_target.Components;
                                DocumentBuilderData.SaveString(DocumentBuilderData.Path.DocTemplate, (templateName + ".txt"), JsonUtility.ToJson(serializableList), true);
                                isSaveTemplate = false;
                            }
                        }
                        if (GUILayout.Button("Cancel", GUILayout.Width(80)))
                        {
                            isSaveTemplate = false;
                        }
                    }
                });


            }
            else if (isLoadTemplate)
            {
                HorizontalGroup(() =>
                {
                    templateName = EditorGUILayout.Popup(int.Parse(templateName), allTemplate).ToString();
                    if (GUILayout.Button("Load", GUILayout.Width(80)))
                    {
                        DocumentBuilderData.SerializableList<DocComponent> serializableList;
                        serializableList = JsonUtility.FromJson<DocumentBuilderData.SerializableList<DocComponent>>(DocumentBuilderData.LoadString(DocumentBuilderData.Path.DocTemplate, allTemplate[int.Parse(templateName)]));
                        m_target.Components = serializableList.list;
                        isLoadTemplate = false;
                    }
                    if (GUILayout.Button("Cancel", GUILayout.Width(80)))
                    {
                        isLoadTemplate = false;
                    }
                });
            }
            else if (isEditTemplate)
            {
                foreach (string filename in allTemplate)
                {
                    EditorGUITool.HorizontalGroup(() =>
                    {
                        if (GUILayout.Button("Remove", GUILayout.Width(80)))
                        {
                            File.Delete(DocumentBuilderData.Path.ProjectRoot + '\\' + DocumentBuilderData.Path.DocTemplate + '\\' + filename);
                            File.Delete(DocumentBuilderData.Path.ProjectRoot + '\\' + DocumentBuilderData.Path.DocTemplate + '\\' + filename + ".meta");
                            AssetDatabase.Refresh();

                            allTemplate = Directory.GetFiles(DocumentBuilderData.Path.ProjectRoot + '\\' + DocumentBuilderData.Path.DocTemplate);
                            List<string> popupOption = new List<string>();
                            for (int i = 0; i < allTemplate.Length; i++)
                            {
                                if (allTemplate[i].Substring(allTemplate[i].Length - 5) != ".meta")
                                    popupOption.Add(allTemplate[i].Substring(allTemplate[i].LastIndexOf("\\") + 1));
                            }
                            allTemplate = popupOption.ToArray();
                        }
                        GUILayout.Label(filename);
                    });
                }
                GUILayout.Space(10);
                if (GUILayout.Button("end editing"))
                {
                    isEditTemplate = false;
                }
            }
            else
            {
                HorizontalGroup(() =>
                {
                    EditorGUILayout.LabelField("DocComponent Template",GUILayout.Width(150));
                    if (GUILayout.Button("Save as new"))
                    {
                        isSaveTemplate = true;
                        templateName = "";
                        doubleCheck = false;
                    }
                    if (GUILayout.Button("Load from old"))
                    {
                        allTemplate = Directory.GetFiles(DocumentBuilderData.Path.ProjectRoot + '\\' + DocumentBuilderData.Path.DocTemplate);
                        List<string> popupOption = new List<string>();
                        for (int i = 0; i < allTemplate.Length; i++)
                        {
                            if (allTemplate[i].Substring(allTemplate[i].Length - 5) != ".meta")
                                popupOption.Add(allTemplate[i].Substring(allTemplate[i].LastIndexOf("\\") + 1));
                        }
                        allTemplate = popupOption.ToArray();
                        isLoadTemplate = true;
                        templateName = "0";
                    }
                    if (GUILayout.Button("edit", GUILayout.Width(50)))
                    {
                        allTemplate = Directory.GetFiles(DocumentBuilderData.Path.ProjectRoot + '\\' + DocumentBuilderData.Path.DocTemplate);
                        List<string> popupOption = new List<string>();
                        for (int i = 0; i < allTemplate.Length; i++)
                        {
                            if (allTemplate[i].Substring(allTemplate[i].Length - 5) != ".meta")
                                popupOption.Add(allTemplate[i].Substring(allTemplate[i].LastIndexOf("\\") + 1));
                        }
                        allTemplate = popupOption.ToArray();
                        isEditTemplate = true;
                    }
                });
            }
            #endregion

            #region component edit
            GUILayout.Space(15);
            if (m_target.Components.Count != 0)
            {
                for (int i = 0; i < m_target.Components.Count; i++)
                {
                    Rect rect = new Rect();
                    rect = GUILayoutUtility.GetLastRect();
                    DisableGroup(selectingComponent != i, () =>
                    {
                        // Allow Edit
                        if (selectingComponent == i)
                            HorizontalGroup(() =>
                            {
                                if (i != 0)
                                {
                                    if (GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        var temp = m_target.Components[i];
                                        m_target.Components[i] = m_target.Components[i - 1];
                                        m_target.Components[i - 1] = temp;
                                        selectingComponent--;
                                    }
                                }
                                else
                                    GUILayoutUtility.GetRect(0,0, GUILayout.Width(20), GUILayout.Height(20));
                                if (i != m_target.Components.Count - 1)
                                {
                                    if (GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        var temp = m_target.Components[i];
                                        m_target.Components[i] = m_target.Components[i + 1];
                                        m_target.Components[i + 1] = temp;
                                        selectingComponent++;
                                    }
                                }
                                else
                                    GUILayoutUtility.GetRect(0, 0, GUILayout.Width(20), GUILayout.Height(20));
                                Rect btnRect = GUILayoutUtility.GetRect(0,0,GUILayout.Width(25),GUILayout.Height(25));
                                btnRect.y -= 3;
                                btnRect.x -= 5;

                                btnRect.x = btnRect.xMax;
                                if (TextureButton(btnRect,Icon.Copy))
                                {
                                    GUIUtility.systemCopyBuffer = m_target.Components[i].ToString();
                                }

                                btnRect.x = btnRect.xMax + 5;
                                if (TextureButton(btnRect, Icon.Paste))
                                {
                                    if (!m_target.Components[i].FromString(GUIUtility.systemCopyBuffer))
                                        Debug.LogWarning("You are not Pasting a DocComponent !");
                                }

                                btnRect.x = btnRect.xMax + 5;
                                if (TextureButton(btnRect, Icon.Duplicate))
                                {
                                    DocComponent newComponent = new DocComponent();
                                    newComponent.FromString(m_target.Components[i].ToString());
                                    m_target.Components.Insert(i, newComponent);
                                    selectingComponent++;
                                }


                                GUILayout.Label("");
                                btnRect = GUILayoutUtility.GetRect(0, 0, GUILayout.Width(25), GUILayout.Height(25));
                                btnRect.y -= 3;
                                btnRect.x -= 5;
                                if (TextureButton(btnRect, Icon.Delete))
                                {
                                    m_target.Components.RemoveAt(i);
                                    i--;
                                    selectingComponent = -1;
                                    lastSelectingComponent = -1;
                                }
                            });
                        if (i < m_target.Components.Count && i >= 0)
                            m_target.Components[i] = DocComponentLayout.DocComponentField(m_target.Components[i], selectingComponent == i);

                        if(selectingComponent == i)
                            EditorGUILayout.Space(15);
                        
                    });
                    rect.y = rect.yMax;
                    rect.width = EditorGUIUtility.currentViewWidth-40;
                    rect.height = GUILayoutUtility.GetRect(0,15).y - rect.y;
                    if (selectingComponent != i)
                    {
                        // check if click component
                        if (GUI.Button(rect, "", new GUIStyle()))
                        {
                            selectingComponent = i;
                            selectingRect = rect;
                            backup = new DocComponent(m_target.Components[selectingComponent]);
                            EditorGUIUtility.editingTextField = false;
                        }
                        EditorGUI.DrawRect(new Rect(rect.x - 12, rect.y - 3, 6, rect.height + 6), new Color(.8f,.8f,1f,0.25f));
                    }
                    else
                    {
                        EditorGUI.DrawRect(new Rect(rect.x - 12, rect.y - 3, 6, rect.height + 6), new Color(.8f, 1f, .8f, 0.25f));
                        EditorGUILayout.Space(5);
                        Rect btnRect = GUILayoutUtility.GetLastRect();
                        btnRect.x -= 8;
                        btnRect.y -= 13;
                        btnRect.height = 16;
                        btnRect.width = 60;
                        if(GUI.Button(btnRect,"Insert ->"))
                        {
                            DocComponent newComponent = new DocComponent();
                            m_target.Components.Insert(i+1, newComponent);
                            selectingComponent++;
                            EditorGUIUtility.editingTextField = false;
                        }
                    }
                }
            }
            HorizontalGroup(() =>
            {
                if (GUILayout.Button("Add new component", GUILayout.Width(200)))
                {
                    m_target.Components.Add(new DocComponent());
                }
                if (GUILayout.Button("Paste component", GUILayout.Width(200)))
                {
                    DocComponent doc = new DocComponent();
                    doc.FromString(GUIUtility.systemCopyBuffer);
                    m_target.Components.Add(doc);
                }
            });
            #endregion


            if (selectingComponent != -1)
                lastSelectingComponent = selectingComponent;

            EditorUtility.SetDirty(target);
        }
    }
}