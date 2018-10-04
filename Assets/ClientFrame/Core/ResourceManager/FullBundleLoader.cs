using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace U3dClient.ResourceMgr
{
    public class FullBundleLoader : LoaderBase
    {
        private string m_BundleName = null;
        private LoadState m_LoadState = LoadState.Init;
        private AssetBundle m_Bundle = null;
        private int m_BundleIndex = -1;
        private List<int> m_DependBundleIndexList = new List<int>();
        private readonly HashSet<int> m_ResouceIndexSet = new HashSet<int>();
        private readonly Dictionary<int, Action<bool, AssetBundle>> m_LoadedCallbackDict =
            new Dictionary<int, Action<bool, AssetBundle>>();

        protected override void OnReuse()
        {
            ResetData();
        }

        protected override void OnRecycle()
        {
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
                    var resIndex = SingleBundleLoader.SLoadSync(depend, null);
                    m_DependBundleIndexList.Add(resIndex);
                }
                m_BundleIndex = SingleBundleLoader.SLoadSync(m_BundleName, null);
                var bundleLoader = SingleBundleLoader.SGetLoader(m_BundleIndex);
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
            m_LoadState = LoadState.Loading;
            if (s_ManifestAsset)
            {
                var depends = s_ManifestAsset.GetAllDependencies(m_BundleName);
                foreach (var depend in depends)
                {
                    var resIndex = SingleBundleLoader.SLoadAsync(depend, null);
                    m_DependBundleIndexList.Add(resIndex);
                }
            }

            SingleBundleLoader bundleLoader;
            foreach (var resIndex in m_DependBundleIndexList)
            {
                bundleLoader = SingleBundleLoader.SGetLoader(resIndex);
                if (!bundleLoader.IsComplate)
                {
                    yield return null;
                }
            }
            m_BundleIndex = SingleBundleLoader.SLoadAsync(m_BundleName, null);
            bundleLoader = SingleBundleLoader.SGetLoader(m_BundleIndex);
            if (!bundleLoader.IsComplate)
            {
                yield return null;
            }

            m_Bundle = bundleLoader.GetAssetBundle();
            m_LoadState = LoadState.Complete;

            foreach (var action in m_LoadedCallbackDict)
            {
                var callback = action.Value;
                callback(m_Bundle != null, m_Bundle);
            }
            m_LoadedCallbackDict.Clear();
        }

        private static readonly string s_ManifestBundleName = "AssetBundles";
        private static readonly string s_ManifestAssetName = "AssetBundleManifest";
        private static AssetBundleManifest s_ManifestAsset = null;
        private static int s_ManifestBundleIndex = -1;
        private static readonly Action<bool, AssetBundle> s_DefaultLoadedCallback = (isOk, bundle) => { };


        public static void InitBundleManifest()
        {
            if (s_ManifestBundleIndex != -1)
            {
                SingleBundleLoader.SUnLoad(s_ManifestBundleIndex);
                s_ManifestBundleIndex = -1;
                s_ManifestAsset = null;
            }
            s_ManifestBundleIndex = SingleBundleLoader.SLoadSync(s_ManifestBundleName, null);
            var loader = SingleBundleLoader.SGetLoader(s_ManifestBundleIndex);
            var bundle = loader.GetAssetBundle();
            s_ManifestAsset = bundle.LoadAsset<AssetBundleManifest>(s_ManifestAssetName);
        }
    }
}