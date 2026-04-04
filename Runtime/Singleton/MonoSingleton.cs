using UnityEngine;

namespace GameFramework
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static bool applicationIsQuitting;

        private static T m_Instance = null;

        private static readonly object m_lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                lock (m_lock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = Object.FindObjectOfType(typeof(T)) as T;
                        if (m_Instance == null)
                        {
                            m_Instance = new GameObject("Singleton of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                            m_Instance.Init();
                        }
                    }
                }

                return m_Instance;
            }
        }

        protected virtual void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this as T;
                Object.DontDestroyOnLoad(base.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            m_Instance = null;
        }

        public virtual void Init()
        {
        }

        private void OnApplicationQuit()
        {
            applicationIsQuitting = true;
            m_Instance = null;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.quitting += delegate
            {
                applicationIsQuitting = true;
            };
        }
    }
}