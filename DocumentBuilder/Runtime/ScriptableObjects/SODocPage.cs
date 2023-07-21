using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/Document Builder/Document Page")]
    public class SODocPage : ScriptableObject
    {
        public List<SODocPage> SubPages = new List<SODocPage>();
        public List<DocComponent> Components = new List<DocComponent>();
    }

}