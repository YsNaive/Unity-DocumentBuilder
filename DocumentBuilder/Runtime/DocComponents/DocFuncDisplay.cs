using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public class DocFuncDisplay : DocVisual
    {
        public override string VisualID => "4";

        protected override void OnCreateGUI()
        {
            throw new System.NotImplementedException();
        }

        public class Data
        {
            public string Name = "";
            public List<string> Syntaxs = new List<string>();
            public List<string> ReturnTypes = new List<string>();
            public List<ParamData> Params = new List<ParamData>();
            [System.NonSerialized]
            public List<string> ReturnTypesDescription = new List<string>();
            [System.NonSerialized]
            public List<string> ParamsDescription = new List<string>();

            public Data()
            {
                Syntaxs.Add("");
                ReturnTypes.Add("");
                Params.Add(new ParamData());
                ReturnTypesDescription.Add("");
                ParamsDescription.Add("");
            }

            public void SetParamsDescription(List<string> descriptions, int start, int end)
            {
                ParamsDescription.Clear();
                for (int i = start; i < end; i++)
                {
                    ParamsDescription.Add(descriptions[i]);
                }
            }

            public void SetReturnTypesDescription(List<string> descriptions, int start, int end)
            {
                ReturnTypesDescription.Clear();
                for (int i = start; i < end; i++)
                {
                    ReturnTypesDescription.Add(descriptions[i]);
                }
            }

            public void AddNewParams()
            {
                Params.Add(new ParamData());
                ParamsDescription.Add("");
            }

            public void RemoveLastParams()
            {
                Params.RemoveAt(Params.Count - 1);
                ParamsDescription.RemoveAt(ParamsDescription.Count - 1);
            }

            public void RemoveLastReturnType()
            {
                ReturnTypes.RemoveAt(ReturnTypes.Count - 1);
                ReturnTypesDescription.RemoveAt(ReturnTypesDescription.Count - 1);
            }

            public void AddNewReturnType()
            {
                ReturnTypes.Add("");
                ReturnTypesDescription.Add("");
            }

            public List<string> getTexts()
            {
                List<string> texts = new List<string>();
                for (int i = 0;i < ParamsDescription.Count; i++)
                {
                    texts.Add(ParamsDescription[i]);
                }
                for (int i = 0;i < ReturnTypesDescription.Count;i++)
                {
                    texts.Add(ReturnTypesDescription[i]);
                }

                return texts;
            }
        }

        [System.Serializable]
        public class ParamData
        {
            public string ParamName = "";
            public string Type = "";
        }
    }
}
