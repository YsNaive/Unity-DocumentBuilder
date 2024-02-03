using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
namespace NaiveAPI.DocumentBuilder
{
    public class DSTypeField : VisualElement, INotifyValueChanged<Type>
    {
        private class SearchInfo
        {
            public Type type;
            public string name, lowcaseName, space;
        }

        const string settingsPath = "DSTypeFieldSettings.txt";
        public static bool IgnoreCase
        {
            get => m_IgnoreCase;
            set
            {
                m_IgnoreCase = value;
                saveSettings();
            }
        }
        private static bool m_IgnoreCase = true;
        public static int ChoiceDisplayCount
        {
            get => m_ChoiceDisplayCount;
            set
            {
                m_ChoiceDisplayCount = value;
                saveSettings();
            }
        }
        private static int m_ChoiceDisplayCount = 15;
        static void loadSettings()
        {
            var datas = DocCache.LoadData(settingsPath).Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (datas.Length < 2)
                return;
            else
            {
                IgnoreCase = bool.Parse(datas[0]);
                ChoiceDisplayCount = int.Parse(datas[1]);
            }
        }
        static void saveSettings()
        {
            DocCache.SaveData(settingsPath, $"{IgnoreCase}\n{ChoiceDisplayCount}");
        }

        private static readonly List<SearchInfo> SearchTable = new();
        private static PopupElement popup;
        private static PopupElement settingsPopup;
        private static DSScrollView choiceContainer;

