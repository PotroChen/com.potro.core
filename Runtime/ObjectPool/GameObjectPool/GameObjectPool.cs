using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;

/// <summary>
/// GameObject对象池
/// </summary>
public class GameObjectPool : IPool<GameObject>
{
    public event Action<GameObject> OnRecycle;

    public int MaxSize { get; set; }
    public int TotalCount { get { return objectsInPool.Count + objectsPoped.Count; } }
    private List<IPoolObject<GameObject>> objectsInPool = new List<IPoolObject<GameObject>>();
    private List<IPoolObject<GameObject>> objectsPoped = new List<IPoolObject<GameObject>>();
    private GameObject prefab;

    private GameObject poolRoot;
    private bool rootCreateBySelf = true;
    public GameObjectPool(GameObject prefab, int maxSize,Transform poolRoot = null)
    {
        this.prefab = prefab;
        this.MaxSize = maxSize;

        if (poolRoot == null)
        {
            this.poolRoot = new GameObject("GameObjectPool");
            UnityEngine.Object.DontDestroyOnLoad(this.poolRoot);
        }
        else
        {
            this.poolRoot = poolRoot.gameObject;
            rootCreateBySelf = false;
        }
    }

    public void WarmUp()
    {
        while (TotalCount < MaxSize)
        {
            GameObject content = GameObject.Instantiate(prefab, poolRoot.transform);
            content.SetActive(false);
            IPoolObject<GameObject> poolObject = new GameObjectPoolObject(this, content);
            objectsInPool.Add(poolObject);
        }
    }

    public IPoolObject<GameObject> Get()
    {
        IPoolObject<GameObject> poolObject = Get_Internal(null);
        return poolObject;
    }

    public IPoolObject<GameObject> Get(Transform parent) 
    {
        IPoolObject<GameObject> poolObject = Get_Internal(parent);
        return poolObject;
    }

    private IPoolObject<GameObject> Get_Internal(Transform parent)
    {
        IPoolObject<GameObject> poolObject = null;

        if (objectsInPool.Count == 0)
        {
            GameObject content = GameObject.Instantiate(prefab, parent);
            poolObject = new GameObjectPoolObject(this, content);

            poolObject.Content.SetActive(true);

            objectsPoped.Add(poolObject);
        }
        else
        {
            int lastIndex = objectsInPool.Count - 1;
            poolObject = objectsInPool[lastIndex];
            poolObject.Content.transform.SetParent(parent);
            poolObject.Content.SetActive(true);

            objectsInPool.RemoveAt(lastIndex);
            objectsPoped.Add(poolObject);
        }
        return poolObject;
    }

    public void RecycleObject(IPoolObject<GameObject> poolObject)
    {
        if (poolObject.Pool != this)
            return;

        OnRecycle?.Invoke(poolObject.Content);
        if (TotalCount > MaxSize)
        {
            poolObject.Dispose();
        }
        else
        {
            objectsPoped.Remove(poolObject);
            objectsInPool.Add(poolObject);

            poolObject.Content.transform.SetParent(poolRoot.transform);
            poolObject.Content.SetActive(false);
        }
    }

    public void DisposeObject(IPoolObject<GameObject> poolObject)
    {
        if(poolObject.Content!=null)
            GameObject.DestroyImmediate(poolObject.Content);

        if (objectsPoped.Contains(poolObject))
            objectsPoped.Remove(poolObject);

        if (objectsInPool.Contains(poolObject))
            objectsInPool.Remove(poolObject);
    }

    public void Dispose()
    {
        if (rootCreateBySelf && poolRoot != null)
        {
            GameObject.DestroyImmediate(poolRoot);
        }
        for (int i = 0; i < objectsInPool.Count; i++)
        {
            objectsInPool[i].Dispose();
        }

        for (int i = objectsPoped.Count - 1; i >= 0; i--)
        {
            objectsPoped[i].Dispose();
        }
    }
}
