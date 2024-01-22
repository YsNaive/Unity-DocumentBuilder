using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class SDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> s_keys;
        [SerializeField] private List<TValue> s_values;
        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0, imax = s_keys.Count; i < imax; i++)
                Add(s_keys[i], s_values[i]);
            s_keys = null;
            s_values = null;
        }

        public void OnBeforeSerialize()
        {
            s_keys = new List<TKey>();
            s_values = new List<TValue>();
            foreach(var pair in this)
            {
                s_keys.Add(pair.Key);
                s_values.Add(pair.Value);
            }
        }
    }

}