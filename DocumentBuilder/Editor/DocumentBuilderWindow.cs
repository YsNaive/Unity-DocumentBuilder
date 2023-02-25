using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using static DocumentBuilder.EditorGUITool;
namespace DocumentBuilder
{
    public class DocumentBuilderWindow : EditorWindow
    {
        #region MenuItem
        public static DocumentBuilderWindow Instance { get; private set; }
        [MenuItem("Tools/NaiveAPI/Document Window")]
        public static void ShowWindow()
        {
            Data.Save();
            if(Instance != null)
                Instance.titleContent = new GUIContent("Document Window");
            GetWindow<DocumentBuilderWindow>("Document Window");
        }
        public static void ShowWindow(SODocInformation root, string windowName = "Document")
        {
            Data.BookRoot = root;
            Data.Save();
            if (Instance != null)
                Instance.titleContent = new GUIContent(windowName);
            GetWindow<DocumentBuilderWindow>(windowName);
        }
        #endregion

        public GUIStyle menuStyle = new GUIStyle();
        private Texture2D selectBookIcon;
        private Texture2D searchIcon;
        private SODocInformation searchDocument; // use to display search result
        private bool isEditMode = false;
        private bool isOpenMenu = true;
        private bool hotkeyState = false;
        private Editor editingDocument;
        private Color littleTitleColor = ColorSet.Information;
        private void OnEnable()
        {
            menuStyle.normal.textColor = ColorSet.Default;
            minSize = new Vector2(500, 100);
            Instance = this;
            littleTitleColor.a = .65f;
            Data.Load();
            selectBookIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/Gear.png");
            searchIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DefaultMenuIcon/Search.png");
            searchDocument = AssetDatabase.LoadAssetAtPath<SODocInformation>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/Search.asset");

            if (isEditMode)
                editingDocument = Editor.CreateEditor(Data.SelectingDocInfo);
        }

        bool isEditingMenuWidth = false;
        bool isDirty = false;
        bool isSelectingBook;
        string searchMsg = "";
        Vector2 menuViewPosition, infoViewPosition;
        private void OnGUI()
        {
            isDirty = false;
            if (Data.SelectingDocInfo == null)
                Data.Load();
            #region MenuWidth modify
            if (Event.current.type == EventType.MouseDown)
                if (Event.current.button == 0)
                    if (Mathf.Abs(Event.current.mousePosition.x - Data.MenuWidth) < 5)
                        isEditingMenuWidth = true;
            if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseLeaveWindow)
                if (Event.current.button == 0)
                    isEditingMenuWidth = false;
            if (isEditingMenuWidth)
            {
                Data.MenuWidth = Event.current.mousePosition.x;
                if (position.width - Data.MenuWidth < 100) Data.MenuWidth = position.width - 100;
                if (Data.MenuWidth < 100) Data.MenuWidth = 100;
                isDirty = true;
            }
            #endregion

            #region Edit Mode
            if (GUI.Button(new Rect(position.width - 60, 5, 45, 20), isEditMode ? "Edit" : "View") ||
                (Event.current.control && Event.current.keyCode == KeyCode.E && !hotkeyState))
            {
                isEditMode = !isEditMode;
                if (isEditMode)
                    editingDocument = Editor.CreateEditor(Data.SelectingDocInfo);
                hotkeyState = true;
                return;
            }
            if (Event.current.type == EventType.KeyUp)
                hotkeyState = false;
            #endregion

