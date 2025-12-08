using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// Component对象池
    /// PS:GameObjectPool每次Get后,都要GetComponent,很麻烦
    /// </summary>
    public class ComponentPool<TComponent> : IPool<TComponent> where TComponent:Component
    {
        public event Action<TComponent> OnRecycle;
        public int MaxSize { get; set; }
        public int TotalCount { get { return objectsInPool.Count + objectsPoped.Count; } }
        private List<IPoolObject<TComponent>> objectsInPool = new List<IPoolObject<TComponent>>();
        private List<IPoolObject<TComponent>> objectsPoped = new List<IPoolObject<TComponent>>();
        private TComponent template;

        private GameObject poolRoot;
        private bool rootCreateBySelf = true;

        public ComponentPool(TComponent template,int maxSize,Transform poolRoot = null)
        {
            this.template = template;
            this.MaxSize = maxSize;

            if (poolRoot == null)
            {
                this.poolRoot = new GameObject("ComponentPool");
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
                var  content = UnityEngine.Object.Instantiate(template, poolRoot.transform);
                content.gameObject.SetActive(false);
                IPoolObject<TComponent> poolObject = new ComponentPoolObject<TComponent>(this, content);
                objectsInPool.Add(poolObject);
            }
        }

        public IPoolObject<TComponent> Get()
        {
            IPoolObject<TComponent> poolObject = Get_Internal(null);
            return poolObject;
        }

        public IPoolObject<TComponent> Get(Transform parent)
        {
            IPoolObject<TComponent> poolObject = Get_Internal(parent);
            return poolObject;
        }

        private IPoolObject<TComponent> Get_Internal(Transform parent)
        {
            IPoolObject<TComponent> poolObject = null;

            if (objectsInPool.Count == 0)
            {
                TComponent content = UnityEngine.Object.Instantiate(template, parent);
                poolObject = new ComponentPoolObject<TComponent>(this, content);

                poolObject.Content.gameObject.SetActive(true);

                objectsPoped.Add(poolObject);
            }
            else
            {
                int lastIndex = objectsInPool.Count - 1;
                poolObject = objectsInPool[lastIndex];
                poolObject.Content.transform.SetParent(parent);
                poolObject.Content.gameObject.SetActive(true);

                objectsInPool.RemoveAt(lastIndex);
                objectsPoped.Add(poolObject);
            }
            return poolObject;
        }

        public void RecycleObject(IPoolObject<TComponent> poolObject)
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
                poolObject.Content.gameObject.SetActive(false);
            }
        }

        public void DisposeObject(IPoolObject<TComponent> poolObject)
        {
            if (poolObject.Content != null)
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
}
