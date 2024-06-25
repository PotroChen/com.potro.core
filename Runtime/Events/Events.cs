using System;
using System.Collections.Generic;

namespace GameFramework
{
    /// <summary>
    /// 事件系统
    /// </summary>
    public static class Events
    {
        public delegate void Callback();
        public delegate void Callback<T>(T arg);

        private static Dictionary<Type, Delegate> m_Delegates = new Dictionary<Type, Delegate>();

        public static void Subscribe<TEvent>(Callback callback) where TEvent : IEvent
        {
            if (m_Delegates.TryGetValue(typeof(TEvent), out Delegate d))
            {
                m_Delegates[typeof(TEvent)] = Delegate.Combine(d, callback);
            }
            else
            {
                m_Delegates.Add(typeof(TEvent), callback);
            }
        }

        public static void Subscribe<TEvent, TArg>(Callback<TArg> callback) where TEvent : IEvent<TArg> where TArg : struct
        {
            if (m_Delegates.TryGetValue(typeof(TEvent), out Delegate d))
            {
                m_Delegates[typeof(TEvent)] = Delegate.Combine(d, callback);
            }
            else
            {
                m_Delegates.Add(typeof(TEvent), callback);
            }
        }

        static public void Unsubscribe<TEvent>(Callback callback) where TEvent : IEvent
        {
            if (m_Delegates.TryGetValue(typeof(TEvent), out Delegate d))
            {
                Delegate result = Delegate.Remove(d, callback);

                if (result == null)
                {
                    m_Delegates.Remove(typeof(TEvent));
                }
                else
                {
                    m_Delegates[typeof(TEvent)] = result;
                }
            }
        }

        static public void Unsubscribe<TEvent, TArg>(Callback<TArg> callback) where TEvent : IEvent<TArg> where TArg : struct
        {
            if (m_Delegates.TryGetValue(typeof(TEvent), out Delegate d))
            {
                Delegate result = Delegate.Remove(d, callback);

                if (result == null)
                {
                    m_Delegates.Remove(typeof(TEvent));
                }
                else
                {
                    m_Delegates[typeof(TEvent)] = result;
                }
            }
        }

        public static void Publish<TEvent>() where TEvent : IEvent
        {
            if (m_Delegates.TryGetValue(typeof(TEvent), out Delegate d))
            {
                Callback callback = d as Callback;

                callback?.Invoke();
            }
        }

        public static void Publish<TEvent, TArg>(TArg arg) where TEvent : IEvent<TArg> where TArg : struct
        {
            if (m_Delegates.TryGetValue(typeof(TEvent), out Delegate d))
            {
                Callback<TArg> callback = d as Callback<TArg>;

                callback?.Invoke(arg);
            }
        }
    }
}