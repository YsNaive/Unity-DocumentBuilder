using System;
using UnityEditor;
using UnityEngine;

namespace DocumentBuilder
{
    public static class EditorGUITool
    {

        public static void ColorRegion(bool isSuccess, Action action) { ColorRegion(isSuccess ? ColorSet.Success : ColorSet.Danger, action); }
        public static void ColorRegion(UnityEngine.Color color, Action action)
        {
            var temp = GUI.color;
            GUI.color = color;
            action?.Invoke();
            GUI.color = temp;
        }
        public static Rect HorizontalGroup(Action action, params GUILayoutOption[] options)
        {
            int indentLevel = EditorGUI.indentLevel;
            float labelWidth = EditorGUIUtility.labelWidth;

            var output =
            EditorGUILayout.BeginHorizontal(options);
            EditorGUILayout.LabelField("", GUILayout.Width(indentLevel * 15f), GUILayout.Height(15));
            action?.Invoke();
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
            return output;
        }
        public static Rect VerticalGroup(Action action, params GUILayoutOption[] options)
        {
            int indentLevel = EditorGUI.indentLevel;
            float labelWidth = EditorGUIUtility.labelWidth;

            var output = EditorGUILayout.BeginVertical(options);
            action?.Invoke();
            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel = indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
            return output;
        }
        public static void DisableGroup(bool isDisable, Action action)
        {
            EditorGUI.BeginDisabledGroup(isDisable);
            action?.Invoke();
            EditorGUI.EndDisabledGroup();
        }
        public static Rect DividerLine(string label = "", float height = 5, float xBleed = 5) { return DividerLine(label, UnityEngine.Color.gray, UnityEngine.Color.white, height, xBleed); }
        public static Rect DividerLine(string label, UnityEngine.Color color, float height = 5, float xBleed = 5) { return DividerLine(label, color, color, height, xBleed); }
        public static Rect DividerLine(string label, UnityEngine.Color lineColor, UnityEngine.Color textColor, float height = 5, float xBleed = 5)
        {
            float labelWidth;
            if (label != "")
                labelWidth = GUI.skin.label.CalcSize(new GUIContent(label)).x;
            else
                labelWidth = 0;

            Rect position = GUILayoutUtility.GetRect(0, height + 10);

            if (labelWidth == 0)
            {
                EditorGUI.DrawRect(new Rect(position.x + xBleed, (float)(position.y + (position.height * 0.5f) - height * 0.4), position.width - (xBleed * 2), height), lineColor);
            }
            else
            {
                Color defaultColor = GUI.color;
                float lineWidth = ((position.width - labelWidth) * 0.5f) - xBleed;
                Rect rect = new Rect(position.x + xBleed, (float)(position.y + (position.height * 0.5f) - height * 0.4), lineWidth - 2, height);
                EditorGUI.DrawRect(rect, lineColor);
                rect.x += lineWidth;
                rect.y += (rect.height * 0.5f) - 7;
                rect.height = 14;
                rect.width = labelWidth;
                GUI.color = textColor;
                EditorGUI.LabelField(rect, label);
                GUI.color = defaultColor;
                rect.height = height;
                rect.y = (float)(position.y + (position.height * 0.5f) - height * 0.4);
                rect.x += labelWidth;
                rect.width = lineWidth;
                EditorGUI.DrawRect(rect, lineColor);
            }

            return position;
        }

        public static bool TextureButton(Rect position, Texture2D texture)
        {
            GUI.DrawTexture(position, texture);
            return GUI.Button(position, texture, GUIStyle.none);
        }
        public static void DrawRectangle(Rect rect, UnityEngine.Color color, float thickness)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
        }

