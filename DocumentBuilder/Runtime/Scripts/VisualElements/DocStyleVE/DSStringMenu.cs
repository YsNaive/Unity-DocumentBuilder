using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static PlasticGui.LaunchDiffParameters;

namespace NaiveAPI.DocumentBuilder
{
    public static class DSStringMenu
    {
        private class MenuContainer : PopupElement
        {
            public event Action<string> OnSelected;
            public List<MenuContainer> ChildMenuContainer = new();
            public MenuContainer(node node, Action<string> callback = null)
            {
                if (callback != null)
                    OnSelected += callback;
                OnSelected += str=> { Close(); };
                style.minWidth = DocStyle.Current.LabelWidth;
                style.backgroundColor = DocStyle.Current.BackgroundColor;
                style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1));
                var container = new DSScrollView();

                EventCallback<GeometryChangedEvent> registerMaxSize = null; registerMaxSize = evt =>
                {
                    container.style.maxHeight = container.panel.visualTree.worldBound.height * 0.75f;
                    container.style.maxWidth = container.panel.visualTree.worldBound.width * 0.75f;
                    container.UnregisterCallback(registerMaxSize);
                };  container.RegisterCallback(registerMaxSize);

                Add(container);

                foreach (var choice in node.choices)
                {
                    var textElement = createMenuText(choice);
                    textElement.RegisterCallback<PointerDownEvent>(evt =>
                    { OnSelected?.Invoke($"{(node.path == "" ? "" : $"{node.path}/")}{choice}"); });
                    textElement.RegisterCallback<PointerEnterEvent>(evt => { textElement.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
                    textElement.RegisterCallback<PointerLeaveEvent>(evt => { textElement.style.backgroundColor = Color.clear; });
                    textElement.RegisterCallback<PointerDownEvent>(evt => { textElement.style.backgroundColor = Color.clear; });
                    container.Add(textElement);
                }

                if (node.children != null)
                {
                    foreach (var child in node.children)
                    {
                        var title = createSubMenu(child);
                        container.Add(title);
                    }
                }
            }
            DSTextElement createMenuText(string text)
            {
                var textElement = new DSTextElement(text);
                textElement.style.paddingLeft = DocStyle.Current.LineHeight.Value / 2f;
                textElement.style.paddingTop = textElement.style.paddingBottom = 2;
                textElement.style.borderBottomWidth = 1.5f;
                textElement.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
                textElement.style.whiteSpace = WhiteSpace.NoWrap;
                return textElement;
            }
            DSTextElement createSubMenu(node node)
            {
                var title = createMenuText(node.value);
                var arrow = new VisualElement();
                arrow.style.SetIS_Style(DocStyle.Current.ArrowIcon);
                arrow.style.marginRight = DocStyle.Current.MainTextSize / 2f;
                arrow.style.marginLeft = StyleKeyword.Auto;
                title.RegisterCallback<PointerEnterEvent>(evt => { title.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
                title.RegisterCallback<PointerLeaveEvent>(evt => { title.style.backgroundColor = Color.clear; });
                title.RegisterCallback<PointerDownEvent>(evt => { title.style.backgroundColor = Color.clear; });
                title.Add(arrow);
                var childMenu = new MenuContainer(node, OnSelected);
                childMenu.mask.pickingMode = PickingMode.Ignore;
                OnClosed += childMenu.Close;
                bool isInside = false;
                title.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    isInside = true;
                    title.schedule.Execute(() =>
                    {
                        if (!isInside) return;
                        foreach (var child in ChildMenuContainer)
                            child.Close();
                        childMenu.Open(this);
                        EventCallback<GeometryChangedEvent> changeEvt = null;
                        changeEvt = evt =>
                        {
                            var pos = new Vector2(0, title.worldBound.y - 1);
                            if (childMenu.parent.worldBound.xMax - title.worldBound.xMax > childMenu.resolvedStyle.width)
                            {
                                pos.x = title.worldBound.xMax;
                            }
                            else
                            {
                                pos.x = title.worldBound.x - childMenu.resolvedStyle.width;
                                pos.y += DocStyle.Current.LineHeight.Value / 2f;
                            }

                            pos = childMenu.parent.WorldToLocal(pos);
                            childMenu.style.left = pos.x;
                            childMenu.style.top = pos.y;
                            childMenu.UnregisterCallback(changeEvt);
                        }; childMenu.RegisterCallback(changeEvt);
                    }).ExecuteLater(150);
                });
                childMenu.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    foreach (var child in childMenu.ChildMenuContainer)
                        if (child.IsOpend) return;
                    childMenu.Close();
                });
                title.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    isInside = false;
                });
                ChildMenuContainer.Add(childMenu);
                return title;
            }
        }
        private class node
        {
            public string path = "";
            public string value = "";
            public List<node> children;
            public List<string> choices = new();
            public void AddChoice(string value)
            {
                choices.Add(value);
            }
            public void AddChild(string value)
            {
                children ??= new();
                children.Add(new node { value = value, path = $"{(path == "" ? "" : $"{path}/")}{value}" });
            }
        }
        private static node parseChoices(List<string> choices)
        {
            node rootNode = new node() { value = ""};
            foreach(var choice in choices)
            {
                var path = choice.Split('/', StringSplitOptions.RemoveEmptyEntries);
                node current = rootNode;
                for(int i=0,imax = path.Length - 1; i < imax;i++)
                {
                    bool found = false;
                    if(current.children != null)
                    {
                        foreach (var n in current.children)
                        {
                            if (n.value == path[i])
                            {
                                found = true;
                                current = n;
                                break;
                            }
                        }
                    }
                    if (found) continue;
                    current.AddChild(path[i]);
                    current = current.children[^1];
                }
                current.AddChoice(path[^1]);
            }
            return rootNode;
        }
        public static PopupElement CreatePopupMenu(List<string> choices, Action<string> callback)
        {
            return new MenuContainer(parseChoices(choices), callback);
        }
        public static void Open(IPanel panel, Vector2 position, List<string> choices, Action<string> callback)
        {
            PopupElement popup = CreatePopupMenu(choices, callback);
            popup.Open(panel.visualTree);
            position = popup.parent.WorldToLocal(position);
            popup.style.left = position.x;
            popup.style.top = position.y;
        }
    }

}