            #region Menu and Information Layout
            Rect menuRect = new Rect();
            // Draw Menu and Document
            HorizontalGroup(() =>
            {
                #region Menu
                if (isOpenMenu)
                {
                    VerticalGroup(() =>
                    {
                        EditorGUI.DrawRect(new Rect(0, 0, Data.MenuWidth, position.height), new Color(0.19f, 0.19f, 0.19f)); // background

                            // search
                            GUI.DrawTexture(new Rect(13, 6, 19, 19), searchIcon);
                        EditorGUI.BeginChangeCheck();
                        searchMsg = EditorGUI.TextField(new Rect(35, 9, Data.MenuWidth - 60, 15), searchMsg);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (searchMsg != "")
                                searchDoc(Data.BookRoot);
                            else
                                searchDocument.SubPages.Clear();
                        }
                        if (TextureButton(new Rect(Data.MenuWidth - 21, 8, 16, 16), Icon.LeftArrow))
                        {
                            isOpenMenu = false;
                        }
                        EditorGUILayout.LabelField("", GUILayout.Height(30), GUILayout.Width(Data.MenuWidth));
                            // menu layout
                            menuViewPosition =
                        EditorGUILayout.BeginScrollView(menuViewPosition, GUILayout.Width(Data.MenuWidth - 8));
                        menuRect = drawMenu(new Rect(0, 0, Data.MenuWidth, Data.MenuUnitHeight), searchMsg == "" ? Data.BookRoot : searchDocument);
                        EditorGUILayout.LabelField("", GUILayout.Width(menuRect.width - 18), GUILayout.Height(menuRect.yMax + 35));
                        EditorGUILayout.EndScrollView();
                    }, GUILayout.Width(Data.MenuWidth));
                }
                else
                {
                    EditorGUI.DrawRect(new Rect(0, 0, 30, position.height), new Color(0.19f, 0.19f, 0.19f)); // background
                        EditorGUILayout.LabelField("", GUILayout.Width(30));
                    if (TextureButton(new Rect(5, 6, 20, 20), Icon.RightArrow))
                    {
                        isOpenMenu = true;
                    }
                }
                #endregion

                #region DocComponents
                VerticalGroup(() =>
                {
                    if( isEditMode)
                    {
                        HorizontalGroup(() =>
                        {
                            EditorGUILayout.LabelField("- Editing Target ", GUILayout.Width(105));
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.ObjectField(Data.SelectingDocInfo, typeof(SODocInformation), false, GUILayout.Width(350));
                            EditorGUI.EndDisabledGroup();
                        });
                        infoViewPosition = EditorGUILayout.BeginScrollView(infoViewPosition);
                        HorizontalGroup(() =>
                        {
                            GUILayout.Space(5);
                            VerticalGroup(() =>
                            {
                                editingDocument.OnInspectorGUI();
                            });
                        });
                        EditorGUILayout.EndScrollView();
                    }
                    else
                    {
                        ColorRegion(littleTitleColor, () =>
                        {
                            EditorGUI.indentLevel = 0;
                            EditorGUILayout.LabelField(Data.SelectingDocInfo.Name);
                        });

                        infoViewPosition = EditorGUILayout.BeginScrollView(infoViewPosition);
                        drawDocInformation();
                        GUILayoutUtility.GetRect(0, 120);
                        EditorGUILayout.EndScrollView();
                    }
                });
                #endregion
            });

            // Draw Select Menu          
            if (isSelectingBook)
            {
                Data.MenuDisplaySettings.Clear();
                Rect rect = new Rect(3, position.height - 23, 20, 20);
                EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(.1f, .1f, .1f, .7f));
                rect.x += 20;
                rect.y -= 20;
                rect.height = 16;
                rect.width = Data.MenuWidth;
                foreach (var doc in SODocumentBuilderSetting.Get.DocBookList)
                {
                    if (drawMenuUnit(rect, doc, new MenuDisplaySetting(), false))
                    {
                        Data.BookRoot = doc;
                        Data.SelectingDocInfo = doc;
                        editingDocument = Editor.CreateEditor(doc);
                        isSelectingBook = false;
                    }
                    rect.y -= 16;
                }

