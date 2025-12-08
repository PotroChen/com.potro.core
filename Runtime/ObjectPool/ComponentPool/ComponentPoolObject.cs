using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// Component对象池被管理对象
    /// </summary>
    public class ComponentPoolObject<TComponent> : IPoolObject<TComponent> where TComponent : Component
    {
        public IPool<TComponent> Pool { get; set; }

        public TComponent Content { get; set; }

        public ComponentPoolObject(ComponentPool<TComponent> pool,TComponent content)
        {
            Pool = pool;
            Content = content;
        }

        public void Recycle()
        {
            Pool.RecycleObject(this);
        }

        public void Dispose()
        {
            Pool.DisposeObject(this);
        }
    }
}
