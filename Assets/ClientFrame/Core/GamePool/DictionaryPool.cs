using System;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient.GamePool
{
    internal static class DictionaryPool<T1, T2>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<Dictionary<T1, T2>> s_ListPool = new ObjectPool<Dictionary<T1, T2>>(null, l => l.Clear());

        public static Dictionary<T1, T2> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(Dictionary<T1, T2> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}