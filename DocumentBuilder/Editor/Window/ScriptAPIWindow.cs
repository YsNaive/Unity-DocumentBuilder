using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIWindow : EditorWindow
    {
        [System.Serializable]
        public class Settings : ISerializationCallbackReceiver
        {
            public float SplitViewPercent = 25;
            public Type ActiveType;
            [SerializeField] string s_ActiveType;

            public void OnAfterDeserialize()
            {
                ActiveType = Type.GetType(s_ActiveType);
            }

            public void OnBeforeSerialize()
            {
                if (ActiveType == null)
                    s_ActiveType = "";
                else
                    s_ActiveType = ActiveType.AssemblyQualifiedName;
            }
        }
        Settings settings;
        Stack<Type> VisitHistory = new();
        DSTypeField typeField;
        DSScrollView LeftPanel, RightPanelScrollView;
        VisualElement RightPanel;
        VisualElement RightPanelHeader;
        PopupElement MemberInfoPopup;
        DSScrollView MemberInfoScrollView;
        SOScriptAPIInfo currentAPIInfo;
        SplitView splitView;
        private void CreateGUI()
        {
            settings = JsonUtility.FromJson<Settings>(DocCache.LoadData("SciprtAPIWindowSettings.json"));
            settings ??= new();
            var padding = ISPadding.Pixel(5);
            LeftPanel = new();
            RightPanel = new();
            RightPanelHeader = new();
            RightPanelHeader.style.flexShrink = 0;
            RightPanelScrollView = new();
            MemberInfoScrollView = new();
            MemberInfoScrollView.style.SetIS_Style(padding);
            MemberInfoPopup = new() { AutoClose = true };
            MemberInfoPopup.style.backgroundColor = DocStyle.Current.BackgroundColor;
            var bc = DocStyle.Current.BackgroundColor;
            bc.a = 0.6f;
            MemberInfoPopup.CoverMask.style.backgroundColor = bc;
            MemberInfoPopup.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1.5f));
            MemberInfoPopup.Add(MemberInfoScrollView);
            MemberInfoPopup.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == (int)MouseButton.RightMouse)
                {
                    MemberInfoPopup.Close();
                }
            });
            typeField = new DSTypeField();
            var root = rootVisualElement;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            LeftPanel.mode = ScrollViewMode.VerticalAndHorizontal;
            LeftPanel.style.SetIS_Style(padding);
            RightPanel.style.SetIS_Style(padding);
            RightPanel.Add(RightPanelHeader);
            RightPanel.Add(RightPanelScrollView);
            splitView = new SplitView(FlexDirection.Row, 25);
            splitView.Add(LeftPanel);
            splitView.Add(RightPanel);
            splitView.SplitPercent = settings.SplitViewPercent;
            root.Add(splitView);
            typeField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == evt.previousValue) return;
                VisitHistory.Clear();
                repaintTypeSelect(evt.newValue);
            });
            LeftPanel.Add(typeField);
            typeField.value = settings.ActiveType;
            foreach (var type in TypeReader.FindAllTypesWhere(t => { return t.IsSubclassOf(typeof(ScriptAPIMenuDefinition)); }))
                LeftPanel.Add(((ScriptAPIMenuDefinition)Activator.CreateInstance(type)).CreateFoldoutHierarchy(null, ve =>
                {
                    ve.RegisterCallback<PointerEnterEvent>(evt =>
                    {
                        ve.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    });
                    ve.RegisterCallback<PointerLeaveEvent>(evt =>
                    {
                        ve.style.backgroundColor = Color.clear;
                    });
                    ve.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        VisitHistory.Clear();
                        typeField.value = ve.TargetType;
                        repaintTypeSelect(ve.TargetType);
                    });
                }));
        }
        private void OnDisable()
        {
            settings = JsonUtility.FromJson<Settings>(DocCache.LoadData("SciprtAPIWindowSettings.json"));
            settings ??= new();
            if(splitView != null)
                settings.SplitViewPercent = splitView.SplitPercent;
            if (VisitHistory != null && VisitHistory.Count > 0)
                settings.ActiveType = VisitHistory.Peek();
            DocCache.SaveData("SciprtAPIWindowSettings.json", JsonUtility.ToJson(settings));
        }
        void repaintTypeSelect(Type type)
        {
            RightPanelScrollView.Clear();
            RightPanelHeader.Clear();
            if (type == null) return;
            VisitHistory.Push(type);
            if(VisitHistory.Count != 1)
            {
                var hor = new DSHorizontal();
                hor.style.opacity = 0.6f;
                hor.style.flexWrap = Wrap.Wrap;
                hor.style.flexShrink = 2;
                int i = VisitHistory.Count;
                foreach (var item in VisitHistory.Reverse())
                {
                    var typeVe = new DSTypeNameElement(item);
                    var popCount = i;
                    i--;
                    typeVe.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        Type pre = null;
                        while(popCount-->0)
                            pre = VisitHistory.Pop();
                        repaintTypeSelect(pre);
                    });
                    if (i != VisitHistory.Count - 1)
                    {
                        var arrow = new VisualElement();
                        arrow.style.SetIS_Style(DocStyle.Current.ArrowIcon);
                        typeVe.Insert(0, arrow);
                    }
                    hor.Add(typeVe);
                }
                RightPanelHeader.Add(hor);
            }
            ScriptAPIInfoHolder.Infos.TryGetValue(type, out currentAPIInfo);
            var ve = new DSTypeElement(type);
            foreach (var typeText in ve.VisitTypeName())
            {
                var typeVe = typeText;
                var text = typeVe.NameText.text;
                var uText = $"<u>{text}</u>";
                typeVe.NameText.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    typeVe.NameText.text = uText;
                });
                typeVe.NameText.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    typeVe.NameText.text = text;
                });
                typeVe.NameText.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.pointerId != (int)MouseButton.LeftMouse) return;
                    if (evt.modifiers != EventModifiers.Control) return;
                    if (typeVe.TargetType == VisitHistory.Peek()) return;
                    repaintTypeSelect(typeVe.TargetType);
                });
            }
            foreach(var tuple in ve.VisitMember())
            {
                var element = tuple.element;
                element.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    element.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                });
                element.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    element.style.backgroundColor = Color.clear;
                });
                element.RegisterCallback<PointerDownEvent>(evt =>
                {
                    openMemberInfo(tuple.memberInfo);
                });
            }
            RightPanelScrollView.Add(ve);
        }

        void openMemberInfo(ICustomAttributeProvider info)
        {
            MemberInfoPopup.Open(rootVisualElement);
            MemberInfoPopup.style.width = RightPanel.layout.width - DocStyle.Current.LineHeight.Value*2;
            MemberInfoPopup.style.height = RightPanel.layout.height - DocStyle.Current.LineHeight.Value*2;
            var pos = MemberInfoPopup.parent.WorldToLocal(RightPanel.LocalToWorld(Vector2.zero));
            MemberInfoPopup.style.left = pos.x + DocStyle.Current.LineHeight.Value;
            MemberInfoPopup.style.top = pos.y + DocStyle.Current.LineHeight.Value;

            MemberInfoScrollView.Clear();
            var fontSize = DocStyle.Current.MainTextSize;
            DocStyle.Current.MainTextSize = (int)(DocStyle.Current.MainTextSize * 1.25f);
            MemberInfoScrollView.Add(DSScriptAPIElement.Create(info));
            DocStyle.Current.MainTextSize = fontSize;
            if(currentAPIInfo != null)
            {
                var memberAPIInfo = currentAPIInfo.GetMemberInfo(info);
                foreach(var com in memberAPIInfo.Description)
                {
                    var visual = DocVisual.Create(com);
                    visual.style.marginLeft = DocStyle.Current.LineHeight;
                    MemberInfoScrollView.Add(visual);
                }
                if (typeof(MethodBase).IsAssignableFrom(info.GetType()))
                {
                    var paramInfos = (info as MethodBase).GetParameters();
                    if(paramInfos.Length > 0)
                    {
                        MemberInfoScrollView.Add(DocVisual.Create(DocDividline.CreateComponent()));
                        foreach(var paramInfo in paramInfos)
                        {
                            MemberInfoScrollView.Add(DSScriptAPIElement.Create(paramInfo));
                            var paramAPIInfo = currentAPIInfo.GetMemberInfo(paramInfo);
                            foreach (var com in paramAPIInfo.Description.Count!=0? paramAPIInfo.Description: paramAPIInfo.Tooltip)
                            {
                                var visual = DocVisual.Create(com);
                                visual.style.marginLeft = DocStyle.Current.LineHeight;
                                MemberInfoScrollView.Add(visual);
                            }
                        }
                    }
                }
            }
        }
    }

}