using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocPageDeleter : VisualElement
    {
        public DocPageDeleter(SODocPage page, Action<(bool isDelete, string[] deletedPage, string[] deletedFolder)> callback = null)
        {
            style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize));
            var sysPath = AssetDatabase.GetAssetPath(page);
            sysPath = Application.dataPath + sysPath.Substring(6, sysPath.LastIndexOf('/') - 6);
            var hor = new DSHorizontal();
            hor.style.marginBottom = DocStyle.Current.LineHeight;
            hor.style.marginBottom = DocStyle.Current.LineHeight * 0.5f;
            var title = new DSTextElement("Delete page : ");
            title.style.width = DocStyle.Current.LabelWidth;
            hor.Add(title);
            hor.Add(new DocPageMenuItem(page, DocPageMenuItem.InitMode.Single));
            Add(hor);

            Add(DocVisual.Create(DocDescription.CreateComponent(
                "<b>### THIS CAN NOT BE UNDO ###</b>",DocDescription.DescriptionType.Danger)));

            var singleDelete = new DSButton("Delete this page", DocStyle.Current.DangerColor);
            singleDelete.style.marginTop = DocStyle.Current.MainTextSize;
            singleDelete.clicked += () =>
            {
                var result = DocPageEditorUtils.DeletePageAsset(page, DocPageEditorUtils.DeleteMode.Self);
                callback?.Invoke((true, result.deletedPage, result.deletedFolder));
            };

            var treeDelete = new DSButton("Delete this page <b>AND ITS ALL CHILDERN</b>", DocStyle.Current.DangerColor);
            treeDelete.style.marginTop = DocStyle.Current.MainTextSize;
            treeDelete.clicked += () =>
            {
                var result = DocPageEditorUtils.DeletePageAsset(page, DocPageEditorUtils.DeleteMode.Tree);
                callback?.Invoke((true, result.deletedPage, result.deletedFolder));
            };

            var cancel = new DSButton("Cancel",DocStyle.Current.SuccessColor);
            cancel.style.marginTop = DocStyle.Current.MainTextSize;
            cancel.style.height = DocStyle.Current.MainTextSize * 3f;
            cancel.style.fontSize = DocStyle.Current.MainTextSize* 1.5f;
            cancel.clicked += () =>
            {
                callback?.Invoke((false, new string[0], new string[0]));
            };

            Add(cancel);
            Add(singleDelete);
            var preview = new DocPageMenuItem(page, DocPageMenuItem.InitMode.Single);
            preview.style.marginLeft = DocStyle.Current.LineHeight;
            preview.SetEnabled(false);
            Add(preview);
            Add(treeDelete);
            preview = new DocPageMenuItem(page, DocPageMenuItem.InitMode.Tree);
            foreach (var item in preview.MenuItems()) item.IsOpen = true;
            preview.SetEnabled(false);
            Add(preview);
        }
    }
}
