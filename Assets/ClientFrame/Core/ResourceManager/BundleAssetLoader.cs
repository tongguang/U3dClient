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
    public class BundleAssetBaseLoader : BaseLoader
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
                FullBundleBaseLoader.SUnLoad(m_BundleIndex);
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
                m_BundleIndex = FullBundleBaseLoader.SLoadSync(m_BundleName, null);
                var bundleLoader = FullBundleBaseLoader.SGetLoader(m_BundleIndex);
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
            m_BundleIndex = FullBundleBaseLoader.SLoadAsync(m_BundleName, null);
            var bundleLoader = FullBundleBaseLoader.SGetLoader(m_BundleIndex);
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
            STryUnLoadByAssetKey(m_AssetKeyName);
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

            STryUnLoadByAssetKey(m_AssetKeyName);
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

        private static readonly ObjectPool<BundleAssetBaseLoader> s_LoaderPool =
            new ObjectPool<BundleAssetBaseLoader>(
                (loader) => { loader.OnReuse(); },
                (loader) => { loader.OnRecycle(); });

        private static readonly Action<bool, Object> s_DefaultLoadedCallback = (isOk, Object) => { };

        private static readonly Dictionary<string, BundleAssetBaseLoader> s_NameToLoader =
            new Dictionary<string, BundleAssetBaseLoader>();

        private static readonly Dictionary<int, BundleAssetBaseLoader> s_ResIndexToLoader =
            new Dictionary<int, BundleAssetBaseLoader>();

        private static void SCalculateAssetKey(string bundleName, string assetName, out string assetKey)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(bundleName);
            stringBuilder.Append(s_BundleANdAssetSpliteChar);
            stringBuilder.Append(assetName);
            assetKey = stringBuilder.ToString();
        }

        public static int SLoadAsync<T>(string bundleName, string assetName, Action<bool, T> loadedAction)
            where T : Object
        {
            BundleAssetBaseLoader baseLoader;
            string assetKey;
            SCalculateAssetKey(bundleName, assetName, out assetKey);
            s_NameToLoader.TryGetValue(assetKey, out baseLoader);
            if (baseLoader == null)
            {
                baseLoader = s_LoaderPool.Get();
                baseLoader.Init(bundleName, assetName, assetKey);
                s_NameToLoader.Add(assetKey, baseLoader);
            }

            var resIndex = loadedAction != null
                ? baseLoader.InternalLoadAsync((isOk, assetObj) => { loadedAction(isOk, assetObj as T); })
                : baseLoader.InternalLoadAsync(null);
            s_ResIndexToLoader.Add(resIndex, baseLoader);

            return resIndex;
        }

        public static int SLoadSync<T>(string bundleName, string assetName, Action<bool, T> loadedAction)
            where T : Object
        {
            BundleAssetBaseLoader baseLoader;
            string assetKey;
            SCalculateAssetKey(bundleName, assetName, out assetKey);

            s_NameToLoader.TryGetValue(assetKey, out baseLoader);
            if (baseLoader == null)
            {
                baseLoader = s_LoaderPool.Get();
                baseLoader.Init(bundleName, assetName, assetKey);
                s_NameToLoader.Add(assetKey, baseLoader);
            }

            var resIndex = loadedAction != null
                ? baseLoader.InternalLoadSync((isOk, assetObj) => { loadedAction(isOk, assetObj as T); })
                : baseLoader.InternalLoadSync(null);
            s_ResIndexToLoader.Add(resIndex, baseLoader);

            return resIndex;
        }

        public static BundleAssetBaseLoader SGetLoader(int resouceIndex)
        {
            BundleAssetBaseLoader baseLoader = null;
            s_ResIndexToLoader.TryGetValue(resouceIndex, out baseLoader);
            return baseLoader;
        }

        public static BundleAssetBaseLoader SGetLoader(string bundleName, string assetName)
        {
            BundleAssetBaseLoader baseLoader = null;
            string assetKey;
            SCalculateAssetKey(bundleName, assetName, out assetKey);
            s_NameToLoader.TryGetValue(assetKey, out baseLoader);
            return baseLoader;
        }

        public static void SUnLoad(int resouceIndex)
        {
            BundleAssetBaseLoader baseLoader;
            s_ResIndexToLoader.TryGetValue(resouceIndex, out baseLoader);
            if (baseLoader != null)
            {
                baseLoader.InternalUnload(resouceIndex);
            }
        }

        private static void STryUnLoadByName(string bundleName, string assetName)
        {
            string assetKey;
            SCalculateAssetKey(bundleName, assetName, out assetKey);
            STryUnLoadByAssetKey(assetKey);
        }

        private static void STryUnLoadByAssetKey(string assetKey)
        {
            BundleAssetBaseLoader baseLoader;
            s_NameToLoader.TryGetValue(assetKey, out baseLoader);
            if (baseLoader != null && baseLoader.CanRealUnload())
            {
                s_NameToLoader.Remove(assetKey);
                s_LoaderPool.Release(baseLoader);
            }
        }
    }
}