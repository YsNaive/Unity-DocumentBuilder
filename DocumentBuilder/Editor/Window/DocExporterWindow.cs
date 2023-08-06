using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DocExporterWindow : EditorWindow
{
    [MenuItem("Tools/NaiveAPI/DocumentBuilder/Exporter")]
    public static void ShowWindow()
    {
        GetWindow<DocExporterWindow>("Document Exporter");
    }

    public const string CommonChar = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
    public const string SpChar = "\\n\\t\\r\\0\\b\\f\\v\\\\\\'\\";
    public static readonly List<string> Mode = new List<string>{ "Char Table", "Markdown" };
    VisualElement Layout;
    ObjectField TargetFolder;
    DropdownField ModeSelect;
    string exportPath;
    private void CreateGUI()
    {
        ModeSelect = DocRuntime.NewDropdownField("Export Mode", Mode);
        ModeSelect.RegisterValueChangedCallback(e =>
        {
            Layout.Clear();
            if (ModeSelect.index == 0)
                repaintCharTable();
            else if (ModeSelect.index == 1)
                repaintMarkdown();
        }); ModeSelect.index = 0;
        rootVisualElement.style.backgroundColor = SODocStyle.Current.BackgroundColor;
        Layout = DocRuntime.NewEmpty();
        TargetFolder = DocEditor.NewObjectField<DefaultAsset>("Export Folder");
        rootVisualElement.Add(ModeSelect);
        rootVisualElement.Add(TargetFolder);
        rootVisualElement.Add(Layout);
        repaintCharTable();
        rootVisualElement.style.SetIS_Style(ISPadding.Pixel(10));
    }

    private ObjectField createSODocPageSelectField()
    {
        var field = DocEditor.NewObjectField<SODocPage>("");

        field.RegisterValueChangedCallback(e =>
        {
            if(e.newValue != null && e.previousValue  == null)
            {
                field.parent.Add(createSODocPageSelectField());
            }
            else if(e.newValue == null && e.previousValue != null)
            {
                if(field.parent.IndexOf(field) != field.parent.childCount-1)
                {
                    field.parent.Remove(field);
                }
            }
        });

        return field;
    }

    private void repaintCharTable()
    {
        var container = DocRuntime.NewEmpty();
        container.Add(createSODocPageSelectField());
        Layout.Add(container);
        var btn = DocRuntime.NewButton("Export", () =>
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
        var container = DocRuntime.NewEmpty();
        container.Add(createSODocPageSelectField());
        Layout.Add(container);
        var btn = DocRuntime.NewButton("Export", () =>
        {
            exportCount = 0;
            foreach (ObjectField obj in container.Children())
            {
                if (obj.value == null) continue;
                exportMarkdown((SODocPage)obj.value, exportPath, includeSubPages.value);
            }
            AssetDatabase.Refresh();
            Debug.Log($"DocumentBuilder: Export {exportCount} files.");
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
    int exportCount = 0;
    private void exportMarkdown(SODocPage page, string path, bool includeSub)
    {
        if (page == null) return;
        exportCount++;
        StringBuilder sb = new StringBuilder();
        foreach (DocComponent doc in page.Components) {
            var field = DocEditor.CreateComponentField(doc);
            field.SetStatus(true);
            sb.Append(field.DocEditVisual.ToMarkdown(path));
            sb.AppendLine();
            sb.AppendLine();
        }
        File.WriteAllText($"{Application.dataPath}{path.Replace("Assets","")}/{page.name}.md", sb.ToString());
        if(includeSub)
        {
            if(page.SubPages.Count > 0)
            {
                string subPath = $"{page.name}SubPages";
                if (!AssetDatabase.IsValidFolder(path + "/" + subPath))
                    AssetDatabase.CreateFolder(path, subPath);
                subPath = path + "/" + subPath;
                foreach(var subPage in page.SubPages) 
                    exportMarkdown(subPage, subPath, includeSub);
            }
        }
    }
}
