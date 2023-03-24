using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Managing a prefab pool using Addressables
/// </summary>
public class PrefabPool
{
    private class Pool
    {
        private string assetKey;
        private Stack<GameObject> objects = new Stack<GameObject>();
        private Vector3 scale;

        public int Count => objects.Count;

        public Pool(string key)
        {
            assetKey = key;
        }

        public void Pop(Action<GameObject> call)
        {
            if (objects.Count > 0)
            {
#if UNITY_EDITOR
                Debug.Log("PrefabPool pop : " + assetKey + " count : " + objects.Count);
#endif
                call(objects.Pop());
            }
            else
            {
                Addressables.InstantiateAsync(assetKey).Completed += (handle) =>
                {
                    if (handle.Result != null)
                    {
#if UNITY_EDITOR
                        Debug.Log(
                            "PrefabPool.InstantiateAsync : "
                                + assetKey
                                + " count : "
                                + objects.Count
                        );
#endif
                        scale = handle.Result.transform.localScale;
                        call(handle.Result);
                    }
                };
            }
        }

        public void Push(GameObject obj)
        {
            if (obj == null)
                return;
            obj.transform.localScale = scale;
            obj.SetActive(false);
            objects.Push(obj);
        }

        public void ClearPool()
        {
            while (objects.Count > 0)
            {
                var gameObject = objects.Pop();
                Addressables.ReleaseInstance(gameObject);
            }
            objects.Clear();
        }
    }

    private Dictionary<string, Pool> poolDic = new Dictionary<string, Pool>();

    private Transform rootTransform;
    public Transform Root
    {
        set => rootTransform = value;
    }

    public void ClearPool()
    {
        foreach (var pool in poolDic.Values)
        {
            pool.ClearPool();
        }
        poolDic.Clear();
    }

    public void Push(string key, GameObject obj)
    {
        if (obj == null)
            return;
        if (rootTransform != null)
            obj.transform.SetParent(rootTransform);
#if UNITY_EDITOR
        obj.name += "_POOL";
#endif
        if (poolDic.ContainsKey(key))
        {
            poolDic[key].Push(obj);
        }
    }

    public void Pop<T>(string key, Action<T> callback)
        where T : UnityEngine.Object
    {
        Pop(
            key,
            (obj) =>
            {
#if UNITY_EDITOR
                obj.name = obj.name.Replace("_POOL", "");
#endif
                callback(obj.GetComponent<T>());
            }
        );
    }

    public void Pop(string key, Action<GameObject> callback)
    {
        if (poolDic.ContainsKey(key))
        {
            poolDic[key].Pop(callback);
        }
        else
        {
            Pool p = new Pool(key);
            poolDic.Add(key, p);
            p.Pop(callback);
        }
    }

    public int GetCount(string key)
    {
        if(poolDic.ContainsKey(key))
        {
            return poolDic[key].Count;
        }
        return 0;
    }
}
