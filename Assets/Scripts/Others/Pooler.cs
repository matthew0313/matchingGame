using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class Pooler<T> where T : Component
{
    List<T> pool = new();
    Action<T> onTakeout, onRelease;
    readonly int maxSize, defaultSize;
    T prefab;
    public Pooler(T prefab, int maxSize = 100, int defaultSize = 0, Action<T> onTakeout = null, Action<T> onRelease = null)
    {
        this.prefab = prefab;
        this.maxSize = maxSize;
        this.defaultSize = defaultSize;
        this.onTakeout = onTakeout;
        this.onRelease = onRelease;
        for (int i = 0; i < defaultSize; i++) 
        {
            Release(MonoBehaviour.Instantiate(prefab));
        }
    }
    public T GetObject()
    {
        T obj = Get();
        OnTakeout(obj);
        return obj;
    }
    public T GetObject(Vector3 position, Quaternion rotation)
    {
        T obj = Get();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        OnTakeout(obj);
        return obj;
    }
    public T GetObject(Vector3 position, Quaternion rotation, Transform parent)
    {
        T obj = Get();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.parent = parent;
        OnTakeout(obj);
        return obj;
    }
    public T GetObject(Transform parent)
    {
        T obj = Get();
        obj.transform.parent = parent;
        OnTakeout(obj);
        return obj;
    }
    void OnTakeout(T obj)
    {
        if (onTakeout == null) obj.gameObject.SetActive(true);
        else onTakeout.Invoke(obj);
    }
    public void ReleaseObject(T obj)
    {
        Release(obj);
    }
    T Get()
    {
        pool.RemoveAll((T obj) => obj == null);
        if(pool.Count == 0) Release(MonoBehaviour.Instantiate(prefab));
        T result = pool[0];
        pool.RemoveAt(0);
        return result;
    }
    void Release(T obj)
    {
        if(pool.Count >= maxSize)
        {
            MonoBehaviour.Destroy(pool[0].gameObject);
            pool.RemoveAt(0);
        }
        OnRelease(obj);
        pool.Add(obj);
    }
    void OnRelease(T obj)
    {
        if(onRelease == null) obj.gameObject.SetActive(false);
        else onRelease.Invoke(obj);
    }
}