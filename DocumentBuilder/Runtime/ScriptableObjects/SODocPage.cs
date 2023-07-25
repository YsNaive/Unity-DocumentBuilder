using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName ="Naive API/Doc Page")]
    public class SODocPage : ScriptableObject
    {
        public Texture2D Icon;
        public List<SODocPage> SubPages = new List<SODocPage>();
        public List<DocComponent> Components = new List<DocComponent>();
    }
}
