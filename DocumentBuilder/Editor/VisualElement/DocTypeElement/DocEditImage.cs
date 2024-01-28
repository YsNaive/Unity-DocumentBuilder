using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Charts/Image")]
    public class DocEditImage : DocEditVisual<DocImage.Data>
    {
        [Obsolete] public override string DisplayName => "Image";

        public override string VisualID => "5";

        private VisualElement root, urlVisual, objVisual, base64Visual;

        protected override void OnCreateGUI()
        {
            LoadDataFromTarget();
            root = new DSHorizontal();
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            TextField scaleField = new DSTextField("Scale", value =>
            {
                float.TryParse(value.newValue, out visualData.scale);
                SaveDataToTarget();
            });
            DocStyle.Current.EndLabelWidth();
            scaleField.value = visualData.scale.ToString();
            scaleField.style.width = Length.Percent(39);
            var enumField = new DSEnumField<DocImage.Mode>("", visualData.mode, value =>
            {
                visualData.mode = value.newValue;
                SaveDataToTarget();
                urlObjDisplay(visualData.mode);
            });
            enumField.style.width = Length.Percent(20);
            urlVisual = generateUrlVisual();
            objVisual = generateObjVisual();
            base64Visual = generateBase64Visual();
            root.Add(scaleField);
            root.Add(enumField);
            this.Add(root);
            urlObjDisplay(visualData.mode);
        }

        public override string ToMarkdown(string dstPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            switch (visualData.mode)
            {
                case DocImage.Mode.Url:
                    string strImage = $"![]({visualData.url})";
                    stringBuilder.Append(strImage);
                    break;
                case DocImage.Mode.Object:
                    break;
            }

            return stringBuilder.ToString();
        }

        private void urlObjDisplay(DocImage.Mode mode)
        {
            if (root.Contains(objVisual))
                root.Remove(objVisual);
            if (root.Contains(urlVisual))
                root.Remove(urlVisual);
            if (root.Contains(base64Visual))
                root.Remove(base64Visual);

            if (mode == DocImage.Mode.Url)
            {
                root.Add(urlVisual);
            }
            else if (mode == DocImage.Mode.Object)
            {
                root.Add(objVisual);
            }
            else if (mode == DocImage.Mode.Base64)
            {
                root.Add(base64Visual);
            }
        }

        private VisualElement generateObjVisual()
        {
            Texture2D texture;
            if (Target.ObjsData.Count != 0)
                texture = (Texture2D)Target.ObjsData[0];
            else
                texture = null;
            ObjectField objectField = DocEditor.NewObjectField<Texture2D>(value =>
            {
                Target.ObjsData.Clear();
                Target.ObjsData.Add(value.newValue);
            });
            objectField.style.width = Length.Percent(40);
            objectField.value = texture;

            return objectField;
        }

        private VisualElement generateBase64Visual()
        {
            var field = new DSTextField();
            field.SetValueWithoutNotify(visualData.base64 == "" ? "Empty" : "Value Assigned");
            field.RegisterValueChangedCallback(evt =>
            {
                SaveDataToTarget();
                if(Convert.TryFromBase64String(evt.newValue, null, out _))
                {
                    visualData.base64 = evt.newValue;
                    field.SetValueWithoutNotify("Value Assigned");
                    field.InputFieldElement.style.unityBackgroundImageTintColor = DocStyle.Current.SuccessColor;
                }
                else
                {
                    visualData.base64 = "";
                    field.SetValueWithoutNotify("Format Error");
                    field.InputFieldElement.style.unityBackgroundImageTintColor = DocStyle.Current.DangerColor;
                }
            });
            field.style.width = Length.Percent(80);
            field[0].style.textOverflow = TextOverflow.Ellipsis;
            var btn = new DSButton("Paste");
            btn.clicked+=() =>
            {
#if UNITY_EDITOR_WIN
                string command = "powershell";
                string arguments = $"-command \"[Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms'); [System.Windows.Forms.Clipboard]::GetImage().Save('{DocCache.DirectoryRoot}\\cache.png')\"";

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process process = new Process { StartInfo = processStartInfo };
                process.Start();
                process.WaitForExit();
                if(process.ExitCode == 0)
                {
                    Texture2D tex = new Texture2D(1, 1);
                    var img = File.ReadAllBytes($"{DocCache.DirectoryRoot}\\cache.png");
                    visualData.base64 = Convert.ToBase64String(img);
                    SaveDataToTarget();
                    File.Delete($"{DocCache.DirectoryRoot}\\cache.png");
                    field.InputFieldElement.style.unityBackgroundImageTintColor = DocStyle.Current.SuccessColor;
                    field.SetValueWithoutNotify("Image Loaded");
                }
                else
                {
                    field.InputFieldElement.style.unityBackgroundImageTintColor = DocStyle.Current.DangerColor;
                    field.SetValueWithoutNotify("Load Failed");
                }
#else
                Debug.Log("Paste image on DocImage is only support on Windows");
#endif

            };

            btn.style.flexGrow = 1f;
            var hor = new DSHorizontal();
            hor.Add(field);
            hor.Add(btn);
            hor.style.flexGrow = 1f;
            return hor;
        }
        private VisualElement generateUrlVisual()
        {
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(10));
            TextField urlField = new DSTextField("Url", value =>
            {
                visualData.url = value.newValue;
                SaveDataToTarget();
            });
            DocStyle.Current.EndLabelWidth();
            urlField.value = visualData.url;
            urlField.style.width = Length.Percent(40);

            return urlField;
        }
    }
}
