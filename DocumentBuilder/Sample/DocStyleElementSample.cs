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
        void Start()
        {
            //if (UsingStyle != null) DocStyle.Current = UsingStyle.Get();
            UID = FindObjectOfType<UIDocument>();
            root = DocRuntime.NewContainer();
            UID.rootVisualElement.Add(root);

            root.Add(DocRuntime.NewLabel("This is Label"));
            root.Add(DocRuntime.NewTextElement("This is TextElement"));
            root.Add(DocRuntime.NewTextField("TextField"));
            root.Add(DocRuntime.NewDropdownField("Unity Dropdown", new List<string>() { "Select A", "Select B", "Select C" }));
            root.Add(DocRuntime.NewDropdown("Custom Dropdown", new List<string>() { "Select A", "Select B", "Select C" }, (e) => { Debug.Log("Select " + e); }));
            root.Add(DocRuntime.NewButton("Button", () => { Debug.Log("Click !"); }));
            root.Add(DocRuntime.NewCheckButton("CheckButton", () => { Debug.Log("YES !"); }));
            var foldout = DocRuntime.NewFoldout("Foldout");
            foldout.Add(DocRuntime.NewTextElement("Some contents..."));
            root.Add(foldout);
            var scrollview = DocRuntime.NewScrollView();
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 1"));
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 2"));
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 3"));
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 4"));
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 5"));
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 6"));
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 7"));
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 8"));
            scrollview.Add(DocRuntime.NewTextElement("ScrollView line 9"));
            scrollview.style.height = 200;
            root.Add(scrollview);
            foreach (var ve in root.Children())
                ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding);


        }
    }
}
