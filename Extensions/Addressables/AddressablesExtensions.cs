using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.AddressableAssets
{
    public static class AddressablesExtensions
    {
        private static List<string> temp_Keys = new List<string>();
        public static bool TryGetLocation<T>(string key, out IResourceLocation location, bool mute = false)
        {
            temp_Keys.Clear();
            temp_Keys.Add(key);
            return TryGetLocation(temp_Keys,typeof(T),out location, mute);
        }

        public static bool TryGetLocation(IList<string> keys, Type type, out IResourceLocation location, bool mute = false)
        {
            location = default;
            if (TryGetLocations(keys, type, out IList<IResourceLocation> locations, mute))
            {
                if (locations.Count > 1)
                {
                    if (!mute)
                    {
                        string msg = "找到Keys所对应的资源数量大于1个,Keys:";
                        foreach (var key in keys)
                        {
                            msg += $"|{key}|";
                        }
                        Debug.LogError(msg);
                    }
                    return false;
                }

                location = locations[0];
                return true;
            }
            return false;
        }

        private static bool TryGetLocations(IList<string> keys, Type type, out IList<IResourceLocation> locations, bool mute = false)
        {
            locations = null;
            var loadResLocationOp = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Intersection, type);
            loadResLocationOp.WaitForCompletion();
            if (loadResLocationOp.Result.Count <= 0)
            {
                if (!mute)
                {
                    string msg = "无法找到Keys所对应的资源,Keys:";
                    foreach (var key in keys)
                    {
                        msg += $"|{key}|";
                    }
                    Debug.LogError(msg);
                }
                return false;
            }

            locations = loadResLocationOp.Result;
            return true;
        }
    }

}
