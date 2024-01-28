using NaiveAPI.DocumentBuilder;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public static class DocPageEditorUtils
    {
        public enum DeleteMode
        {
            Self,
            Tree,
        }
        public static string GetAssetFolderPath(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            path = path.Substring(0, path.LastIndexOf('/'));
            return path;
        }
        public static string GetAssetSystemFolderPath(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            path = path.Substring(6, path.LastIndexOf('/') - 6);
            return Application.dataPath + path;
        }
        public static string ValidSubPageFolderPath(SODocPage page)
        {
            var folderPath = GetAssetFolderPath(page);
            var subPageFolderName = $"{page.name}SubPages";
            var subPageFolderPath = $"{folderPath}/{subPageFolderName}";
            if (!AssetDatabase.IsValidFolder(subPageFolderPath))
                AssetDatabase.CreateFolder(folderPath, subPageFolderName);
            return subPageFolderPath;
        }
        public static SODocPage CreatePageAsset(SODocPage parentPage, string name)
        {
            var newPage = CreatePageAsset($"{ValidSubPageFolderPath(parentPage)}/{name}.asset");
            parentPage.SubPages.Add(newPage);
            return newPage;
        }
        public static SODocPage CreatePageAsset(string path)
        {
            var newPage = ScriptableObject.CreateInstance<SODocPage>();
            AssetDatabase.CreateAsset(newPage, path);
            AssetDatabase.Refresh();
            return newPage;
        }
        public static (string[] deletedPage, string[] deletedFolder) DeletePageAsset(SODocPage toDelete, DeleteMode mode = DeleteMode.Self)
        {
            List<string> deletedPage = new();
            List<SODocPage> toDeleteList;
            if (mode == DeleteMode.Self)
                toDeleteList = new() { toDelete };
            else
                toDeleteList = new(toDelete.Pages());
            var effectSystemFolder = GetAssetSystemFolderPath(toDelete);
            foreach (var page in toDeleteList)
            {
                var path = AssetDatabase.GetAssetPath(page);
                deletedPage.Add(path);
                AssetDatabase.DeleteAsset(path);
            }
            var deleteFolderResult = DeleteEmptySubPagesFolder(Directory.CreateDirectory(effectSystemFolder));
            AssetDatabase.Refresh();

            return (deletedPage.ToArray(), deleteFolderResult) ; ;
        }
        /// <summary>
        /// Remove any empty folders contains "SubPages" in name
        /// </summary>
        public static string[] DeleteEmptySubPagesFolder() { return DeleteEmptySubPagesFolder(Directory.CreateDirectory(Application.dataPath)); }
        public static string[] DeleteEmptySubPagesFolder(DirectoryInfo beginDirectory)
        {
            var result = deleteEmptySubPagesFolderRec(beginDirectory, new());
            AssetDatabase.Refresh();
            return result.ToArray();
        }
        private static List<string> deleteEmptySubPagesFolderRec(DirectoryInfo directory, List<string> logBuffer)
        {
            foreach (var subDir in directory.GetDirectories())
                deleteEmptySubPagesFolderRec(subDir, logBuffer);

            if (!directory.Name.Contains("SubPages"))
                return logBuffer;

            if (directory.GetFiles().Length == 0)
            {
                Directory.Delete(directory.FullName);
                File.Delete($"{directory.FullName}.meta");
                logBuffer.Add($"{directory.FullName}");
            }
            return logBuffer;
        }

        public static bool MovePageAsset(SODocPage page, string toFolder)
        {
            if (!AssetDatabase.IsValidFolder(toFolder))
            {
                Debug.LogError($"DocPageEditorUtils: Fail to move page \"{page.name}\" to \"{toFolder}\", the folder is not valid");
                return false;
            }
            string orgPath = AssetDatabase.GetAssetPath(page);
            string effectFolder = GetAssetSystemFolderPath(page);
            AssetDatabase.MoveAsset(orgPath, $"{toFolder}/{page.name}.asset");
            DeleteEmptySubPagesFolder(Directory.CreateDirectory(effectFolder));

            AssetDatabase.Refresh();
            return true;
        }
    }
}