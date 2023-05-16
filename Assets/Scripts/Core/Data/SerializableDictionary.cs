using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SerializableDictionary<TKey, TValue> : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private List<KeyValueEntry> _entries;
    private List<TKey> _keys = new();

    public Dictionary<TKey, TValue> Dictionary = new Dictionary<TKey, TValue>();

    [Serializable]
    class KeyValueEntry
    {
        public TKey Key;
        public TValue Value;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return Dictionary.TryGetValue(key, out value);
    }


    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Dictionary.Clear();

        for (int i = 0; i < _entries.Count; i++)
        {
            Dictionary.Add(_entries[i].Key, _entries[i].Value);
        }

    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (_entries == null)
        {
            return;
        }

        _keys.Clear();

        for (int i = 0; i < _entries.Count; i++)
        {
            _keys.Add(_entries[i].Key);
        }

        var result = _keys.GroupBy(x => x)
                 .Where(g => g.Count() > 1)
                 .Select(x => new { Element = x.Key, Count = x.Count() })
                 .ToList();

        if (result.Count > 0)
        {
            var duplicates = string.Join(", ", result);
            Debug.LogError($"Warning {GetType().FullName} keys has duplicates {duplicates}");
        }
    }
}
