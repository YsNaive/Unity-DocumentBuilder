using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocPageDeleter : VisualElement
    {
        public DocPageDeleter(SODocPage page, Action<(bool isDelete, string[] deletedPage, string[] deletedFolder)> callback = null)
        {
            var sysPath = AssetDatabase.GetAssetPath(page);
            sysPath = Application.dataPath + sysPath.Substring(6, sysPath.LastIndexOf('/') - 6);
            style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize));
            Add(new DSLabel($"Delete Pages {page.name}"));
            Add(new DSLabel("### THIS CAN NOT BE UNDO ###"));
            var singleDelete = DocRuntime.NewButton("Delete this page");
            singleDelete.style.marginTop = DocStyle.Current.LabelTextSize;
            singleDelete.clicked += () =>
            {
                var result = DocPageEditorUtils.DeletePageAsset(page, DocPageEditorUtils.DeleteMode.Self);
                callback?.Invoke((true, result.deletedPage, result.deletedFolder));
            };

            var treeDelete = DocRuntime.NewButton("Delete this page <b>AND ITS ALL CHILDERN</b>");
            treeDelete.style.marginTop = DocStyle.Current.LabelTextSize;
            treeDelete.clicked += () =>
            {
                var result = DocPageEditorUtils.DeletePageAsset(page, DocPageEditorUtils.DeleteMode.Tree);
                callback?.Invoke((true, result.deletedPage, result.deletedFolder));
            };

            var cancel = DocRuntime.NewButton("Cancel");
            cancel.style.marginTop = DocStyle.Current.LabelTextSize;
            cancel.clicked += () =>
            {
                callback?.Invoke((false, new string[0], new string[0]));
            };

            Add(singleDelete);
            Add(treeDelete);
            Add(cancel);
        }
    }
}
