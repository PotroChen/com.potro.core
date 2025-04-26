using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.AddressableAssets
{
    public class ResLoader
    {
        private Dictionary<string, AsyncOperationHandle> mAssetOpHandleCache = new Dictionary<string, AsyncOperationHandle>();

        public T LoadAsset<T>(string location)
        {
            AsyncOperationHandle opHandle = default;
            if (mAssetOpHandleCache.TryGetValue(location, out opHandle))
            {
                if (!opHandle.IsDone)
                {
                    opHandle.WaitForCompletion();
                }
                return (T)opHandle.Result;
            }

            opHandle = Addressables.LoadAssetAsync<T>(location);
            if (opHandle.IsValid())
            {
                opHandle.WaitForCompletion();
                if (opHandle.Result != null)
                {
                    mAssetOpHandleCache.Add(location, opHandle);
                    return (T)opHandle.Result;
                }
                else
                {
                    Addressables.Release(opHandle);
                    return (T)(object)null;
                }
            }
            else
            {
                return (T)(object)null;
            }
        }

        public AsyncOperationHandle LoadAssetAsync<T>(string location, Action<T> completed = null)
        {
            AsyncOperationHandle opHandle = default;
            if (mAssetOpHandleCache.TryGetValue(location, out opHandle))
            {
                if (opHandle.IsDone)
                    completed?.Invoke((T)opHandle.Result);
                else
                {
                    if (completed != null)
                    {
                        opHandle.Completed += (handle) =>
                        {
                            completed?.Invoke((T)opHandle.Result);
                        };
                    }
                }
                return opHandle;
            }

            opHandle = Addressables.LoadAssetAsync<T>(location);
            if (opHandle.IsValid())
            {
                if (completed != null)
                {
                    opHandle.Completed += (handle) =>
                    {
                        completed.Invoke((T)opHandle.Result);
                    };
                }
                mAssetOpHandleCache.Add(location, opHandle);
            }
            return opHandle;
        }

        public void ReleaseAllAssets()
        {
            foreach (var kvp in mAssetOpHandleCache)
            {
                Addressables.Release(kvp.Value);
            }

            mAssetOpHandleCache.Clear();
        }
        
        //TODO Check Asset Exists
    }
}
