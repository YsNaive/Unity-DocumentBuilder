using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName ="Naive API/DocumentBuilder/Doc Page")]
    public class SODocPage : ScriptableObject
    {
        public Texture2D Icon;
        public List<SODocPage> SubPages = new List<SODocPage>();
        public List<DocComponent> Components = new List<DocComponent>();
        public DocPageAniMode IntroMode = DocPageAniMode.Sametime;
        public DocPageAniMode OuttroMode = DocPageAniMode.Sametime;
        public int IntroDuration = 200;
        public int OuttroDuration = 200;
        public enum DocPageAniMode
        {
            None,
            Sametime,
            OneByOne,
            Flow,
        }
    }
}
