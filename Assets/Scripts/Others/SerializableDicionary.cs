using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDicionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] List<MyKeyValuePair<TKey, TValue>> list = new();

    public void OnAfterDeserialize()
    {
        foreach (var i in list) Add(i.key, i.value);
    }

    public void OnBeforeSerialize()
    {
        foreach (var i in this) list.Add(new MyKeyValuePair<TKey, TValue>() { key = i.Key, value = i.Value });
    }
}
[System.Serializable]
public struct MyKeyValuePair<TKey, TValue>
{
    public TKey key;
    public TValue value;
}
