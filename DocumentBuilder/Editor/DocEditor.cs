using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public static class DocEditor
    {
        public static ObjectField NewObjectField<T>(EventCallback<ChangeEvent<UnityEngine.Object>> valueChange = null) { return NewObjectField<T>("", valueChange); }
        public static ObjectField NewObjectField<T>(string label = "", EventCallback<ChangeEvent<UnityEngine.Object>> valueChange = null)
        {
            ObjectField objectField = new ObjectField();
            objectField.style.ClearMarginPadding();
            objectField.objectType = typeof(T);
            if(valueChange != null)
                objectField.RegisterValueChangedCallback(valueChange);
            objectField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            objectField[0].Q<Label>().style.SetIS_Style(DocStyle.Current.MainText);
            if (label != "")
            {
                objectField.label = label;
                objectField[0].style.SetIS_Style(DocStyle.Current.MainText);
                objectField[0].style.width = DocStyle.Current.LabelWidth;
                objectField[0].style.minWidth = 0;
            }
            return  objectField;
        }
        
        public static IntegerField NewIntField(string label, EventCallback<ChangeEvent<int>> valueChange = null)
        {
            IntegerField integerField = new IntegerField();
            integerField.style.ClearMarginPadding();
            integerField.style.minHeight = DocStyle.Current.LineHeight;
            integerField[0].style.SetIS_Style(DocStyle.Current.MainText);
            integerField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            integerField.style.height = 18;
            if (label != "")
            {
                integerField.label = label;
                integerField[0].style.SetIS_Style(DocStyle.Current.MainText);
                integerField[0].style.minWidth = DocStyle.Current.LabelWidth;
            }
            if (valueChange != null)
                integerField.RegisterValueChangedCallback(valueChange);
            return integerField;
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