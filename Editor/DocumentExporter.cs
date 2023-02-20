using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DocumentBuilder
{
    public static class DocumentExporter
    {
        private static int index;
        /// <summary>
        /// don't use Assets-Releated path
        /// </summary>
        public static bool ToMarkDown(SODocInformation root, string path)
        {
            Debug.Log(path);
            if (!Directory.Exists(path))
                return false;
            if (root == null)
                return false;
            string rootPath = path + "/" + root.name;
            Directory.CreateDirectory(rootPath);
            StringBuilder summary = new StringBuilder();
            summary.Append("# SUMMARY\n\n");
            ToMarkDownRecursive(root, rootPath, new string(rootPath), summary, 0);
            File.WriteAllText(rootPath + "/SUMMARY.md", summary.ToString());
            return true;    
        }
        /// <summary>
        /// return summary
        /// </summary>
        private static StringBuilder ToMarkDownRecursive(SODocInformation root,string rootPath, string cdPath, StringBuilder summary, int deep)
        {
            index = 0;

            if (deep == 0)
            {
                File.WriteAllText(cdPath + $"/{root.name}.md", DocInformationToMD(root, cdPath));
                for (int i = 0; i < deep; i++) summary.Append("    ");
                summary.Append($"* [{root.name}]({root.name}.md)\n");
            }

            foreach (var com in root.SubPages)
            {
                if (com == null) continue;

                if (com.SubPages == null) continue;
                File.WriteAllText(cdPath + $"/{com.name}.md", DocInformationToMD(com, cdPath));
                for (int i = -1; i < deep; i++) summary.Append("    ");
                summary.Append($"* [{com.name}]({cdPath.Replace(rootPath+ (deep == 0? "":"/"), "")}{(deep == 0? "":"/")}{com.name}.md)\n");

                if (com.SubPages == null) continue;
                if (com.SubPages.Count != 0)
                {
                    Directory.CreateDirectory(cdPath + "/" + com.name);
                    ToMarkDownRecursive(com, rootPath, cdPath + "/" + com.name, summary, deep+1);
                }
            }
            return summary;
        }
        private static string DocInformationToMD(SODocInformation root, string folderPath)
        {
            StringBuilder text = new StringBuilder();
            text.Append("# ").Append(root.name).Append("\n\n");
            foreach (DocComponent info in root.Components)
            {
                switch (info.ComponentType)
                {
                    case DocComponentType.Description:
                        text.Append(info.Text[0]).Append('\n');
                        break;
                    case DocComponentType.NameAndUsage:
                        break;
                    case DocComponentType.FuncDisplay:
                        text.Append("### ")
                            .Append(" <font color=#7293A0>")
                            .Append(info.Text[0]).Append("</font>")
                            .Append(" <font color=#CCC066>")
                            .Append(info.Text[1]).Append("</font>")
                            .Append(" ( ");
                        for (int i = 2; i < info.Text.Count - 1; i++)
                        {
                            string[] func = info.Text[i].Split("\n");
                            if (i != 2)
                            {
                                text.Append(", ");
                            }
                            text.Append(" <font color=#7293A0>")
                                .Append(func[0]).Append("</font>")
                                .Append(" <font color=#8CCCFF>")
                                .Append(func[0]).Append("</font>")
                                .Append(" )");
                        }
                        text.Append("\n")
                            .Append(info.Text[info.Text.Count - 1])
                            .Append("\n");
                        break;
                    case DocComponentType.Label:
                        text.Append("## ").Append(info.Text[0]).Append("\n\n");
                        break;
                    case DocComponentType.Picture:
                        string sourcePath = DocumentBuilderData.Path.ProjectRoot + "/" + AssetDatabase.GetAssetPath(info.Picture);
                        string targetPath = folderPath + "/img_" + index.ToString() + ".png";
                        File.Copy(sourcePath, targetPath, true);
                        text.Append($"![image](img_{index.ToString()}.png)\n");
                        index++;
                        break;
                    case DocComponentType.Matrix:
                        text.Append('\n');
                        for (int i = 0; i < info.Text.Count; i++)
                        {
                            string[] contents = info.Text[i].Split("%column%");
                            for (int j = 0; j < contents.Length; j++)
                            {
                                text.Append('|').Append(contents[j].Replace("\\n"," "));
                            }
                            text.Append("|\n");

                            if (i == 0)
                            {
                                for (int j = 0; j < contents.Length; j++) text.Append("|:-:");
                                text.Append("|\n");
                            }
                        }
                        text.Append('\n');
                        break;
                    case DocComponentType.DividerLine:
                        text.Append("\n--- \n");
                        break;
                }
            }
            return text.ToString();
        }
    }
}
