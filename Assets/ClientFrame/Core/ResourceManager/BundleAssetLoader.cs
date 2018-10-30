using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using U3dClient.GamePool;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace U3dClient.ResourceMgr
{
    public class BundleAssetLoader : BaseLoader
    {
        private string m_BundleName = null;
        private string m_AssetName = null;
        private string m_AssetKeyName = null;
        private LoadState m_LoadState = LoadState.Init;
        private int m_BundleIndex = -1;
        private Object m_AssetObject = null;
        private readonly HashSet<int> m_ResouceIndexSet = new HashSet<int>();

        private readonly Dictionary<int, Action<bool, Object>> m_LoadedCallbackDict =
            new Dictionary<int, Action<bool, Object>>();

        protected override void OnReuse()
        {
            ResetData();
        }

        protected override void OnRecycle()
        {
            if (m_BundleIndex != -1)
            {
                FullBundleBaseLoader.UnLoad(m_BundleIndex);
            }
            ResetData();
        }

        protected override void ResetData()
        {
            m_BundleName = null;
            m_AssetName = null;
            m_AssetKeyName = null;
            m_LoadState = LoadState.Init;
            m_BundleIndex = -1;
            m_AssetObject = null;
            m_ResouceIndexSet.Clear();
            m_LoadedCallbackDict.Clear();
        }

        private void Init(string bundleName, string assetName, string assetKey)
        {
            m_BundleName = bundleName;
            m_AssetName = assetName;
            m_AssetKeyName = assetKey;
        }

        private int InternalLoadAsync(Action<bool, Object> loadedAction)
        {
            if (loadedAction == null)
            {
                loadedAction = s_DefaultLoadedCallback;
            }

            var index = ResourceManager.GetNewResourceIndex();
            m_ResouceIndexSet.Add(index);
            if (m_LoadState == LoadState.Init)
            {
                m_LoadedCallbackDict.Add(index, loadedAction);
                m_LoadState = LoadState.WaitLoad;
                MainThreadDispatcher.StartCoroutine(LoadFuncEnumerator());
            }
            else if (m_LoadState == LoadState.WaitLoad || m_LoadState == LoadState.Loading)
            {
                m_LoadedCallbackDict.Add(index, loadedAction);
            }
            else
            {
                loadedAction(m_AssetObject != null, m_AssetObject);
            }

            return index;
        }

        private int InternalLoadSync(Action<bool, Object> loadedAction)
        {
            if (loadedAction == null)
            {
                loadedAction = s_DefaultLoadedCallback;
            }

            var index = ResourceManager.GetNewResourceIndex();
            m_ResouceIndexSet.Add(index);
            if (m_LoadState == LoadState.Init || m_LoadState == LoadState.WaitLoad)
            {
                m_BundleIndex = FullBundleBaseLoader.LoadSync(m_BundleName, null);
                var bundleLoader = FullBundleBaseLoader.GetLoader(m_BundleIndex);
                var bundle = bundleLoader.GetAssetBundle();
                m_AssetObject = bundle.LoadAsset<Object>(m_AssetName);

                m_LoadState = LoadState.Complete;
                loadedAction(m_AssetObject != null, m_AssetObject);
            }
            else if (m_LoadState == LoadState.Loading)
            {
                Debug.LogWarning("错误加载 fullbundleloader");
            }
            else
            {
                loadedAction(m_AssetObject != null, m_AssetObject);
            }

            return index;
        }

        protected override IEnumerator LoadFuncEnumerator()
        {
            if (m_LoadState != LoadState.WaitLoad)
            {
                yield break;
            }

            m_LoadState = LoadState.Loading;
            m_BundleIndex = FullBundleBaseLoader.LoadAsync(m_BundleName, null);
            var bundleLoader = FullBundleBaseLoader.GetLoader(m_BundleIndex);
            while (!bundleLoader.IsComplate)
            {
                yield return null;
            }

            var bundle = bundleLoader.GetAssetBundle();
            var request = bundle.LoadAssetAsync<Object>(m_AssetName);
            while (!request.isDone)
            {
                yield return null;
            }

            m_AssetObject = request.asset;
            m_LoadState = LoadState.Complete;

            foreach (var action in m_LoadedCallbackDict)
            {
                var callback = action.Value;
                callback(m_AssetObject != null, m_AssetObject);
            }

            m_LoadedCallbackDict.Clear();
            TryUnLoadByAssetKey(m_AssetKeyName);
        }

        private void InternalUnload(int resourceIndex)
        {
            if (m_LoadedCallbackDict.ContainsKey(resourceIndex))
            {
                m_LoadedCallbackDict.Remove(resourceIndex);
            }

            if (m_ResouceIndexSet.Contains(resourceIndex))
            {
                m_ResouceIndexSet.Remove(resourceIndex);
            }

            TryUnLoadByAssetKey(m_AssetKeyName);
        }

        private bool CanRealUnload()
        {
            if (m_LoadState == LoadState.Init || m_LoadState == LoadState.WaitLoad)
            {
                return true;
            }
            else if (m_LoadState == LoadState.Loading)
            {
                return false;
            }
            else
            {
                if (m_ResouceIndexSet.Count > 0)
                {
                    return false;
                }

                return true;
            }
        }

        private static string s_BundleANdAssetSpliteChar = "|";

        private static readonly ObjectPool<BundleAssetLoader> s_LoaderPool =
            new ObjectPool<BundleAssetLoader>(
                (loader) => { loader.OnReuse(); },
                (loader) => { loader.OnRecycle(); });

        private static readonly Action<bool, Object> s_DefaultLoadedCallback = (isOk, Object) => { };

        private static readonly Dictionary<string, BundleAssetLoader> s_NameToLoader =
            new Dictionary<string, BundleAssetLoader>();

        private static readonly Dictionary<int, BundleAssetLoader> s_ResIndexToLoader =
            new Dictionary<int, BundleAssetLoader>();

        private static void CalculateAssetKey(string bundleName, string assetName, out string assetKey)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(bundleName);
            stringBuilder.Append(s_BundleANdAssetSpliteChar);
            stringBuilder.Append(assetName);
            assetKey = stringBuilder.ToString();
        }

        public static int LoadAsync<T>(string bundleName, string assetName, Action<bool, T> loadedAction)
            where T : Object
        {
            BundleAssetLoader loader;
            string assetKey;
            CalculateAssetKey(bundleName, assetName, out assetKey);
            s_NameToLoader.TryGetValue(assetKey, out loader);
            if (loader == null)
            {
                loader = s_LoaderPool.Get();
                loader.Init(bundleName, assetName, assetKey);
                s_NameToLoader.Add(assetKey, loader);
            }

            var resIndex = loadedAction != null
                ? loader.InternalLoadAsync((isOk, assetObj) => { loadedAction(isOk, assetObj as T); })
                : loader.InternalLoadAsync(null);
            s_ResIndexToLoader.Add(resIndex, loader);

            return resIndex;
        }

        public static int LoadSync<T>(string bundleName, string assetName, Action<bool, T> loadedAction)
            where T : Object
        {
            BundleAssetLoader loader;
            string assetKey;
            CalculateAssetKey(bundleName, assetName, out assetKey);

            s_NameToLoader.TryGetValue(assetKey, out loader);
            if (loader == null)
            {
                loader = s_LoaderPool.Get();
                loader.Init(bundleName, assetName, assetKey);
                s_NameToLoader.Add(assetKey, loader);
            }

            var resIndex = loadedAction != null
                ? loader.InternalLoadSync((isOk, assetObj) => { loadedAction(isOk, assetObj as T); })
                : loader.InternalLoadSync(null);
            s_ResIndexToLoader.Add(resIndex, loader);

            return resIndex;
        }

        public static BundleAssetLoader GetLoader(int resouceIndex)
        {
            BundleAssetLoader loader = null;
            s_ResIndexToLoader.TryGetValue(resouceIndex, out loader);
            return loader;
        }

        public static BundleAssetLoader GetLoader(string bundleName, string assetName)
        {
            BundleAssetLoader loader = null;
            string assetKey;
            CalculateAssetKey(bundleName, assetName, out assetKey);
            s_NameToLoader.TryGetValue(assetKey, out loader);
            return loader;
        }

        public static void UnLoad(int resouceIndex)
        {
            BundleAssetLoader loader;
            s_ResIndexToLoader.TryGetValue(resouceIndex, out loader);
            if (loader != null)
            {
                loader.InternalUnload(resouceIndex);
            }
        }

        private static void TryUnLoadByName(string bundleName, string assetName)
        {
            string assetKey;
            CalculateAssetKey(bundleName, assetName, out assetKey);
            TryUnLoadByAssetKey(assetKey);
        }

        private static void TryUnLoadByAssetKey(string assetKey)
        {
            BundleAssetLoader loader;
            s_NameToLoader.TryGetValue(assetKey, out loader);
            if (loader != null && loader.CanRealUnload())
            {
                s_NameToLoader.Remove(assetKey);
                s_LoaderPool.Release(loader);
            }
        }
    }
}