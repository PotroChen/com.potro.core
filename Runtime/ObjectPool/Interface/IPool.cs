using System;

/// <summary>
/// 对象池接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPool<T>
{
    event Action<T> OnRecycle;
    /// <summary>
    /// 获取IPoolObject
    /// </summary>
    /// <returns></returns>
    IPoolObject<T> Get();

    /// <summary>
    /// 回收IPoolObject
    /// </summary>
    /// <param name="poolObject"></param>
    void RecycleObject(IPoolObject<T> poolObject);

    /// <summary>
    /// 销毁IPoolObject
    /// </summary>
    /// <param name="poolObject"></param>
    void DisposeObject(IPoolObject<T> poolObject);

    /// <summary>
    /// 销毁自己
    /// </summary>
    void Dispose();
}
