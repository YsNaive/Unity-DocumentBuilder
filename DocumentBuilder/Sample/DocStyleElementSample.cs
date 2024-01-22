using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.Sample
{
    public class DocStyleElementSample : MonoBehaviour
    {
        UIDocument UID;
        VisualElement root;
        public SODocStyle UsingStyle;
        public SODocPage PreviewPage;
        void Start()
        {
            if (UsingStyle != null) DocStyle.Current = UsingStyle.Get();
            UID = FindObjectOfType<UIDocument>();
            root = new DSScrollView();
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            UID.rootVisualElement.Add(root);

            root.Add(new DSLabel("This is Label"));
            root.Add(new DSTextElement("This is TextElement"));
            root.Add(new DSTextField("TextField"));
            root.Add(new DSDropdown("Dropdown") { choices = new List<string>() { "Select A", "Select B", "Select C", "Sub Menu/Item 1", "Sub Menu/Item 2", "Sub Menu/Item 3" } });
            root.Add(new DSButton("Button", () => { Debug.Log("Click !"); }));
            var foldout = new DSFoldout("Foldout");
            foldout.Add(new DSTextElement("Some contents..."));
            root.Add(foldout);
            var scrollview = new DSScrollView();
            scrollview.Add(new DSTextElement("ScrollView line 1"));
            scrollview.Add(new DSTextElement("ScrollView line 2"));
            scrollview.Add(new DSTextElement("ScrollView line 3"));
            scrollview.Add(new DSTextElement("ScrollView line 4"));
            scrollview.Add(new DSTextElement("ScrollView line 5"));
            scrollview.Add(new DSTextElement("ScrollView line 6"));
            scrollview.Add(new DSTextElement("ScrollView line 7"));
            scrollview.Add(new DSTextElement("ScrollView line 8"));
            scrollview.Add(new DSTextElement("ScrollView line 9"));
            scrollview.style.height = 200;
            root.Add(scrollview);
            foreach (var ve in root.Children())
                ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding);
        }
    }
}
