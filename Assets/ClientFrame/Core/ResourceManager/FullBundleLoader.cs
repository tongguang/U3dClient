using System;
using System.Collections;
using System.Collections.Generic;
using U3dClient.GamePool;
using UnityEngine;
using UniRx;

namespace U3dClient.ResourceMgr
{
    public class FullBundleBaseLoader : BaseLoader
    {
        private string m_BundleName = null;
        private LoadState m_LoadState = LoadState.Init;
        private AssetBundle m_Bundle = null;
        private int m_BundleIndex = -1;
        private List<int> m_DependBundleIndexList = new List<int>();
        private readonly HashSet<int> m_ResouceIndexSet = new HashSet<int>();

        private readonly Dictionary<int, Action<bool, AssetBundle>> m_LoadedCallbackDict =
            new Dictionary<int, Action<bool, AssetBundle>>();

        public bool IsComplate
        {
            get { return m_LoadState == LoadState.Complete; }
        }

        protected override void OnReuse()
        {
            ResetData();
        }

        protected override void OnRecycle()
        {
            if (m_BundleIndex != -1)
            {
                SingleBundleBaseLoader.SUnLoad(m_BundleIndex);
            }

            foreach (var dependIndex in m_DependBundleIndexList)
            {
                SingleBundleBaseLoader.SUnLoad(dependIndex);
            }
            ResetData();
        }

        protected sealed override void ResetData()
        {
            m_BundleName = "";
            m_Bundle = null;
            m_LoadState = LoadState.Init;
            m_BundleIndex = -1;
            m_DependBundleIndexList.Clear();
            m_ResouceIndexSet.Clear();
        }

        private void Init(string bundleName)
        {
            m_BundleName = bundleName;
        }

        private int InternalLoadAsync(Action<bool, AssetBundle> loadedAction)
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
                loadedAction(m_Bundle != null, m_Bundle);
            }

