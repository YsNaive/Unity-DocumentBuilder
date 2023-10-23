using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
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
        DSTextElement errorMsgText;
        Action<SODocPage> callback;
        DocPageFactoryField factorySelector;
        public DocPageCreator(SODocPage parentPage, Action<SODocPage> callback = null)
        {
            this.parentPage = parentPage;
            this.callback = callback;
            style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize));
            Add(new DSLabel("Create SubPage"));
            Add(new DocPageMenuItem(parentPage, DocPageMenuItem.InitMode.Single));
            this[1].style.marginBottom = DocStyle.Current.LineHeight;

            initNameField();
            factorySelector = new DocPageFactoryField();
            initCreateButton();
            initCancelButton();

            Add(nameField);
            Add(errorMsgText);
            Add(factorySelector);

            var btnContainer = DocRuntime.NewEmptyHorizontal();
            btnContainer.style.marginTop = DocStyle.Current.LineHeight;
            btnContainer.style.flexGrow = 1f;
            btnContainer.Add(createButton);
            btnContainer.Add(cancelButton);
            Add(btnContainer);

        }
        void initNameField()
        {
            errorMsgText = new DSTextElement("Page name can't be empty.");
            errorMsgText.style.color = DocStyle.Current.DangerTextColor;
            nameField = new DSTextField("Page Name");
            nameField.style.backgroundColor = DocStyle.Current.DangerColor;
            nameField.RegisterValueChangedCallback(evt =>
            {
                bool isValid = true;
                var errorMsg = "";
                if (string.IsNullOrEmpty(evt.newValue))
                {
                    errorMsg = "Page name can't be empty.";
                    isValid = false;
                }
                if (isValid)
                {
                    if(AssetDatabase.LoadAssetAtPath<Object>($"{DocPageEditorUtils.ValidSubPageFolderPath(parentPage)}/{evt.newValue}.asset") != null)
                    {
                        isValid = false;
                        errorMsg = "Already exist a sub page with same name.";
                    }
                }
                errorMsgText.style.display = isValid ? DisplayStyle.None : DisplayStyle.Flex;
                nameField.style.backgroundColor = isValid ? Color.clear : DocStyle.Current.DangerColor;
                errorMsgText.text = errorMsg;

                createButton.SetEnabled(isValid);
            });
        }
        void initCreateButton()
        {
            createButton = DocRuntime.NewButton("Create",DocStyle.Current.SuccessColor);
            createButton.style.flexGrow = 1f;
            createButton.SetEnabled(false);
            createButton.clicked += () =>
            {
                callback?.Invoke(factorySelector.Selecting.CreatePageAsset(parentPage, nameField.value));
            };
        }
        void initCancelButton()
        {
            cancelButton = DocRuntime.NewButton("Cancel");
            cancelButton.style.flexGrow = 1f;
            cancelButton.clicked += () =>
            {
                callback?.Invoke(null);
            };
        }
    }

}