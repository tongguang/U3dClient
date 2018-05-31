using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

namespace U3dClient
{
    public interface IResourceLoader
    {
        long LoadAssetAsync<T>(string abName, string assetName, Action<T> loadedAction, bool isLoadDepend) where T : Object;
        void UnLoadAsset(long refRequest);
    }
}