            return index;
        }

        private int InternalLoadSync(Action<bool, AssetBundle> loadedAction)
        {
            if (loadedAction == null)
            {
                loadedAction = s_DefaultLoadedCallback;
            }

            var index = ResourceManager.GetNewResourceIndex();
            m_ResouceIndexSet.Add(index);
            if (m_LoadState == LoadState.Init || m_LoadState == LoadState.WaitLoad)
            {
                var depends = s_ManifestAsset.GetAllDependencies(m_BundleName);
                foreach (var depend in depends)
                {
                    var resIndex = SingleBundleBaseLoader.SLoadSync(depend, null);
                    m_DependBundleIndexList.Add(resIndex);
                }

                m_BundleIndex = SingleBundleBaseLoader.SLoadSync(m_BundleName, null);
                var bundleLoader = SingleBundleBaseLoader.SGetLoader(m_BundleIndex);
                m_Bundle = bundleLoader.GetAssetBundle();

                m_LoadState = LoadState.Complete;
                loadedAction(m_Bundle != null, m_Bundle);
            }
            else if (m_LoadState == LoadState.Loading)
            {
                Debug.LogWarning("错误加载 fullbundleloader");
            }
            else
            {
                loadedAction(m_Bundle != null, m_Bundle);
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
            if (s_ManifestAsset)
            {
                var depends = s_ManifestAsset.GetAllDependencies(m_BundleName);
                foreach (var depend in depends)
                {
                    var resIndex = SingleBundleBaseLoader.SLoadAsync(depend, null);
                    m_DependBundleIndexList.Add(resIndex);
                }
            }

            SingleBundleBaseLoader bundleBaseLoader;
            foreach (var resIndex in m_DependBundleIndexList)
            {
                bundleBaseLoader = SingleBundleBaseLoader.SGetLoader(resIndex);
                while (!bundleBaseLoader.IsComplate)
                {
                    yield return null;
                }
            }

            m_BundleIndex = SingleBundleBaseLoader.SLoadAsync(m_BundleName, null);
            bundleBaseLoader = SingleBundleBaseLoader.SGetLoader(m_BundleIndex);
            while (!bundleBaseLoader.IsComplate)
            {
                yield return null;
            }

            m_Bundle = bundleBaseLoader.GetAssetBundle();
            m_LoadState = LoadState.Complete;

            foreach (var action in m_LoadedCallbackDict)
            {
                var callback = action.Value;
                callback(m_Bundle != null, m_Bundle);
            }

            m_LoadedCallbackDict.Clear();

            STryUnLoadByName(m_BundleName);
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

            STryUnLoadByName(m_BundleName);
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

        public AssetBundle GetAssetBundle()
        {
            return m_Bundle;
        }

        public LoadState GetLoadState()
        {
            return m_LoadState;
        }

        private static readonly string s_ManifestBundleName = "AssetBundles";
        private static readonly string s_ManifestAssetName = "AssetBundleManifest";
        private static AssetBundleManifest s_ManifestAsset = null;
        private static int s_ManifestBundleIndex = -1;


        private static readonly ObjectPool<FullBundleBaseLoader> s_LoaderPool =
            new ObjectPool<FullBundleBaseLoader>(
                (loader) => { loader.OnReuse(); },
                (loader) => { loader.OnRecycle(); });

        private static readonly Action<bool, AssetBundle> s_DefaultLoadedCallback = (isOk, bundle) => { };

        private static readonly Dictionary<string, FullBundleBaseLoader> s_NameToLoader =
            new Dictionary<string, FullBundleBaseLoader>();

        private static readonly Dictionary<int, FullBundleBaseLoader> s_ResIndexToLoader =
            new Dictionary<int, FullBundleBaseLoader>();

        public static void SInitBundleManifest()
        {
            if (s_ManifestBundleIndex != -1)
            {
                SingleBundleBaseLoader.SUnLoad(s_ManifestBundleIndex);
                s_ManifestBundleIndex = -1;
                s_ManifestAsset = null;
            }

            s_ManifestBundleIndex = SingleBundleBaseLoader.SLoadSync(s_ManifestBundleName, null);
            var loader = SingleBundleBaseLoader.SGetLoader(s_ManifestBundleIndex);
            var bundle = loader.GetAssetBundle();
            s_ManifestAsset = bundle.LoadAsset<AssetBundleManifest>(s_ManifestAssetName);
        }

        public static int SLoadAsync(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            FullBundleBaseLoader baseLoader;
            s_NameToLoader.TryGetValue(bundleName, out baseLoader);
            if (baseLoader == null)
            {
                baseLoader = s_LoaderPool.Get();
                baseLoader.Init(bundleName);
                s_NameToLoader.Add(bundleName, baseLoader);
            }

            var resIndex = baseLoader.InternalLoadAsync(loadedAction);
            s_ResIndexToLoader.Add(resIndex, baseLoader);

            return resIndex;
        }

        public static int SLoadSync(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            FullBundleBaseLoader baseLoader;
            s_NameToLoader.TryGetValue(bundleName, out baseLoader);
            if (baseLoader == null)
            {
                baseLoader = s_LoaderPool.Get();
                baseLoader.Init(bundleName);
                s_NameToLoader.Add(bundleName, baseLoader);
            }

            var resIndex = baseLoader.InternalLoadSync(loadedAction);
            s_ResIndexToLoader.Add(resIndex, baseLoader);

            return resIndex;
        }

        public static FullBundleBaseLoader SGetLoader(int resouceIndex)
        {
            FullBundleBaseLoader baseLoader = null;
            s_ResIndexToLoader.TryGetValue(resouceIndex, out baseLoader);
            return baseLoader;
        }

        public static FullBundleBaseLoader SGetLoader(string bundleName)
        {
            FullBundleBaseLoader baseLoader = null;
            s_NameToLoader.TryGetValue(bundleName, out baseLoader);
            return baseLoader;
        }

        public static void SUnLoad(int resouceIndex)
        {
            FullBundleBaseLoader baseLoader;
            s_ResIndexToLoader.TryGetValue(resouceIndex, out baseLoader);
            if (baseLoader != null)
            {
                baseLoader.InternalUnload(resouceIndex);
            }
        }

        private static void STryUnLoadByName(string bundleName)
        {
            FullBundleBaseLoader baseLoader;
            s_NameToLoader.TryGetValue(bundleName, out baseLoader);
            if (baseLoader != null && baseLoader.CanRealUnload())
            {
                s_NameToLoader.Remove(bundleName);
                s_LoaderPool.Release(baseLoader);
            }
        }
    }
}