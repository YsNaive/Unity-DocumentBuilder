using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DSAssetFolderField : DSHorizontal, INotifyValueChanged<string>
    {
        private string m_value = "Assets";
        public string value { get => m_value; 
            set
            {
                using ChangeEvent<string> evt = ChangeEvent<string>.GetPooled(m_value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);
            }
        }
        PopupElement popup;
        DSTextElement field;
        ObjectField objectField;
        public DSAssetFolderField()
        {
            popup = new PopupElement();
            popup.style.backgroundColor = DocStyle.Current.BackgroundColor;
            popup.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1.5f));
            popup.style.SetIS_Style(ISPadding.Pixel(5));
            field = new DSTextElement();
            field.focusable = false;
            field.style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            field.style.flexGrow = 1;
            objectField = DocEditor.NewObjectField<DefaultAsset>("");
            objectField.style.flexGrow = 1;
            objectField.Q<Label>().RegisterValueChangedCallback(evt => { evt.StopPropagation(); });
            objectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null) return;
                var path = AssetDatabase.GetAssetPath(evt.newValue);
                if (AssetDatabase.IsValidFolder(path))
                    value = path;
                else
                    objectField.SetValueWithoutNotify(evt.previousValue);
            });
            var icon = new VisualElement();
            icon.style.SetIS_Style(DocStyle.Current.ArrowIcon);
            icon.style.unityBackgroundImageTintColor = Color.white;
            icon.style.backgroundImage = DocEditorData.Instance.BuildinIcon.Find(match => { return match.name == "Folder"; });
            Add(icon);
            var fieldHor = new DSHorizontal(field, objectField);
            fieldHor.style.flexGrow = 1;
            Add(fieldHor);
            field.RegisterCallback<PointerDownEvent>(evt =>
            {
                openPopup(m_value);
            });
            SetValueWithoutNotify(m_value);
        }
        void openPopup(string rootPath)
        {
            popup.Open(this);
            var pos = field.worldBound.position;
            pos = popup.CoverMask.WorldToLocal(pos);
            popup.style.left = pos.x;
            popup.style.top = pos.y + field.layout.height;

            popup.Clear();
            var path = rootPath;
            if (path != "Assets")
            {
                path = rootPath.Substring(0, path.LastIndexOf('/'));
                popup.Add(createSelectText(path, false));
            }
            var name = rootPath;
            var index = name.LastIndexOf("/");
            if (index != -1)
                name = name.Substring(index + 1);
            var icon = new VisualElement();
            icon.style.SetIS_Style(DocStyle.Current.ArrowIcon);
            icon.style.unityBackgroundImageTintColor = Color.white;
            icon.style.opacity = 0.6f;
            icon.style.backgroundImage = DocEditorData.Instance.BuildinIcon.Find(match => { return match.name == "Folder"; });
            var nameText = new DSTextElement(name);
            var hor = new DSHorizontal();
            hor.Add(icon);
            hor.Add(nameText);
            popup.Add(hor);
            foreach (var subPath in AssetDatabase.GetSubFolders(rootPath))
                popup.Add(createSelectText(subPath, true));
        }
        VisualElement createSelectText(string path, bool isForward)
        {
            var name = "";
            var i = path.LastIndexOf("/");
            if (i != -1)
                name = path.Substring(i + 1);
            else
                name = path;
            DSHorizontal root = new();
            root.Add(new DSTextElement(name));
            var arrow = new VisualElement();
            arrow.style.SetIS_Style(DocStyle.Current.ArrowIcon);
            if (!isForward)
                arrow.style.rotate = new Rotate(180);
            else
                arrow.style.marginLeft = DocStyle.Current.LineHeight;
            root.Insert(0, arrow);
            root.RegisterCallback<PointerDownEvent>(evt =>
            {
                openPopup(path);
                value = path;
            });
            root.RegisterCallback<PointerEnterEvent>(evt =>
            {
                root.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            });
            root.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                root.style.backgroundColor = Color.clear;
            });
            return root;
        }
        public void SetValueWithoutNotify(string newValue)
        {
            if (!AssetDatabase.IsValidFolder(newValue))
                throw new System.Exception("Given value to DSAssetFolderField is not a valid Assets folder.");
            m_value = newValue;
            (field as INotifyValueChanged<string>).SetValueWithoutNotify(newValue.Substring(newValue.LastIndexOf("/") + 1));
            objectField.SetValueWithoutNotify(AssetDatabase.LoadAssetAtPath<DefaultAsset>(newValue));
        }
    }


}
