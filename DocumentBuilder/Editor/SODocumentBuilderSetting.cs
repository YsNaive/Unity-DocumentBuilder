using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using static DocumentBuilder.EditorGUITool;
using System.IO;

namespace DocumentBuilder
{
    public class SODocumentBuilderSetting : ScriptableObject
    {
        public List<SODocInformation> DocBookList;
        public DefaultAsset MarkdownExportFolder;
        public SODocInformation ExportRoot;
        public static SODocumentBuilderSetting Get
        {
            get
            {
                var settings = AssetDatabase.LoadAssetAtPath<SODocumentBuilderSetting>(DocumentBuilderData.Path.ProjectSetting + "\\DocumentBuilderSetting.asset");
                if (settings == null)
                {
                    settings = CreateInstance<SODocumentBuilderSetting>();
                    AssetDatabase.CreateAsset(settings, DocumentBuilderData.Path.ProjectSetting + "\\DocumentBuilderSetting.asset");
                    AssetDatabase.SaveAssets();
                }
                return settings;
            }
        }
    }

    static class SONaiveApiSettingProvider
    {
        [SettingsProvider]
        public static SettingsProvider NaiveApiSettingProvider()
        {
            SODocumentBuilderSetting data = null;
            Color defaultColor = GUI.color;
            return new SettingsProvider("Project/DocumentBuilder", SettingsScope.Project)
            {
                label = "DocumentBuilder Settings",
                activateHandler = (context, element) => {
                    data = SODocumentBuilderSetting.Get;
                },
                guiHandler = (context) =>
                {

                    DividerLine("Document Book List");
                    GUILayout.Space(5);
                    for (int i = 0; i < data.DocBookList.Count; i++) 
                    {
                        HorizontalGroup(() =>
                        {
                            EditorGUILayout.LabelField($"Book{i + 1}", GUILayout.Width(40));
                            data.DocBookList[i] = (SODocInformation)EditorGUILayout.ObjectField(data.DocBookList[i], typeof(SODocInformation), false);
                            if (GUILayout.Button("Remove", GUILayout.Width(60)))
                            {
                                data.DocBookList.RemoveAt(i);
                                i--;
                            }
                        });
                    }

                    if (GUILayout.Button("Add",GUILayout.Width(100)))
                    {
                        data.DocBookList.Add(null);
                    }

                    GUILayout.Space(50);
                    DividerLine("Export");
                    GUILayout.Space(5);
                    HorizontalGroup(() =>
                    {
                        EditorGUIUtility.labelWidth = 30;
                        data.MarkdownExportFolder = (DefaultAsset)EditorGUILayout.ObjectField("Path ", data.MarkdownExportFolder, typeof(DefaultAsset), false);
                        GUILayout.Space(15);
                        data.ExportRoot = (SODocInformation)EditorGUILayout.ObjectField("root ", data.ExportRoot, typeof(SODocInformation), false);
                        GUILayout.Space(15);
                        if (GUILayout.Button("Export as Markdown",GUILayout.Width(150)))
                        {
                            DocumentExporter.ToMarkDown(data.ExportRoot, DocumentBuilderData.Path.ProjectRoot + "/" + AssetDatabase.GetAssetPath(data.MarkdownExportFolder));
                            AssetDatabase.Refresh();
                        }
                    });

                    EditorUtility.SetDirty(data);
                }
            };
        }
    }

}

