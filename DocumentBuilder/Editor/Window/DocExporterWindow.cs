using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocExporterWindow : EditorWindow
    {

        public const string CommonChar = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        public const string SpChar = "\\n\\t\\r\\0\\b\\f\\v\\\\\\'\\";
        public static readonly List<string> Mode = new List<string> { "Char Table", "Markdown" };
        VisualElement Layout;
        ObjectField TargetFolder;
        DSDropdown ModeSelect;
        string exportPath;
        private void CreateGUI()
        {
            ModeSelect = new DSDropdown("Export Mode") { choices = Mode };
            ModeSelect.RegisterValueChangedCallback(e =>
            {
                Layout.Clear();
                if (ModeSelect.index == 0)
                    repaintCharTable();
                else if (ModeSelect.index == 1)
                    repaintMarkdown();
            }); ModeSelect.index = 0;
            rootVisualElement.style.backgroundColor = DocStyle.Current.BackgroundColor;
            Layout = new VisualElement();
            TargetFolder = DocEditor.NewObjectField<DefaultAsset>("Export Folder");
            rootVisualElement.Add(ModeSelect);
            rootVisualElement.Add(Layout);
            repaintCharTable();
            rootVisualElement.style.SetIS_Style(ISPadding.Pixel(10));
        }

        private ObjectField createSODocPageSelectField()
        {
            var field = DocEditor.NewObjectField<SODocPage>("");

            field.RegisterValueChangedCallback(e =>
            {
                if (e.newValue != null && e.previousValue == null)
                {
                    field.parent.Add(createSODocPageSelectField());
                }
                else if (e.newValue == null && e.previousValue != null)
                {
                    if (field.parent.IndexOf(field) != field.parent.childCount - 1)
                    {
                        field.parent.Remove(field);
                    }
                }
            });

            return field;
        }

        private void repaintCharTable()
        {
            var container = new VisualElement();
            container.Add(createSODocPageSelectField());
            Layout.Add(TargetFolder);
            Layout.Add(container);
            var btn = new DSButton("Export", () =>
            {
                Queue<SODocPage> queue = new Queue<SODocPage>();
                List<SODocPage> pagelist = new List<SODocPage>();
                foreach (ObjectField obj in container.Children())
                    queue.Enqueue((SODocPage)obj.value);
                while (queue.Count > 0)
                {
                    var now = queue.Dequeue();
                    if (now == null) continue;
                    foreach (var sub in now.SubPages)
                    {
                        if (sub == null) continue;
                        if (!pagelist.Contains(sub))
                        {
                            pagelist.Add(sub);
                            queue.Enqueue(sub);
                        }
                    }
                }
                HashSet<char> foundChars = new HashSet<char>();
                Debug.Log(pagelist.Count);
                foreach (var page in pagelist)
                {
                    foreach (var com in page.Components)
                    {
                        foreach (var str in com.TextData)
                        {
                            foreach (var c in str)
                            {
                                if (SpChar.Contains(c))
                                    continue;
                                if (CommonChar.Contains(c))
                                    continue;
                                if (!foundChars.Contains(c))
                                    foundChars.Add(c);
                            }
                        }
                    }
                }
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(CommonChar);
                foreach (var c in foundChars)
                { stringBuilder.Append(c); }
                TextAsset textAsset = new TextAsset(stringBuilder.ToString());
                AssetDatabase.CreateAsset(textAsset, exportPath + "/char table.asset");
            });
            btn.style.marginTop = 20;
            btn.SetEnabled(false);
            TargetFolder.RegisterValueChangedCallback(e =>
            {
                if (e.newValue != null)
                {
                    exportPath = AssetDatabase.GetAssetPath(e.newValue);
                }
                btn.SetEnabled(AssetDatabase.IsValidFolder(exportPath));
            });
            Layout.Add(btn);
        }
        private void repaintMarkdown()
        {
            Toggle includeSubPages = new Toggle();
            includeSubPages.label = "Include SubPages";
            includeSubPages.value = true;
            Layout.Add(includeSubPages);
            var container = new VisualElement();
            container.Add(createSODocPageSelectField());
            VisualElement info = new VisualElement();
            var btn = new DSButton("Export", () =>
            {
                EditorUtility.DisplayProgressBar("Document Builder", "Exporting Markdown...", 1);
                exportFailCount = 0;
                exportCount = 0;
                failList.Clear();
                info.Clear();
                foreach (ObjectField obj in container.Children())
                {
                    if (obj.value == null) continue;
                    exportMarkdown((SODocPage)obj.value, exportPath, includeSubPages.value);
                }
                AssetDatabase.Refresh();
                DocComponent doc = new DocComponent();
                doc.VisualID = new DocDescription().VisualID;
                doc.TextData.Add($"Success Export {exportCount} files.");
                doc.JsonData = JsonUtility.ToJson(new ValueTuple<DocDescription.DescriptionType>(DocDescription.DescriptionType.Success));
                info.Add(DocRuntime.CreateDocVisual(doc));
                if (exportFailCount > 0)
                {
                    doc.TextData[0] = $"Fail Export {exportFailCount}.";
                    doc.JsonData = JsonUtility.ToJson(new ValueTuple<DocDescription.DescriptionType>(DocDescription.DescriptionType.Danger));
                    info.Add(DocRuntime.CreateDocVisual(doc));
                    foreach (var page in failList)
                    {
                        var field = DocEditor.NewObjectField<SODocPage>("");
                        field.value = page;
                        field.style.backgroundColor = DocStyle.Current.DangerColor;
                        info.Add(field);
                    }
                }
            });
            btn.style.marginTop = 20;
            btn.SetEnabled(false);
            var pathField = new DSTextField("Dst Path", e =>
            {
                if (Directory.Exists(e.newValue))
                {
                    btn.SetEnabled(true);
                    exportPath = e.newValue;
                }
                else
                {
                    btn.SetEnabled(false);
                }
            });

            Layout.Add(pathField);
            Layout.Add(container);
            Layout.Add(info);
            Layout.Add(btn);
        }
        int exportCount = 0;
        int exportFailCount = 0;
        List<SODocPage> failList = new List<SODocPage>();
        private void exportMarkdown(SODocPage page, string path, bool includeSub)
        {
            if (page == null) return;
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (DocComponent doc in page.Components)
                {
                    var field = new DocComponentField(new DocComponentProperty(doc));
                    field.SetStatus(true);
                    sb.Append(field.DocEditVisual.ToMarkdown(path));
                    sb.AppendLine();
                    sb.AppendLine();
                }
                exportCount++;
            }
            catch
            {
                exportFailCount++;
                failList.Add(page);
            }
            string texts = sb.ToString();
            if (!string.IsNullOrEmpty(texts))
                File.WriteAllText($"{path}/{page.name}.md", sb.ToString());
            if (includeSub)
            {
                if (page.SubPages.Count > 0)
                {
                    string subPath = $"{page.name}";
                    if (!Directory.Exists(path + "/" + subPath))
                        Directory.CreateDirectory(path + "/" + subPath);
                    subPath = path + "/" + subPath;
                    foreach (var subPage in page.SubPages)
                        exportMarkdown(subPage, subPath, includeSub);
                }
            }
        }
    }

}