        static Texture2D connect;
        static Texture2D disConnect;
        static Texture2D upArrow;
        static Texture2D rightArrow;
        static Texture2D downArrow;
        static Texture2D leftArrow;
        static Texture2D copy;
        static Texture2D paste;
        static Texture2D duplicate;
        static Texture2D delete;
        public static class Icon
        {
            public static Texture2D Connect
            {
                get
                {
                    if (connect == null)
                    {
                        connect = new Texture2D(16, 16);
                        connect.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAFRJREFUOI1jYBgFjDDG8+fP/6NLSkpKMhKSY8KlgBCA6WHBZTIuMXTLWIhVCJOTlJRkRJZjItXp6AAjkLB5ARmgq8UIA2yG4QtkJmJsxQbI0TNcAQAkmSxhaHmhNgAAAABJRU5ErkJggg=="));
                        connect.filterMode = FilterMode.Point;
                        connect.Apply();
                    }
                    return connect;
                }
            }
            public static Texture2D DisConnect
            {
                get
                {
                    if (disConnect == null)
                    {
                        disConnect = new Texture2D(16, 16);
                        disConnect.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAGBJREFUOI3lUcERwCAIE88lsv9wGcN+MQWsX5sfkIQjtPYPkJwkZzQbnqRDAJYJF4NMHNWeC8BGJVTjaFGvtmofgCmnR4ITLAY+7Sw87ZcZ+DozDG/WpL986WWw+/9FeAAN/ULoi7lzhQAAAABJRU5ErkJggg=="));
                        disConnect.filterMode = FilterMode.Point;
                        disConnect.Apply();
                    }
                    return disConnect;
                }
            }
            public static Texture2D UpArrow
            {
                get
                {
                    if (upArrow == null)
                    {
                        upArrow = new Texture2D(24, 24);
                        upArrow.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAHBJREFUSIntjbEVgCAMRE92oMz+g6VkCKz0IUI4wMIiV0Fy+R/weHZzsEVVzeVfRKhbqlTDZyTDQglPKQEAYoy0xFy24FdYSXdhwWckzSEDZyWvwQyckTw+K/CR5H7swC1JqEur8N5tGBW+kHg8P88J23pEEos3tVYAAAAASUVORK5CYII="));
                        upArrow.filterMode = FilterMode.Point;
                        upArrow.Apply();
                    }
                    return upArrow;
                }
            }
            public static Texture2D RightArrow
            {
                get
                {
                    if (rightArrow == null)
                    {
                        rightArrow = new Texture2D(24, 24);
                        rightArrow.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAIpJREFUWIXt1rENgDAMRNGAGMETejBPmB1CBaJASWzfdf4dCOWeBAWtVVU1ycyGmQ3mxrECfK9Vdfo8HcCAhABIyBZARN57vXcoxA1AQ8IAFCQNyEJggCgEDvBCTvfJ4C70gd5XAANEP8IU4G90dzgFQAyHAMhhF4Ax7AIwhkMAxg/JFoAxXFXV0w2D/VbZkwL6+QAAAABJRU5ErkJggg=="));
                        rightArrow.filterMode = FilterMode.Point;
                        rightArrow.Apply();
                    }
                    return rightArrow;
                }
            }
            public static Texture2D DownArrow
            {
                get
                {
                    if (downArrow == null)
                    {
                        downArrow = new Texture2D(24, 24);
                        downArrow.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAALFJREFUWIXtllsKhTAMBY/iErrCLiwr7B70S8kV2+akBb2Q+RJ8dCYoFgiC4GWW2gkR2WculHN+XGuduYiH6gSA3ymklKgHl1Ku41o98IEJNAW0uS5iaNV3Bbwwsl2BkSn06k0CLKykScAzBUu9WcCK50U1C1iL2GtdE3gq9X6mlICljKmnBTS62FvvEmgVsvUAsLE3aEbKT2jjk/t+wVMPfOBvOISI7KM7p/+eQBDM4ACUvEEYzcAEAgAAAABJRU5ErkJggg=="));
                        downArrow.filterMode = FilterMode.Point;
                        downArrow.Apply();
                    }
                    return downArrow;
                }
            }
            public static Texture2D LeftArrow
            {
                get
                {
                    if (leftArrow == null)
                    {
                        leftArrow = new Texture2D(24, 24);
                        leftArrow.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAIBJREFUSInV1bkRgDAMRFHD0IaaUP/dKFQRJiJEx64SOyKA/4YxyGudvi70QTPb37WI/HZuNp6th4m7e3p/6w268RaAxMsAGi8BTDwF2HgITMRDYCKeAmw8BCbiIaCq8JwqAVNIusksUvqTGaQ8i1CkNU0RpH3gdBHoRJv6hM9YL4OnQazyM36TAAAAAElFTkSuQmCC"));
                        leftArrow.filterMode = FilterMode.Point;
                        leftArrow.Apply();
                    }
                    return leftArrow;
                }
            }
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
        public static class ColorSet
        {
            public static UnityEngine.Color Success { get => new UnityEngine.Color(0.7f, 1f, 0.7f, 1f); }
            public static UnityEngine.Color Warning { get => new UnityEngine.Color(1f, 1f, 0.7f, 1f); }
            public static UnityEngine.Color Danger { get => new UnityEngine.Color(1f, 0.55f, 0.55f, 1f); }
            public static UnityEngine.Color Information { get => new UnityEngine.Color(0.7f, 1f, 1f, 1f); }
            public static UnityEngine.Color Default { get => new UnityEngine.Color(.85f, .85f, .85f, 1f); }
            public static UnityEngine.Color LightGray { get => new UnityEngine.Color(0.65f, 0.68f, 0.68f); }
            public static UnityEngine.Color DarkGray { get => new UnityEngine.Color(0.35f, 0.35f, 0.35f); }
        }
    }
}
