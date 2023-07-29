using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditItems : DocEditVisual
    {
        public override string DisplayName => "Advance/Items";

        public override string VisualID => "8";

        private VisualElement itemsVisual;

        protected override void OnCreateGUI()
        {
            DocItems.Data data = setData(Target.JsonData, Target.TextData);
            this.Add(generateAnimVisual(data));
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

        private VisualElement generateAnimVisual(DocItems.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.paddingLeft = Length.Percent(1);
            root.style.paddingRight = Length.Percent(1);
            VisualElement childIntro = new VisualElement();
            childIntro.style.SetIS_Style(ISFlex.Horizontal);

            EnumField introField = new EnumField();
            introField.Init(DocItems.AniMode.None);
            introField.value = data.IntroAniMode;
            introField.style.width = Length.Percent(50);
            introField.label = "Intro Mode";
            introField[0].style.minWidth = Length.Percent(30);
            introField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introField.style.ClearMarginPadding();
            childIntro.Add(introField);
            TextField introDurationField = new TextField();
            introDurationField.label = "IntroDuration";
            introDurationField.value = data.IntroDuration.ToString();
            introDurationField.style.width = Length.Percent(50);
            introDurationField[0].style.minWidth = Length.Percent(30);
            introDurationField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introDurationField.visible = data.IntroAniMode != DocItems.AniMode.None;
            introDurationField.style.ClearMarginPadding();
            childIntro.Add(introDurationField);
            introField.RegisterValueChangedCallback(value =>
            {
                data.IntroAniMode = (DocItems.AniMode)value.newValue;
                introDurationField.visible = data.IntroAniMode != DocItems.AniMode.None;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            introDurationField.RegisterValueChangedCallback(value =>
            {
                if (int.TryParse(value.newValue, out int duration))
                {
                    data.IntroDuration = duration;
                    Target.JsonData = JsonUtility.ToJson(data);
                }
            });
            VisualElement childOuttro = new VisualElement();
            childOuttro.style.SetIS_Style(ISFlex.Horizontal);
            EnumField outtroField = new EnumField();
            outtroField.Init(DocItems.AniMode.None);
            outtroField.label = "Outtro Mode";
            outtroField.value = data.IntroAniMode;
            outtroField.style.width = Length.Percent(50);
            outtroField[0].style.minWidth = Length.Percent(30);
            outtroField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroField.style.ClearMarginPadding();
            childOuttro.Add(outtroField);
            TextField outtroDurationField = new TextField();
            outtroDurationField.label = "OuttroDuration";
            outtroDurationField.value = data.OuttroDuration.ToString();
            outtroDurationField.style.width = Length.Percent(50);
            outtroDurationField[0].style.minWidth = Length.Percent(30);
            outtroDurationField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroDurationField.visible = data.OuttroAniMode != DocItems.AniMode.None;
            outtroDurationField.style.ClearMarginPadding();
            childOuttro.Add(outtroDurationField);
            outtroField.RegisterValueChangedCallback(value =>
            {
                data.OuttroAniMode = (DocItems.AniMode)value.newValue;
                outtroDurationField.visible = data.OuttroAniMode != DocItems.AniMode.None;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            outtroDurationField.RegisterValueChangedCallback(value =>
            {
                if (int.TryParse(value.newValue, out int duration))
                {
                    data.OuttroDuration = duration;
                    Target.JsonData = JsonUtility.ToJson(data);
                }
            });
            root.Add(childIntro);
            root.Add(childOuttro);

            return root;
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

            TextField textField = DocRuntime.NewTextField(i.ToString(), (value) =>
            {
                Target.TextData[i] = value.newValue;
            });
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
