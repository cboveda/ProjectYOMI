using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<T, Y> : ISerializationCallbackReceiver
{
    public List<T> Keys = new();
    public List<Y> Values = new();
    public Dictionary<T, Y> Dictionary = new();

    public void OnAfterDeserialize()
    {
        Keys.Clear();
        Values.Clear();

        foreach (var kvp in Dictionary)
        {
            Keys.Add(kvp.Key);
            Values.Add(kvp.Value);
        }
    }

    public void OnBeforeSerialize()
    {
        Dictionary = new();

        for (int i = 0; i != Math.Min(Keys.Count, Values.Count); i++)
        {
            Dictionary.Add(Keys[i], Values[i]);
        }
    }
}
