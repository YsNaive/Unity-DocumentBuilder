using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public static class DSStringMenu
    {
        private class node
        {
            public string value;
            public List<node> children;
            public List<string> choices = new();
            public void AddChoice(string value)
            {
                choices.Add(value);
            }
            public void AddChild(string value)
            {
                children ??= new();
                children.Add(new node { value = value });
            }
        }
        private static node parseChoices(List<string> choices)
        {
            node rootNode = new node() { value = "root"};
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

        private static void createMenu(node rootNode, Action<string> callback, PopupElement popupElement, string path = "")
        {
            popupElement.style.backgroundColor = DocStyle.Current.BackgroundColor;
            popupElement.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1));
            popupElement.style.minWidth = DocStyle.Current.LabelWidth;
            var container = new DSScrollView();
            container.style.backgroundColor = DocStyle.Current.BackgroundColor;
            container.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                container.style.maxHeight = container.panel.visualTree.worldBound.height * 0.75f;
                container.style.maxWidth = container.panel.visualTree.worldBound.width * 0.75f;
            });
            popupElement.Add(container);
            foreach (var choice in rootNode.choices)
            {
                var textElement = new DSTextElement(choice);
                textElement.style.paddingLeft = DocStyle.Current.LineHeight.Value / 2f;
                textElement.style.paddingTop = textElement.style.paddingBottom = 2;
                textElement.style.borderBottomWidth = 1.5f;
                textElement.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
                textElement.style.whiteSpace = WhiteSpace.NoWrap;
                textElement.RegisterCallback<PointerDownEvent>(evt =>
                {
                    callback?.Invoke($"{path}{(path == "" ? "" : "/")}{choice}");
                });
                textElement.RegisterCallback<PointerEnterEvent>(evt => { textElement.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
                textElement.RegisterCallback<PointerLeaveEvent>(evt => { textElement.style.backgroundColor = Color.clear; });
                textElement.RegisterCallback<PointerDownEvent>(evt => { textElement.style.backgroundColor = Color.clear; });
                container.Add(textElement);
            }
            if(rootNode.children != null)
            {
                foreach (var node in rootNode.children)
                {
                    var child = createChildMenu(node, callback, path);
                    container.Add(child.menu);
                    child.popup.OnClosed+= popupElement.Close;
                }
            }
        }
        private static (VisualElement menu, PopupElement popup) createChildMenu(node node, Action<string> callback, string path = "")
        {
            var childMenuContainer = new VisualElement();
            var childMenuTitle = new DSTextElement(node.value);
            childMenuTitle.style.paddingLeft = DocStyle.Current.LineHeight.Value / 2f;
            var arrow = new VisualElement();
            arrow.style.width = DocStyle.Current.LineHeight;
            arrow.style.height = DocStyle.Current.LineHeight;
            arrow.style.scale = new Scale(new Vector3(.7f, .7f, .7f));
            arrow.style.unityBackgroundImageTintColor = DocStyle.Current.FrontgroundColor;
            arrow.style.backgroundImage = SODocStyle.WhiteArrow;
            arrow.style.marginRight = 1;
            arrow.style.marginLeft = StyleKeyword.Auto;
            var titleHor = DocRuntime.NewEmptyHorizontal();
            EventCallback<PointerLeaveEvent> clearColor = evt => { titleHor.style.backgroundColor = Color.clear; };
            titleHor.RegisterCallback<PointerEnterEvent>(evt => { titleHor.style.backgroundColor = DocStyle.Current.SubBackgroundColor; });
            titleHor.Add(childMenuTitle);
            titleHor.Add(arrow);
            titleHor.style.borderBottomWidth = 1.5f;
            titleHor.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
            childMenuContainer.Add(titleHor);
            var childPopup = new PopupElement();
            childPopup.OnOpend += e=> { titleHor.UnregisterCallback(clearColor); };
            childPopup.OnClosed += () => { titleHor.RegisterCallback(clearColor); };
            titleHor.RegisterCallback(clearColor);
            createMenu(node, callback, childPopup,$"{path}{(path==""?"":"/")}{node.value}");
            titleHor.RegisterCallback<PointerDownEvent>(evt =>
            {
                childPopup.Open(childMenuTitle);
                var pos = new Vector2(titleHor.worldBound.xMax, childMenuTitle.worldBound.y - 1);
                pos = childPopup.parent.WorldToLocal(pos);
                childPopup.style.left = pos.x;
                childPopup.style.top = pos.y;
                titleHor.UnregisterCallback(clearColor);
            });
            return (childMenuContainer,childPopup);
        }
        public static PopupElement CreatePopupMenu(List<string> choices, Action<string> callback)
        {
            PopupElement popup = new PopupElement();
            createMenu(parseChoices(choices), callback, popup);
            return popup;
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