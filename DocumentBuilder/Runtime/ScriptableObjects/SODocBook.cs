using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "")]
    public class SODocBook : ScriptableObject
    {
        public SODocPage RootPage;
        public bool DisplayAnimation;
        public bool DisplayChapterMenu;
    }

}