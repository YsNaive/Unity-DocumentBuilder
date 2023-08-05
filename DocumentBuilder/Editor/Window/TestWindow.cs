using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace NaiveAPI.DocumentBuilder
{
    public class TestWindow : EditorWindow
    {
        static TestWindow window;
        [MenuItem("Tools/NaiveAPI/Test Window")]
        public static void ShowWindow()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }
            window = CreateWindow<TestWindow>("Test Window");
        }

        private void CreateGUI()
        {
            const string commonChar = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            const string spChar = "\\n\\t\\r\\0\\b\\f\\v\\\\\\'\\";
            var obj = DocEditor.NewObjectField<SODocPage>("");
            rootVisualElement.Add(obj);
            var textPrint = DocRuntime.NewTextElement("");
            textPrint.enableRichText = false;
            var btn = DocRuntime.NewButton("OK", () =>
            {
                Queue<SODocPage> queue = new Queue<SODocPage>();
                List<SODocPage> pagelist = new List<SODocPage>();
                queue.Enqueue((SODocPage)obj.value);
                while (queue.Count > 0)
                {
                    var now = queue.Dequeue();
                    foreach(var sub in now.SubPages)
                    {
                        if(!pagelist.Contains(sub))
                        {
                            pagelist.Add(sub);
                            queue.Enqueue(sub);
                        }
                    }
                }
                string result = "";
                Debug.Log(pagelist.Count);
                foreach(var page in pagelist)
                {
                    foreach(var com in page.Components)
                    {
                        foreach(var str in com.TextData)
                        {
                            foreach(var c in str)
                            {
                                if (spChar.Contains(c))
                                    continue;
                                if (commonChar.Contains(c))
                                    continue;
                                if(!result.Contains(c))
                                    result += c;
                            }
                        }
                    }
                }
                result = commonChar + result;
                textPrint.text = result;
            });
            rootVisualElement.Add(btn);
            rootVisualElement.Add(textPrint);
        }
    }
}
