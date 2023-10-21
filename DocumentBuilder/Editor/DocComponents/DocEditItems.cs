using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Charts/Items")]
    public class DocEditItems : DocEditVisual
    {
        [Obsolete] public override string DisplayName => "Advance/Items";

        public override string VisualID => "8";

        private VisualElement itemsVisual;

        protected override void OnCreateGUI()
        {
            DocItems.Data data = setData(Target.JsonData, Target.TextData);
            IntegerField numField = DocEditor.NewIntField("Num", (value) =>
            {
                data.num = value.newValue;
                Target.TextData = setNum(data.num);
                regenerateItemsVisual(data);
                Target.JsonData = JsonUtility.ToJson(data);
            });
            numField.value = data.num;
            this.Add(numField);
            itemsVisual = generateItemsVisual(data);
            this.Add(itemsVisual);
        }

        public override string ToMarkdown(string dstPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in Target.TextData)
            {
                if (str == Target.TextData[^1])
                    stringBuilder.Append("- " + str);
                else
                    stringBuilder.Append("- " + str).AppendLine("<br>");
            }

            return stringBuilder.ToString();
        }

        private DocItems.Data setData(string jsonData, List<string> texts)
        {
            DocItems.Data data = JsonUtility.FromJson<DocItems.Data>(jsonData);
            if (data == null)
            {
                data = new DocItems.Data();
                Target.JsonData = JsonUtility.ToJson(data);
                Target.TextData.Clear();
                Target.TextData.Add("");
            }
            Target.ObjsData.Add(DocEditorData.Instance.BuildinIcon.Find((match) =>
            {
                return match.name == "ItemElement";
            }));

            return data;
        }

        private List<string> setNum(int num)
        {
            List<string> list = new List<string>();
            for (int i = 0;i < num; i++)
            {
                if (i >= Target.TextData.Count)
                    list.Add("");
                else
                    list.Add(Target.TextData[i]);
            }

            return list;
        }

        private void regenerateItemsVisual(DocItems.Data data)
        {
            this.Remove(itemsVisual);
            itemsVisual = generateItemsVisual(data);
            this.Add(itemsVisual);
        }

        private VisualElement generateAddDeleteButton()
        {
            VisualElement root = DocRuntime.NewEmptyHorizontal();

            Button addButton = DocRuntime.NewButton("+", DocStyle.Current.SuccessColor);
            addButton.style.width = Length.Percent(50);

            Button deleteButton = DocRuntime.NewButton("-", DocStyle.Current.DangerColor);
            deleteButton.style.width = Length.Percent(50);

            root.Add(addButton);
            root.Add(deleteButton);

            return root;
        }

        private VisualElement generateItemVisual(DocItems.Data data, int i)
        {
            VisualElement root = DocRuntime.NewEmptyHorizontal();

            DocStyle.Current.BeginLabelWidth(ISLength.Percent(10));
            TextField textField = DocRuntime.NewTextField(i.ToString(), (value) =>
            {
                Target.TextData[i] = value.newValue;
            });
            DocStyle.Current.EndLabelWidth();
            textField[0].style.minWidth = Length.Percent(5);
            textField.style.width = Length.Percent(70);
            textField.value = Target.TextData[i];
            root.Add(textField);
            VisualElement addDeleteVisual = generateAddDeleteButton();
            addDeleteVisual.style.width = Length.Percent(30);
            ((Button)addDeleteVisual[0]).clicked += () =>
            {
                data.num++;
                Target.TextData.Insert(i + 1, "");
                Target.JsonData = JsonUtility.ToJson(data);
                regenerateItemsVisual(data);
            }; 
            ((Button)addDeleteVisual[1]).clicked += () =>
            {
                data.num--;
                Target.TextData.RemoveAt(i);
                Target.JsonData = JsonUtility.ToJson(data);
                regenerateItemsVisual(data);
            };
            root.Add(addDeleteVisual);

            return root;
        }

        private VisualElement generateItemsVisual(DocItems.Data data)
        {
            VisualElement root = DocRuntime.NewEmpty();

            for (int i = 0;i < data.num; i++)
            {
                root.Add(generateItemVisual(data, i));
            }

            return root;
        }
    }
}
