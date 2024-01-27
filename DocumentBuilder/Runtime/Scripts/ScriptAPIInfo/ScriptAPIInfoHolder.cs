using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public static class ScriptAPIInfoHolder
    {
        public static Dictionary<Type, SOScriptAPIInfo> Infos = new();
    }
}