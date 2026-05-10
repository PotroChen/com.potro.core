using System.Collections;
using UnityEngine;

namespace GameFramework
{
    public class GlobalCoroutineHelper : MonoSingleton<GlobalCoroutineHelper>
    {
        public new static Coroutine StartCoroutine(IEnumerator routine)
        {
            return ((MonoBehaviour)Instance).StartCoroutine(routine);
        }

        public new static void StopCoroutine(Coroutine routine)
        {
            if (routine != null)
                ((MonoBehaviour)Instance).StopCoroutine(routine);
        }
    }
}

