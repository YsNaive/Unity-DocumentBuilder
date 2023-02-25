using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace DocumentBuilder
{
    public static class DocComponentLayout
    {
        #region Document
        public static DocComponent DocComponentField(DocComponent docComponent, bool isEditMode = false)
        {
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 16;
            labelStyle.normal.textColor = EditorGUITool.ColorSet.Default;
            GUIStyle tempStyle = new GUIStyle();
            tempStyle.alignment = TextAnchor.MiddleLeft;
            tempStyle.normal.textColor = EditorGUITool.ColorSet.Default;

            if (isEditMode)//------------------------------- Edit ------------------------------------------//
            {
                DocComponentType newType = (DocComponentType)EditorGUILayout.EnumPopup("Type ", docComponent.ComponentType);
                if (docComponent.ComponentType != newType)
                    docComponent = new DocComponent(newType);

                if (DocComponentType.Description == docComponent.ComponentType)
                {
                    docComponent.Text[0] = EditorGUILayout.TextArea(docComponent.Text[0]);
                }
                else if (DocComponentType.NameAndUsage == docComponent.ComponentType)
                {
                    docComponent.Text[0] = EditorGUILayout.TextField("Label", docComponent.Text[0]);
                    for (int i = 1; i < docComponent.Text.Count; i++)
                    {
                        string[] prop = docComponent.Text[i].Split("\n");
                        EditorGUITool.HorizontalGroup(() =>
                        {
                            EditorGUILayout.LabelField("Name", GUILayout.Width(60));
                            prop[0] = EditorGUILayout.TextField(prop[0]);
                            EditorGUILayout.LabelField("Usage", GUILayout.Width(60));
                            prop[1] = EditorGUILayout.TextField(prop[1]);
                            docComponent.Text[i] = prop[0] + "\n" + prop[1];
                            if (GUILayout.Button("X", GUILayout.Width(50)))
                            {
                                docComponent.Text.RemoveAt(i--);
                            }
                        });
                    }

                    if (GUILayout.Button("add", GUILayout.Width(50)))
                    {
                        docComponent.Text.Add("\n");
                    }
                }
                else if (DocComponentType.FuncDisplay == docComponent.ComponentType)
                {
                    docComponent.Text[0] = EditorGUILayout.TextField("Return Type", docComponent.Text[0]);
                    docComponent.Text[1] = EditorGUILayout.TextField("Func Name", docComponent.Text[1]);
                    for (int i = 2; i < docComponent.Text.Count - 1; i++)
                    {
                        string[] prop = docComponent.Text[i].Split("\n");
                        EditorGUITool.HorizontalGroup(() =>
                        {
                            EditorGUILayout.LabelField("Type", GUILayout.Width(60));
                            prop[0] = EditorGUILayout.TextField(prop[0]);
                            EditorGUILayout.LabelField("Name", GUILayout.Width(60));
                            prop[1] = EditorGUILayout.TextField(prop[1]);
                            docComponent.Text[i] = prop[0] + "\n" + prop[1];
                            if (GUILayout.Button("X", GUILayout.Width(50)))
                            {
                                docComponent.Text.RemoveAt(i--);
                            }
                        });
                    }
                    if (GUILayout.Button("add parameter", GUILayout.Width(120)))
                    {
                        docComponent.Text.Insert(docComponent.Text.Count - 1, "T\ni");
                    }
                    docComponent.Text[docComponent.Text.Count - 1] = EditorGUILayout.TextArea(docComponent.Text[docComponent.Text.Count - 1]);
                }
                else if (DocComponentType.Picture == docComponent.ComponentType)
                {
                    float scale = EditorGUILayout.FloatField("Scale", float.Parse(docComponent.Text[0], numberFormat));
                    if (scale < 0) scale = 0;
                    docComponent.Text[0] = scale.ToString();
                    docComponent.Picture = (Texture2D)EditorGUILayout.ObjectField("Picture", docComponent.Picture, typeof(Texture2D), false, GUILayout.Height(18));
                    Vector2 imageSize = new Vector2(docComponent.Picture.width, docComponent.Picture.height);
                    scale = (float)(imageSize.y / imageSize.x);
                    imageSize.x *= float.Parse(docComponent.Text[0], numberFormat);
                    imageSize.y = imageSize.x * scale;
                    Rect rect = GUILayoutUtility.GetRect(imageSize.x, imageSize.y + 8);
                    rect.width = imageSize.x;
                    rect.height = imageSize.y;
                    rect.x += (EditorGUI.indentLevel + 1) * 15f;
                    rect.y += 4;
                    GUI.DrawTexture(rect, docComponent.Picture);
                }
                else if (DocComponentType.Label == docComponent.ComponentType)
                {
                    docComponent.Text[0] = EditorGUILayout.TextField(docComponent.Text[0]);
                }
                else if (DocComponentType.Matrix == docComponent.ComponentType)
                {
                    for (int row = 0; row < docComponent.Text.Count; row++)
                    {
                        string[] contents = docComponent.Text[row].Split("%column%");
                        docComponent.Text[row] = "";
                        EditorGUITool.HorizontalGroup(() =>
                        {
                            for (int col = 0; col < contents.Length; col++)
                            {
                                contents[col] = EditorGUILayout.TextField(contents[col]);
                                docComponent.Text[row] += contents[col];
                                if (col != contents.Length - 1)
                                    docComponent.Text[row] += "%column%";
                            }
                            if (docComponent.Text.Count > 1)
                            {
                                if (GUILayout.Button("-", GUILayout.Width(20)))
                                {
                                    docComponent.Text.RemoveAt(row);
                                    row--;
                                }
                            }
                            else
                                GUILayout.Label("", GUILayout.Width(20));
                            GUILayout.Label("", GUILayout.Width(20));
                        });
                    }
                    int col = docComponent.Text[0].Split("%column%").Length;
                    if (col > 1)
                    {
                        EditorGUITool.HorizontalGroup(() =>
                        {
                            for (int i = 0; i < col; i++)
                            {
                                if (GUILayout.Button("-"))
                                {
                                    for (int row = 0; row < docComponent.Text.Count; row++)
                                    {
                                        string[] contents = docComponent.Text[row].Split("%column%");
                                        docComponent.Text[row] = "";
                                        bool isFirst = true;
                                        for (int j = 0; j < col; j++)
                                        {
                                            if (j != i)
                                            {
                                                if (isFirst)
                                                    isFirst = false;
                                                else
                                                    docComponent.Text[row] += "%column%";

                                                docComponent.Text[row] += contents[j];
                                            }
                                        }
                                    }
                                    col--;
                                    i--;
                                }
                            }
                            GUILayout.Label("", GUILayout.Width(20));
                            if (GUILayout.Button("▶", GUILayout.Width(20)))
                            {
                                for (int i = 0; i < docComponent.Text.Count; i++)
                                {
                                    docComponent.Text[i] += "%column%";
                                }
                            }
                        });
                    }
                    else
                    {
                        EditorGUITool.HorizontalGroup(() =>
                        {
                            for (int i = 0; i < col; i++)
                            {
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.Label("", GUILayout.Width(20));
                            if (GUILayout.Button("▶", GUILayout.Width(20)))
                            {
                                for (int i = 0; i < docComponent.Text.Count; i++)
                                {
                                    docComponent.Text[i] += "%column%";
                                }
                            }
                        });
                    }
                    EditorGUITool.HorizontalGroup(() =>
                    {
                        for (int i = 0; i < col; i++)
                        {
                            GUILayout.FlexibleSpace();
                        }
                        if (GUILayout.Button("▼", GUILayout.Width(20)))
                        {
                            string tempStr = "";
                            int col = docComponent.Text[0].Split("%column%").Length;
                            for (int i = 1; i < col; i++)
                            {
                                tempStr += "%column%";
                            }
                            docComponent.Text.Add(tempStr);
                        }
                        GUILayout.Label("", GUILayout.Width(20));
                    });
                }
                else if (DocComponentType.DividerLine == docComponent.ComponentType)
                {
                    docComponent.Text[0] = EditorGUILayout.TextField("Title", docComponent.Text[0]);
                }
            }
            else            //------------------------------- Display ------------------------------------------//
            {
                if (DocComponentType.Description == docComponent.ComponentType)
                {
                    EditorGUI.indentLevel++;
                    EditorStyles.label.wordWrap = true;
                    Vector2 size = GUI.skin.label.CalcSize(new GUIContent(docComponent.Text[0]));
                    EditorGUILayout.LabelField(docComponent.Text[0], GUILayout.Width(size.x), GUILayout.Height(size.y));
                    EditorStyles.label.wordWrap = false;
                    EditorGUI.indentLevel--;
                }
                else if (DocComponentType.NameAndUsage == docComponent.ComponentType)
                {
                    EditorGUILayout.LabelField(docComponent.Text[0], labelStyle, GUILayout.Height(25));
                    for (int i = 1; i < docComponent.Text.Count; i++)
                    {
                        string[] prop = docComponent.Text[i].Split("\n");
                        EditorGUITool.HorizontalGroup(() =>
                        {
                            EditorGUI.indentLevel = 0;
                            EditorGUILayout.LabelField(" " + prop[0], tempStyle, GUILayout.Width(120));
                            Rect tempRect = GUILayoutUtility.GetLastRect();
                            tempRect.y -= 1f;
                            tempRect.height += 3;
                            EditorGUITool.DrawRectangle(tempRect, EditorGUITool.ColorSet.LightGray, 1);
                            EditorGUILayout.LabelField(" " + prop[1], tempStyle);
                            tempRect = GUILayoutUtility.GetLastRect();
                            tempRect.x -= 4f;
                            tempRect.y -= 1f;
                            tempRect.height += 3;
                            EditorGUITool.DrawRectangle(tempRect, EditorGUITool.ColorSet.LightGray, 1);
                        });
                    }
                    EditorGUILayout.Space(15);
                }
                else if (DocComponentType.FuncDisplay == docComponent.ComponentType)
                {
                    EditorGUITool.HorizontalGroup(() =>
                    {
                        tempStyle.normal.textColor = new Color(.045f, .58f, .63f);
                        EditorGUILayout.LabelField(docComponent.Text[0], tempStyle, GUILayout.Height(18), GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(docComponent.Text[0])).x));
                        tempStyle.normal.textColor = new Color(.8f, .78f, .4f);
                        EditorGUILayout.LabelField(docComponent.Text[1], tempStyle, GUILayout.Height(18), GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(docComponent.Text[1])).x));
                        tempStyle.normal.textColor = EditorGUITool.ColorSet.Default;
                        EditorGUILayout.LabelField("(", tempStyle, GUILayout.Height(18), GUILayout.Width(5));
                        for (int i = 2; i < docComponent.Text.Count - 1; i++)
                        {
                            string[] func = docComponent.Text[i].Split("\n");
                            if (i != 2)
                            {
                                tempStyle.normal.textColor = EditorGUITool.ColorSet.Default;
                                EditorGUILayout.LabelField(",", tempStyle, GUILayout.Height(18), GUILayout.Width(5));
                            }
                            tempStyle.normal.textColor = new Color(.045f, .58f, .63f);
                            EditorGUILayout.LabelField(func[0], tempStyle, GUILayout.Height(18), GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(func[0])).x - 2));
                            tempStyle.normal.textColor = new Color(.55f, .8f, 1f);
                            EditorGUILayout.LabelField(func[1], tempStyle, GUILayout.Height(18), GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(func[1])).x - 7));
                        }
                        tempStyle.normal.textColor = EditorGUITool.ColorSet.Default;
                        EditorGUILayout.LabelField(" )", tempStyle, GUILayout.Height(18), GUILayout.Width(10));
                    });
                    EditorGUI.indentLevel += 2;
                    if (docComponent.Text[docComponent.Text.Count - 1] != "")
                        foreach (var str in docComponent.Text[docComponent.Text.Count - 1].Split("\n"))
                            EditorGUILayout.LabelField(str, GUILayout.Height(14));
                    EditorGUI.indentLevel -= 2;
                }
                else if (DocComponentType.Picture == docComponent.ComponentType)
                {
                    if (docComponent.Picture != null)
                    {
                        Vector2 imageSize = new Vector2(docComponent.Picture.width, docComponent.Picture.height);
                        float scale = (float)(imageSize.y / imageSize.x);
                        imageSize.x *= float.Parse(docComponent.Text[0], numberFormat);
                        imageSize.y = imageSize.x * scale;
                        Rect rect = GUILayoutUtility.GetRect(imageSize.x, imageSize.y + 8);
                        rect.width = imageSize.x;
                        rect.height = imageSize.y;
                        rect.x += (EditorGUI.indentLevel + 1) * 15f;
                        rect.y += 4;
                        GUI.DrawTexture(rect, docComponent.Picture);
                    }
                }
                else if (DocComponentType.Label == docComponent.ComponentType)
                {
                    EditorGUILayout.LabelField(docComponent.Text[0], labelStyle);
                    EditorGUILayout.LabelField("", GUILayout.Height(2));
                }
                else if (DocComponentType.Matrix == docComponent.ComponentType)
                {
                    int row = docComponent.Text.Count, col = docComponent.Text[0].Split("%column%").Length;
                    float[] height = new float[row];
                    float[] width = new float[col];
                    for (int i = 0; i < docComponent.Text.Count; i++)
                    {
                        string[] contents = docComponent.Text[i].Split("%column%");
                        for (int j = 0; j < contents.Length; j++)
                        {
                            string[] lines = contents[j].Split("\\n");
                            for (int l = 0; l < lines.Length; l++)
                            {
                                float thisWidth = GUI.skin.label.CalcSize(new GUIContent(lines[l])).x;
                                if (width[j] < thisWidth + 15)
                                    width[j] = thisWidth + 15;
                            }
                            if (height[i] < 22 + (14 * (lines.Length - 1)))
                                height[i] = 22 + (14 * (lines.Length - 1));
                        }
                    }
                    Rect rowRect = EditorGUILayout.GetControlRect(false,0);
                    rowRect.y = rowRect.yMax + 5;
                    rowRect.height = 20;
                    for (int i = 0; i < docComponent.Text.Count; i++)
                    {
                        rowRect.x = 40;
                        string[] contents = docComponent.Text[i].Split("%column%");
                        Rect colRect = rowRect;
                        for (int j = 0; j < contents.Length; j++)
                        {
                            colRect.y = rowRect.y;
                            colRect.width = width[j];
                            string[] lines = contents[j].Split("\\n");
                            colRect.y += (height[i] - 18 * lines.Length) / 2f;

                            for (int l = 0; l < lines.Length; l++)
                            {
                                GUI.Label(colRect, lines[l]);
                                EditorGUITool.DrawRectangle(new Rect(colRect.x -5f, rowRect.y + 0.5f, width[j], height[i]), EditorGUITool.ColorSet.LightGray, .5f);
                                colRect.y += 14;
                            }
                            colRect.x += width[j];
                        }
                        rowRect.y += height[i];
                        GUILayoutUtility.GetRect(0,0, GUILayout.Height(height[i]+3), GUILayout.Width(colRect.x + 18));
                    }
                }
                else if (DocComponentType.DividerLine == docComponent.ComponentType)
                {
                    EditorGUILayout.LabelField("", GUILayout.Height(10));
                    EditorGUITool.DividerLine(docComponent.Text[0] != "" ? $"{docComponent.Text[0]}         " : "", EditorGUITool.ColorSet.DarkGray, EditorGUITool.ColorSet.Default, 2.5f);
                    EditorGUILayout.LabelField("", GUILayout.Height(5));
                }
            }

            return docComponent;
        }
        #endregion
    }

}