        static DSTypeField()
        {
            loadSettings();
            popup = new PopupElement();
            popup.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1));
            popup.style.minHeight = DocStyle.Current.LineHeight;
            popup.style.minWidth = DocStyle.Current.LabelWidth;
            popup.style.backgroundColor = DocStyle.Current.BackgroundColor;
            choiceContainer = new DSScrollView();
            choiceContainer.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                choiceContainer.style.maxHeight = choiceContainer.panel.visualTree.worldBound.height * 0.75f;
                choiceContainer.style.maxWidth = choiceContainer.panel.visualTree.worldBound.width * 0.75f;
            });
            popup.Add(choiceContainer);

            var padding = ISPadding.Pixel(5);
            settingsPopup = new PopupElement();
            settingsPopup.style.SetIS_Style(padding);
            settingsPopup.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1));
            settingsPopup.style.minHeight = DocStyle.Current.LineHeight;
            settingsPopup.style.minWidth = DocStyle.Current.LabelWidth;
            settingsPopup.style.backgroundColor = DocStyle.Current.BackgroundColor;

            var toggle = new DSToggle("Ignore Case");
            toggle.value = IgnoreCase;
            toggle.RegisterValueChangedCallback(evt =>
            {
                IgnoreCase = evt.newValue;
            });
            var count = new DSTextField("Choice Count");
            count.value = ChoiceDisplayCount.ToString();
            count.RegisterCallback<FocusOutEvent>(evt =>
            {
                int newVal;
                if (int.TryParse(count.value, out newVal))
                {
                    ChoiceDisplayCount = newVal;
                }
                else
                {
                    count.SetValueWithoutNotify(ChoiceDisplayCount.ToString());
                }
            });
            settingsPopup.Add(toggle);
            settingsPopup.Add(count);

            foreach (var type in TypeReader.ActiveTypes)
            {
                var name = TypeReader.GetName(type);
                var cutIndex = name.IndexOf('<');
                if (cutIndex > 0)
                    name = name.Substring(0, cutIndex - 1);
                var space = type.Namespace == null ? "" : type.Namespace;
                var lowcaseName = name.ToLower();
                SearchTable.Add(new SearchInfo()
                {
                    type = type,
                    name = name,
                    lowcaseName = lowcaseName,
                    space = space,
                });
            }
        }

        public Type value
        {
            get => m_value;
            set
            {
                using var evt = ChangeEvent<Type>.GetPooled(m_value, value);
                evt.target = this;
                SendEvent(evt);

                m_value = value;
                ((INotifyValueChanged<string>)searchField).SetValueWithoutNotify(TypeReader.GetName(value));
            }
        }
        Type m_value;
        
        DSTextField searchField;
        public DSTypeField(string label = "")
        {
            style.minHeight = DocStyle.Current.LineHeight;
            searchField = new DSTextField(label);
            Add(searchField);
            searchField.RegisterCallback<FocusInEvent>(evt =>
            {
                if (evt.target != searchField) return;
                choiceContainer.Clear();
                addNullChoice();
                openPopup(popup);
            });
            searchField.RegisterValueChangedCallback(evt =>
            {
                choiceContainer.Clear();
                if (evt.newValue == "")
                {
                    addNullChoice();
                    return;
                }
                Dictionary<int, List<SearchInfo>> searchResult = new();
                var input = evt.newValue;
                if(IgnoreCase) input = input.ToLower();
                foreach (var info in SearchTable)
                {
                    if (Math.Abs(input.Length - info.name.Length) > 12) continue;
                    var comapreTo = IgnoreCase ? info.lowcaseName : info.name;
                    var distance = input.LevenshteinDistance(comapreTo);
                    if (comapreTo.StartsWith(input))
                        distance /= 2;
                    if (comapreTo.Contains(input))
                        distance /= 2;
                    if (!searchResult.ContainsKey(distance))
                        searchResult.Add(distance, new());
                    searchResult[distance].Add(info);
                }
                var list = searchResult.OrderBy(m => { return m.Key; }).ToList();
                int i = 1;
                foreach (var pair in list)
                {
                    foreach (var val in pair.Value)
                    {
                        var hor = new DSHorizontal();
                        hor.Add(new DSTypeNameElement(val.type));
                        hor.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
                        hor.RegisterCallback<PointerEnterEvent>(evt => { hor.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
                        hor.RegisterCallback<PointerLeaveEvent>(evt => { hor.style.backgroundColor = DocStyle.Current.BackgroundColor; });
                        hor.style.paddingLeft = DocStyle.Current.MainTextSize;
                        hor.style.paddingRight = DocStyle.Current.MainTextSize;
                        hor.style.minHeight = DocStyle.Current.LineHeight;
                        hor.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
                        hor.style.borderBottomWidth = 1.5f;
                        hor.style.flexShrink = 0;
                        hor.RegisterCallback<PointerDownEvent>(evt => { value = val.type; });
                        var spaceName = new DSTextElement("  " + val.space);
                        spaceName.style.opacity = 0.7f;
                        hor.Add(spaceName);
                        choiceContainer.Add(hor);
                        i++;
                        if (i > m_ChoiceDisplayCount) break;
                    }
                    if (i > m_ChoiceDisplayCount) break;
                }
                addNullChoice();
                openPopup(popup);
            });
            searchField.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (value != null)
                    searchField.SetValueWithoutNotify(TypeReader.GetName(value));
                else
                    searchField.SetValueWithoutNotify("");
                popup.Close();
            });
            var editBtn = new VisualElement();
            editBtn.style.SetIS_Style(DocStyle.Current.IconStyle);
            editBtn.style.backgroundImage = new StyleBackground(DocStyle.Current.GearSprite);
            editBtn.RegisterCallback<PointerDownEvent>(evt => { openPopup(settingsPopup); });
            searchField.Add(editBtn);
        }
        void addNullChoice()
        {
            var nullChoice = new DSTextElement("Null");
            nullChoice.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
            nullChoice.RegisterCallback<PointerEnterEvent>(evt => { nullChoice.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
            nullChoice.RegisterCallback<PointerLeaveEvent>(evt => { nullChoice.style.backgroundColor = DocStyle.Current.BackgroundColor; });
            nullChoice.style.paddingLeft = DocStyle.Current.MainTextSize;
            nullChoice.style.paddingRight = DocStyle.Current.MainTextSize;
            nullChoice.style.minHeight = DocStyle.Current.LineHeight;
            nullChoice.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
            nullChoice.style.borderBottomWidth = 1.5f;
            nullChoice.RegisterCallback<PointerDownEvent>(evt => { value = null; });
            var c = nullChoice.style.color.value; c.a = 0.7f;
            nullChoice.style.color = c;
            choiceContainer.Add(nullChoice);
        }
        void openPopup(PopupElement popup)
        {
            popup.Open(this);
            var pos = searchField.InputFieldElement.worldBound.position;
            pos.y += searchField.InputFieldElement.worldBound.height;
            pos = popup.CoverMask.WorldToLocal(pos);
            popup.style.left = pos.x;
            popup.style.top = pos.y;
        }
        public void SetValueWithoutNotify(Type newValue)
        {
            m_value = newValue;
        }
    }
}
