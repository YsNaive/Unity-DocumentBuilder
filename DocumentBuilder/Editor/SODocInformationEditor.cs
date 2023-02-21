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
        SODocInformation m_target;
        private bool isSaveTemplate = false;
        private bool isLoadTemplate = false;
        private bool isEditTemplate = false;
        private bool doubleCheck = false;
        private int isAutoCreateSubpage = -1;
        private string autoCreateSubpageName = "";
        private string templateName = "";
        private string[] allTemplate;
        private void OnEnable()
        {
            isSaveTemplate = false;
            isLoadTemplate = false;
            isEditTemplate = false;
            doubleCheck = false;
        }
        Vector2 scrollPosition = Vector2.zero;
        SODocInformation.DocumentType lastType;
        public override void OnInspectorGUI()
        {
            if (m_target == null)
                m_target = target as SODocInformation;

            m_target.Name = EditorGUILayout.TextField("Name", m_target.Name);
            m_target.DocType = (SODocInformation.DocumentType)EditorGUILayout.EnumPopup("Document Icon Type", m_target.DocType);
            EditorGUILayout.LabelField("Sub Pages");

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

                    ColorRegion(false, () =>
                    {
                        if (GUILayout.Button("Remove", GUILayout.Width(60)))
                        {
                            m_target.SubPages.RemoveAt(i);
                            i--;
                        }
                    });
                });
            }
            ColorRegion(true, () =>
            {
                if (GUILayout.Button("AddSubpage", GUILayout.Width(100)))
                {
                    m_target.SubPages.Add(null);
                }
            });

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
                    EditorGUILayout.LabelField("DocComponent Template");
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
            GUILayout.Space(15);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if (m_target.Components.Count != 0)
            {
                for (int i = 0; i < m_target.Components.Count; i++)
                {
                    HorizontalGroup(() =>
                    {
                        if (i != 0)
                        {
                            if (GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                var temp = m_target.Components[i];
                                m_target.Components[i] = m_target.Components[i - 1];
                                m_target.Components[i - 1] = temp;
                            }
                        }
                        else
                        {
                            GUILayout.Label("", GUILayout.Width(20), GUILayout.Height(20));
                        }
                        if (i != m_target.Components.Count - 1)
                        {
                            if (GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                var temp = m_target.Components[i];
                                m_target.Components[i] = m_target.Components[i + 1];
                                m_target.Components[i + 1] = temp;
                            }
                        }
                        DividerLine(m_target.Components[i].ComponentType.ToString(), EditorGUITool.ColorSet.Default, 1);
                        if (GUILayout.Button("Copy", GUILayout.Width(60)))
                        {
                            GUIUtility.systemCopyBuffer = m_target.Components[i].ToString();
                        }
                        if (GUILayout.Button("Paste", GUILayout.Width(60)))
                        {
                            if (!m_target.Components[i].FromString(GUIUtility.systemCopyBuffer))
                                Debug.LogWarning("You are not Pasting a DocComponent !");
                        }
                        if (GUILayout.Button("Dup", GUILayout.Width(60)))
                        {
                            DocComponent newComponent = new DocComponent();
                            newComponent.FromString(m_target.Components[i].ToString());
                            m_target.Components.Insert(i, newComponent);
                        }
                        ColorRegion(false, () =>
                        {
                            if (GUILayout.Button("Remove", GUILayout.Width(60)))
                            {
                                m_target.Components.RemoveAt(i);
                                i--;
                            }
                        });
                    });
                    m_target.Components[i] = m_target.Components[i];
                    if (i < m_target.Components.Count && i >= 0)
                        m_target.Components[i] = DocComponentLayout.DocComponentField(m_target.Components[i], true);

                    EditorGUILayout.Space(25);
                }
            }
            HorizontalGroup(() =>
            {
                if (GUILayout.Button("Add new component", GUILayout.Width(200)))
                {
                    m_target.Components.Add(new DocComponent());
                }
                if (GUILayout.Button("Paset component", GUILayout.Width(200)))
                {
                    DocComponent doc = new DocComponent();
                    doc.FromString(GUIUtility.systemCopyBuffer);
                    m_target.Components.Add(doc);
                }
            });
            EditorGUILayout.EndScrollView();
            EditorUtility.SetDirty(target); // if you need to save serialize object
        }
    }
}