                rect = new Rect(40, position.height - 23, 150, 18);
                bool isContains = SODocumentBuilderSetting.Get.DocBookList.Contains(Data.SelectingDocInfo);
                if (GUI.Button(rect, isContains? "Remove shortcut" : "Add shortcut")) {
                    if (isContains)
                        SODocumentBuilderSetting.Get.DocBookList.Remove(Data.SelectingDocInfo);
                    else
                        SODocumentBuilderSetting.Get.DocBookList.Add(Data.SelectingDocInfo);
                    EditorUtility.SetDirty(SODocumentBuilderSetting.Get);
                }
            }
            if (TextureButton(new Rect(3, position.height - 23, 20, 20), selectBookIcon))
            {
                isSelectingBook = !isSelectingBook;
                isDirty = true;
            }

            #endregion
            Repaint();
            // Save data if isDirty
            if (isDirty)
            {
                Repaint();
                Data.Save();
            }
        }

        // will write result into searchDocument
        private void searchDoc(SODocInformation information)
        {
            if (information == null)
                return;
            if (information == Data.BookRoot)
            {
                searchDocument.SubPages.Clear();
            }
            else
            {
                if (information.name.Contains(searchMsg, System.StringComparison.OrdinalIgnoreCase))
                    searchDocument.SubPages.Add(information);
            }

            foreach (var child in information.SubPages)
                searchDoc(child);
        }

        /// <summary>
        /// You should set drawingMenuPosition's value before call this func.
        /// </summary>
        private Rect drawMenu(Rect position, SODocInformation docInformation)
        {
            if (docInformation == null)
                return position;
            if (!Data.MenuDisplaySettings.ContainsKey(docInformation))
            {
                Data.MenuDisplaySettings.Add(docInformation, new MenuDisplaySetting());
                isDirty = true;
            }
            MenuDisplaySetting displaySetting = Data.MenuDisplaySettings[docInformation];

            // draw self
            if (drawMenuUnit(position, docInformation, displaySetting))
            {
                Data.SelectingDocInfo = docInformation;
                if (isEditMode)
                    editingDocument = Editor.CreateEditor(docInformation);
                infoViewPosition = Vector2.zero;
                isDirty = true;
            }
            float width = (50 + GUI.skin.label.CalcSize(new GUIContent(docInformation.name)).x + position.x);
            if (position.width < width)
                position.width = width;
            position.y += Data.MenuUnitHeight;

            //call children recursive
            if (displaySetting.IsShowChildren)
            {
                position.x += Data.MenuUnitHeight;
                foreach (var child in docInformation.SubPages)
                {
                    position = drawMenu(position, child);
                }
                position.x -= Data.MenuUnitHeight;
            }
            return position;
        }

        /// <summary>
        /// return if MenuUnit been clicked
        /// </summary>
        private bool drawMenuUnit(Rect position, SODocInformation docInformation, MenuDisplaySetting displaySetting, bool drawAdditionalButton = true)
        {
            if (docInformation == null)
            {
                return false;
            }
            if (docInformation == Data.SelectingDocInfo && drawAdditionalButton)
            {
                Rect rect = position;
                rect.width = Data.MenuWidth - Data.MenuUnitHeight - 10;
                rect.x = Data.MenuUnitHeight;
                EditorGUI.DrawRect(rect, new Color(.7f, 1f, .7f, .1f));
            }

            Rect iconPos = position;
            iconPos.height -= 2;
            iconPos.width = iconPos.height;
            iconPos.x += 1;
            iconPos.y += 1;
            // draw arrow
            if (drawAdditionalButton)
            {
                if (docInformation == Data.BookRoot || docInformation == searchDocument)
                    displaySetting.IsShowChildren = true;
                else if (docInformation.SubPages.Count > 0)
                    if (TextureButton(iconPos, displaySetting.IsShowChildren ? Icon.DownArrow : Icon.RightArrow))
                        displaySetting.IsShowChildren = !displaySetting.IsShowChildren;
            }
            // draw type icon
            iconPos.x += Data.MenuUnitHeight;
            GUI.DrawTexture(iconPos, docInformation.MenuIcon ? docInformation.MenuIcon : new Texture2D(1, 1));

            position.x += Data.MenuUnitHeight * 2;
            position.width -= Data.MenuUnitHeight * 2;

            // draw display set
            if (docInformation == Data.SelectingDocInfo && docInformation.SubPages.Count > 0 && drawAdditionalButton)
            {
                iconPos = position;
                iconPos.height -= 2;
                iconPos.width = iconPos.height;
                iconPos.x = 0;
                iconPos.y += 1;
                menuStyle.fontStyle = FontStyle.Bold;

                if (displaySetting.IsShowChildren)
                {
                    menuStyle.normal.textColor = new Color(.75f, .6f, .6f);
                    EditorGUI.DrawRect(iconPos, new Color(.36f, .28f, .28f));
                    iconPos.x += 4;
                    iconPos.y -= 2;
                    if (GUI.Button(iconPos, "-", menuStyle))
                    {
                        MenuDisplaySetting setting = new MenuDisplaySetting();
                        setting.IsShowChildren = false;
                        setDisplaySetting(docInformation, setting);
                    }
                }
                else
                {
                    menuStyle.normal.textColor = new Color(.6f, .75f, .6f);
                    EditorGUI.DrawRect(iconPos, new Color(.28f, .36f, .28f));
                    iconPos.x += 2;
                    iconPos.y -= 2;
                    if (GUI.Button(iconPos, "+", menuStyle))
                    {
                        MenuDisplaySetting setting = new MenuDisplaySetting();
                        setting.IsShowChildren = true;
                        setDisplaySetting(docInformation, setting);
                    }
                }


                menuStyle.normal.textColor = ColorSet.Default;
                menuStyle.fontStyle = FontStyle.Normal;
            }

            // return result and draw button
            return GUI.Button(position, docInformation.name, menuStyle);
        }

        /// <summary>
        /// It draw Data.SelectingDocInfo given by drawMenuUnit()
        /// </summary>
        private void drawDocInformation()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Space(5);
            foreach (var doc in Data.SelectingDocInfo.Components)
            {
                DocComponentLayout.DocComponentField(doc);
            }
        }

        private void setDisplaySetting(SODocInformation target, MenuDisplaySetting setting, bool isSetChildren = true)
        {
            if (target == null) return;
            if (!Data.MenuDisplaySettings.ContainsKey(target))
                Data.MenuDisplaySettings.Add(target, setting.Copy());
            else
            {
                Data.MenuDisplaySettings[target].IsShowChildren = setting.IsShowChildren;
            }

            if (isSetChildren)
            {
                foreach (var child in target.SubPages)
                    setDisplaySetting(child, setting);
            }
        }
        [System.Serializable]
        static class Data
        {
            public static float MenuWidth = 240;
            public static float MenuUnitHeight = 16;
            public static SODocInformation BookRoot = null;
            public static SODocInformation SelectingDocInfo = null;
            public static Dictionary<SODocInformation, MenuDisplaySetting> MenuDisplaySettings = new Dictionary<SODocInformation, MenuDisplaySetting>();

            public static void Load()
            {
                if (!File.Exists(Application.temporaryCachePath + "\\DocumentWindowData.txt"))
                {
                    Save();
                }

                string[] datas = File.ReadAllText(Application.temporaryCachePath + "\\DocumentWindowData.txt").Split("%DATA%");
                MenuWidth = float.Parse(datas[0]);
                MenuUnitHeight = float.Parse(datas[1]);
                BookRoot = AssetDatabase.LoadAssetAtPath<SODocInformation>(datas[2]);
                if (BookRoot == null) // if root not found us NaiveAPI Document as Default
                    BookRoot = (SODocInformation)AssetDatabase.LoadAssetAtPath(DocumentBuilderData.Path.DocumentBuilderRoot + "\\Editor\\DocAsset\\NaiveAPI_Document.asset", typeof(SODocInformation));
                SelectingDocInfo = AssetDatabase.LoadAssetAtPath<SODocInformation>(datas[3]);
                if (SelectingDocInfo == null)
                    SelectingDocInfo = BookRoot;
                MenuDisplaySettings.Clear();
                foreach (var data in datas[4].Split("%SETTING%"))
                {
                    if (data == "")
                        continue;

                    string[] temp = data.Split("$KEY$");
                    MenuDisplaySettings.Add((SODocInformation)AssetDatabase.LoadAssetAtPath(temp[0], typeof(SODocInformation)), JsonUtility.FromJson<MenuDisplaySetting>(temp[1]));
                }

                if(BookRoot == null)
                    BookRoot = AssetDatabase.LoadAssetAtPath<SODocInformation>(DocumentBuilderData.Path.DocumentBuilderRoot + "/Editor/DocAsset/DocumentBuilder Docs.asset");
                if(SelectingDocInfo == null)
                    SelectingDocInfo = BookRoot;
            }
            public static void Save()
            {
                StringBuilder data = new StringBuilder();
                data.Append($"{MenuWidth}%DATA%");
                data.Append($"{MenuUnitHeight}%DATA%");
                data.Append($"{AssetDatabase.GetAssetPath(BookRoot)}%DATA%");
                data.Append($"{AssetDatabase.GetAssetPath(SelectingDocInfo)}%DATA%");
                int count = 0;
                int size = MenuDisplaySettings.Count;
                foreach (var key in MenuDisplaySettings.Keys)
                {
                    data.Append($"{AssetDatabase.GetAssetPath(key)}$KEY${JsonUtility.ToJson(MenuDisplaySettings[key])}");
                    count++;
                    if (count < size)
                        data.Append("%SETTING%");
                }

                File.WriteAllText(Application.temporaryCachePath + "\\DocumentWindowData.txt", data.ToString());
            }
        }
        [System.Serializable]
        class MenuDisplaySetting
        {
            public bool IsShowChildren;

            public MenuDisplaySetting Copy()
            {
                MenuDisplaySetting newSetting = new MenuDisplaySetting();
                newSetting.IsShowChildren = IsShowChildren;
                return newSetting;
            }
        }
    }
}

