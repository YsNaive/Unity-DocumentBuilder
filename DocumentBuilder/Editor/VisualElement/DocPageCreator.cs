using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocPageCreator : VisualElement
    {
        Button createButton, cancelButton;
        SODocPage parentPage;
        DSTextField nameField;
        public event Action<SODocPage> callback;
        DocPageFactoryField factorySelector;
        VisualElement notValidMsgContainer;
        public DocPageCreator(SODocPage parentPage, Action<SODocPage> callback = null)
        { 
            style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize));
            this.parentPage = parentPage;
            this.callback = callback;
            var hor = new DSHorizontal();
            hor.style.marginBottom = DocStyle.Current.LineHeight;
            hor.style.marginBottom = DocStyle.Current.LineHeight * 0.5f;
            var title = new DSTextElement("Create Page in");
            title.style.width = DocStyle.Current.LabelWidth;
            hor.Add(title);
            hor.Add(new DocPageMenuItem(parentPage, DocPageMenuItem.InitMode.Single));
            Add(hor);

            initNameField();
            factorySelector = new DocPageFactoryField();
            initCreateButton();
            initCancelButton();

            Add(nameField);
            Add(factorySelector);

            notValidMsgContainer = new VisualElement();
            notValidMsgContainer.style.marginTop = DocStyle.Current.LineHeight;
            Add(notValidMsgContainer);

            var btnContainer = new DSHorizontal();
            btnContainer.style.flexGrow = 1f;
            btnContainer.Add(createButton);
            btnContainer.Add(cancelButton);
            Add(btnContainer);

        }
        void initNameField()
        {
            nameField = new DSTextField("Page Name");
        }
        void initCreateButton()
        {
            createButton = new DSButton("Create",DocStyle.Current.SuccessColor);
            createButton.style.flexGrow = 1f;
            createButton.clicked += () =>
            {
                notValidMsgContainer.Clear();
                var valid = validCheck();
                if(valid != "")
                {
                    var com = DocDescription.CreateComponent(valid, DocDescription.DescriptionType.Danger);
                    notValidMsgContainer.Add(DocRuntime.CreateDocVisual(com));
                    return;
                }
                callback?.Invoke(factorySelector.Selecting.CreatePageAsset(parentPage, nameField.value));
            };
        }
        void initCancelButton()
        {
            cancelButton = new DSButton("Cancel");
            cancelButton.style.flexGrow = 1f;
            cancelButton.clicked += () =>
            {
                callback?.Invoke(null);
            };
        }

        string validCheck()
        {
            StringBuilder result = new();

            bool isValid = true;
            var errorMsg = "";
            if (string.IsNullOrEmpty(nameField.value))
            {
                errorMsg = "Page name can't be empty.";
                isValid = false;
            }
            if (isValid)
            {
                if (AssetDatabase.LoadAssetAtPath<Object>($"{DocPageEditorUtils.ValidSubPageFolderPath(parentPage)}/{nameField.value}.asset") != null)
                {
                    isValid = false;
                    errorMsg = "Already exist a sub page with same name.";
                }
            }
            nameField.style.backgroundColor = isValid ? Color.clear : DocStyle.Current.DangerColor;
            if (!string.IsNullOrEmpty(errorMsg))
                result.AppendLine($"* {errorMsg}");

            errorMsg = factorySelector.Selecting.IsValid();
            if(!string.IsNullOrEmpty(errorMsg))
                result.Append(errorMsg);

            return result.ToString();
        }
    }

}