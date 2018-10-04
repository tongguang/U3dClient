using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient.ResourceMgr
{
    public class FullBundleLoader : LoaderBase
    {
        private string m_BundleName = null;
        private LoadState m_LoadState = LoadState.Init;
        private AssetBundle m_Bundle = null;
        private int m_BundleIndex = -1;
        private List<int> m_DependBundleIndexList = new List<int>();

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
        }

        private void Init(string bundleName)
        {
            m_BundleName = bundleName;
        }

        private int InternalLoadAsync(Action<bool, AssetBundle> loadedAction)
        {
            return 0;
        }

        private int InternalLoadSync(Action<bool, AssetBundle> loadedAction)
        {
            return 0;
        }

        protected override IEnumerator LoadFuncEnumerator()
        {
            throw new System.NotImplementedException();
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