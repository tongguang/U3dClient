using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public class FullBundleLoader : BaseLoader
    {
        #region PublicVal

        public bool IsComplate => m_LoadState == LoadState.Complete;

        #endregion

        #region PrivateVal

        private string m_BundleName;
        private LoadState m_LoadState = LoadState.Init;
        private AssetBundle m_Bundle;
        private int m_BundleIndex = -1;
        private readonly List<int> m_DependBundleIndexList = new List<int>();
        private readonly HashSet<int> m_ResouceIndexSet = new HashSet<int>();

        private readonly Dictionary<int, Action<bool, AssetBundle>> m_LoadedCallbackDict =
            new Dictionary<int, Action<bool, AssetBundle>>();

        #endregion

        #region BaseLoader

        protected override void OnReuse()
        {
            ResetData();
        }

        protected override void OnRecycle()
        {
            if (m_BundleIndex != -1) SingleBundleLoader.UnLoad(m_BundleIndex);

            foreach (var dependIndex in m_DependBundleIndexList) SingleBundleLoader.UnLoad(dependIndex);
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

        protected override IEnumerator LoadFuncEnumerator()
        {
            if (m_LoadState != LoadState.WaitLoad) yield break;
            m_LoadState = LoadState.Loading;
            if (s_ManifestAsset)
            {
                var depends = s_ManifestAsset.GetAllDependencies(m_BundleName);
                foreach (var depend in depends)
                {
                    var resIndex = SingleBundleLoader.LoadAsync(depend, null);
                    m_DependBundleIndexList.Add(resIndex);
                }
            }

            SingleBundleLoader bundleLoader;
            foreach (var resIndex in m_DependBundleIndexList)
            {
                bundleLoader = SingleBundleLoader.GetLoader(resIndex);
                while (!bundleLoader.IsComplate) yield return null;
            }

            m_BundleIndex = SingleBundleLoader.LoadAsync(m_BundleName, null);
            bundleLoader = SingleBundleLoader.GetLoader(m_BundleIndex);
            while (!bundleLoader.IsComplate) yield return null;

            m_Bundle = bundleLoader.GetAssetBundle();
            m_LoadState = LoadState.Complete;

            foreach (var action in m_LoadedCallbackDict)
            {
                var callback = action.Value;
                callback(m_Bundle != null, m_Bundle);
            }

            m_LoadedCallbackDict.Clear();

            TryUnLoadByName(m_BundleName);
        }

        #endregion

        #region PrivateFunc

        private void Init(string bundleName)
        {
            m_BundleName = bundleName;
        }

        private int InternalLoadAsync(Action<bool, AssetBundle> loadedAction)
        {
            if (loadedAction == null) loadedAction = s_DefaultLoadedCallback;

            var index = ResourceManager.GetNewResourceIndex();
            m_ResouceIndexSet.Add(index);
            if (m_LoadState == LoadState.Init)
            {
                m_LoadedCallbackDict.Add(index, loadedAction);
                m_LoadState = LoadState.WaitLoad;
                GameCenter.s_StartCoroutineFunc(LoadFuncEnumerator());
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
            if (loadedAction == null) loadedAction = s_DefaultLoadedCallback;

            var index = ResourceManager.GetNewResourceIndex();
            m_ResouceIndexSet.Add(index);
            if (m_LoadState == LoadState.Init || m_LoadState == LoadState.WaitLoad)
            {
                var depends = s_ManifestAsset.GetAllDependencies(m_BundleName);
                foreach (var depend in depends)
                {
                    var resIndex = SingleBundleLoader.LoadSync(depend, null);
                    m_DependBundleIndexList.Add(resIndex);
                }

                m_BundleIndex = SingleBundleLoader.LoadSync(m_BundleName, null);
                var bundleLoader = SingleBundleLoader.GetLoader(m_BundleIndex);
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

        private void InternalUnload(int resourceIndex)
        {
            if (m_LoadedCallbackDict.ContainsKey(resourceIndex)) m_LoadedCallbackDict.Remove(resourceIndex);

            if (m_ResouceIndexSet.Contains(resourceIndex)) m_ResouceIndexSet.Remove(resourceIndex);

            TryUnLoadByName(m_BundleName);
        }

        private bool CanRealUnload()
        {
            if (m_LoadState == LoadState.Init || m_LoadState == LoadState.WaitLoad) return true;

            if (m_LoadState == LoadState.Loading) return false;

            if (m_ResouceIndexSet.Count > 0) return false;

            return true;
        }

        #endregion

        #region PublicFunc

        public AssetBundle GetAssetBundle()
        {
            return m_Bundle;
        }

        public LoadState GetLoadState()
        {
            return m_LoadState;
        }

        #endregion

        #region PrivateStaticVal

        private static readonly string s_ManifestBundleName = "AssetBundles";
        private static readonly string s_ManifestAssetName = "AssetBundleManifest";
        private static AssetBundleManifest s_ManifestAsset;
        private static int s_ManifestBundleIndex = -1;


        private static readonly ObjectPool<FullBundleLoader> s_LoaderPool =
            new ObjectPool<FullBundleLoader>(
                loader => { loader.OnReuse(); },
                loader => { loader.OnRecycle(); });

        private static readonly Action<bool, AssetBundle> s_DefaultLoadedCallback = (isOk, bundle) => { };

        private static readonly Dictionary<string, FullBundleLoader> s_NameToLoader =
            new Dictionary<string, FullBundleLoader>();

        private static readonly Dictionary<int, FullBundleLoader> s_ResIndexToLoader =
            new Dictionary<int, FullBundleLoader>();

        #endregion

        #region PrivateStaticFunc

        private static void TryUnLoadByName(string bundleName)
        {
            FullBundleLoader loader;
            s_NameToLoader.TryGetValue(bundleName, out loader);
            if (loader != null && loader.CanRealUnload())
            {
                s_NameToLoader.Remove(bundleName);
                s_LoaderPool.Release(loader);
            }
        }

        #endregion

        #region PublicStaticFunc

        public static void InitBundleManifest()
        {
            if (s_ManifestBundleIndex != -1)
            {
                SingleBundleLoader.UnLoad(s_ManifestBundleIndex);
                s_ManifestBundleIndex = -1;
                s_ManifestAsset = null;
            }

            s_ManifestBundleIndex = SingleBundleLoader.LoadSync(s_ManifestBundleName, null);
            var loader = SingleBundleLoader.GetLoader(s_ManifestBundleIndex);
            var bundle = loader.GetAssetBundle();
            s_ManifestAsset = bundle.LoadAsset<AssetBundleManifest>(s_ManifestAssetName);
        }

        public static int LoadAsync(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            FullBundleLoader loader;
            s_NameToLoader.TryGetValue(bundleName, out loader);
            if (loader == null)
            {
                loader = s_LoaderPool.Get();
                loader.Init(bundleName);
                s_NameToLoader.Add(bundleName, loader);
            }

            var resIndex = loader.InternalLoadAsync(loadedAction);
            s_ResIndexToLoader.Add(resIndex, loader);

            return resIndex;
        }

        public static int LoadSync(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            FullBundleLoader loader;
            s_NameToLoader.TryGetValue(bundleName, out loader);
            if (loader == null)
            {
                loader = s_LoaderPool.Get();
                loader.Init(bundleName);
                s_NameToLoader.Add(bundleName, loader);
            }

            var resIndex = loader.InternalLoadSync(loadedAction);
            s_ResIndexToLoader.Add(resIndex, loader);

            return resIndex;
        }

        public static FullBundleLoader GetLoader(int resourceIndex)
        {
            FullBundleLoader loader = null;
            s_ResIndexToLoader.TryGetValue(resourceIndex, out loader);
            return loader;
        }

        public static FullBundleLoader GetLoader(string bundleName)
        {
            FullBundleLoader loader = null;
            s_NameToLoader.TryGetValue(bundleName, out loader);
            return loader;
        }

        public static void UnLoad(int resourceIndex)
        {
            FullBundleLoader loader;
            s_ResIndexToLoader.TryGetValue(resourceIndex, out loader);
            if (loader != null)
            {
                s_ResIndexToLoader.Remove(resourceIndex);
                loader.InternalUnload(resourceIndex);
            }
        }

        #endregion
    }
}