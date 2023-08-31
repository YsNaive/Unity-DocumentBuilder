using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public static class DocEditor
    {
        static DocEditor()
        {
            Reload();
        }
        public static List<string> NameList = new List<string>();
        public static Dictionary<string,string> Name2ID = new Dictionary<string,string>();
        public static Dictionary<string,string> ID2Name = new Dictionary<string,string>();
        public static Dictionary<string, Type> ID2Type = new Dictionary<string, Type>();

        public static void Reload()
        {
            ID2Type.Clear();
            Name2ID.Clear();
            ID2Name.Clear();
            NameList.Clear();
            NameList.Add("None");
            Type baseType = typeof(DocEditVisual);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                    {
                        DocEditVisual doc = (DocEditVisual)System.Activator.CreateInstance(type);
                        NameList.Add(doc.DisplayName);
                        Name2ID.Add(doc.DisplayName, doc.VisualID);
                        ID2Type.Add(doc.VisualID, type);
                        ID2Name.Add(doc.VisualID, doc.DisplayName);
                    }
                }
            }
        }
        public static DocComponentField CreateComponentField(DocComponent docComponent, bool singleMode = false)
        {
            return new DocComponentField(docComponent, singleMode);
        }

        public static ObjectField NewObjectField<T>(EventCallback<ChangeEvent<UnityEngine.Object>> valueChange = null) { return NewObjectField<T>("", valueChange); }
        public static ObjectField NewObjectField<T>(string label = "", EventCallback<ChangeEvent<UnityEngine.Object>> valueChange = null)
        {
            ObjectField objectField = new ObjectField();
            objectField.style.ClearPadding();
            DocRuntime.ApplyMarginPadding(objectField);
            objectField.objectType = typeof(T);
            if(valueChange != null)
                objectField.RegisterValueChangedCallback(valueChange);
            objectField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            objectField[0].style.ClearMarginPadding();
            if(label != "")
            {
                objectField.label = label;
                objectField[0].style.SetIS_Style(DocStyle.Current.MainText);
                objectField[0].style.ClearMarginPadding();
            }
            return  objectField;
        }
        
        public static IntegerField NewIntField(string label, EventCallback<ChangeEvent<int>> valueChange = null)
        {
            IntegerField integerField = new IntegerField();
            integerField.style.ClearPadding();
            DocRuntime.ApplyMarginPadding(integerField);
            integerField[0].style.ClearMarginPadding();
            integerField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            integerField[0].style.minHeight = 18;
            integerField.style.height = 18;
            if (label != "")
            {
                integerField.label = label;
                integerField[0].style.ClearMarginPadding();
                integerField[0].style.minHeight = 18;
            }
            if (valueChange != null)
                integerField.RegisterValueChangedCallback(valueChange);
            return integerField;
        }
        public static EnumField NewEnumField(string label, Enum initValue, EventCallback<ChangeEvent<Enum>> valueChange = null)
        {
            EnumField enumField = new EnumField();
            enumField.style.ClearPadding();
            DocRuntime.ApplyMarginPadding(enumField);
            enumField[0].style.ClearMarginPadding();
            enumField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            enumField[0].style.minHeight = 18;
            if (label != "")
            {
                enumField.label = label;
                enumField[0].style.SetIS_Style(DocStyle.Current.ButtonText);
                enumField[0].style.ClearMarginPadding();
            }
            enumField.Init(initValue);
            if(valueChange != null) 
                enumField.RegisterValueChangedCallback(valueChange);
            return enumField;
        }


        static Texture2D copy;
        static Texture2D paste;
        static Texture2D duplicate;
        static Texture2D delete;
        public static class Icon
        {
            public static Texture2D Copy
            {
                get
                {
                    if (copy == null)
                    {
                        copy = new Texture2D(24, 24);
                        copy.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAQBJREFUWIXdV0EOgyAQhKava3rrY7wZ4s3HeDN+rz1hkO4us7Bo00lMFGRn2FkIOHcxPNfxeL7elkTbupBcX43WxCUhh4/e5JSI/QUhD+OgJgvTLIq4wYEqyJFx3jnd7LkZUf+l5NS4bV08nAEUKdHhncnEnQ3EDKDaqdmhlpEZ0Pqd/x+meX9KYDMQA8Gk40BnIquFHNU1EMkQnyWYFGGLCLNVUCtCrAEuWGvadQKkQmxYfrgARUBk2ekFVATVwHwr1uK/LeB2xxTdLShl8DQLuL6+FgBjRQtadzkEphbUgD0VtxAjqY+nYv5I1nkHjPidi8lZIsSrWU8h3OX0cnwAx3Z+Yn/xS5cAAAAASUVORK5CYII="));
                        copy.filterMode = FilterMode.Point;
                        copy.Apply();
                    }
                    return copy;
                }
            }
            public static Texture2D Paste
            {
                get
                {
                    if (paste == null)
                    {
                        paste = new Texture2D(24, 24);
                        paste.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAPVJREFUWIXNV8sOgyAQhKZf1/TWj+FGiDc/xpvx9+xJgrrsbvehnZMuxBlmQCCEmxF7Da/3Z7UkWuYJ5DoVrYkpIbsXb3JIRH3AyEtOcH0Y1SIeVMceOdXGRQxhP3ruaC1cWeYpkg70Pqqxv8WTIi054TE0/UwFHAm8QArYcByhewQnAU5OsAVQkApUC+BMPkwcaxlqgYl0j2AjLzmBfdwdaEkhJ8wcgAjqT2oYuzHYChCshEsmIQbXCGodcUYsAJrVkgj6u6HBYUMlQAKTCDSbjmkEP5MLI2Odiq3IW4e2U7GJA5rY/udicpUI9GrmKaR3Ob0dX2zvbIDjLIbkAAAAAElFTkSuQmCC"));
                        paste.filterMode = FilterMode.Point;
                        paste.Apply();
                    }
                    return paste;
                }
            }
            public static Texture2D Duplicate
            {
                get
                {
                    if (duplicate == null)
                    {
                        duplicate = new Texture2D(24, 24);
                        duplicate.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAP9JREFUWIXNl0sOgkAMhivxdMadh3FnCDsO4854Pd3oRKCvvy1gV8DM9P/aKfMg2tkOUsPpfHlVCj0fd1Zr8bFa2AKZvKwtzkG0h63E5xBHqUN/u6YE+mF09euIltFnxT321RQz0GCckbT+ILwKgIpHxpgZIKqZEgmsS3v2AghBuDLQnESmxMgelAHJmSZiQbsAfp1EINIAHAQnGIHwZUCJet6GQmA14CxCBMI/BR+nkT+hBGACM4wqCAKZWog4EDRDKgBaUJHpKVuKo7XBLsXwFpwozM02oxKAyEpnjYF2wyiEZu5TcfUh1TwVWw6q7H8uJltBqFezNUGky+nu9ga3m2qKOJV2qgAAAABJRU5ErkJggg=="));
                        duplicate.filterMode = FilterMode.Point;
                        duplicate.Apply();
                    }
                    return duplicate;
                }
            }
            public static Texture2D Delete
            {
                get
                {
                    if (delete == null)
                    {
                        delete = new Texture2D(24, 24);
                        delete.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAANNJREFUWIVjYBhgwIhLwssn+D81Ldq2ZS1WuzAEqW0xIYegcGhtOTZHwBn0shzdEUyUGNKmqcTQpqlEkUMYGRiI8z2pFlVdv0dQzbYtaxlZqGkpLr34HENRFFAD4HUAMcFICBAyA28UUMsRFDmA0lROcQgQaxA6INbhJCdCZIPRywFyQmtw54JRB4w6YNQB9ABEl4QwgFwiopeO5NQbAx4CRDuAlGKWFLUkRQGlNSM2QFSrmFyL8aUJWKuYqBCgZaNk8HRM6OUIvF0zWjoEV+d0wAEABWJKtU+Hsa4AAAAASUVORK5CYII="));
                        delete.filterMode = FilterMode.Point;
                        delete.Apply();
                    }
                    return delete;
                }
            }
        }
    }

}