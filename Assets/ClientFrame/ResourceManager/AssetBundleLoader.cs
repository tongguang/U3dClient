using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace U3dClient
{
    public class AssetBundleLoader : IResourceLoader
    {
        private long m_NowIndex = 0;
        private Dictionary<string, AssetBundleItem> m_BundleNameToItem;
        private Dictionary<long, AssetBundleItem> m_RefToABItem;
        private AssetBundleManifest m_AssetBundlesManifest;
        private long m_AssetBundlesManifestRef;

        private long GetNewRefIndex()
        {
            return m_NowIndex++;
        }

        private string[] GetDependABNames(string abName)
        {
            if (!m_AssetBundlesManifest)
            {
                return null;
            }
            string[] dependencies = m_AssetBundlesManifest.GetAllDependencies(abName);
            return dependencies;
        }

        private void AddAssetRef(long refRequest, string abName, string assetName)
        {
            if (!m_BundleNameToItem.ContainsKey(abName))
            {
                m_BundleNameToItem.Add(abName, new AssetBundleItem(abName));
            }

            {
                var item = m_BundleNameToItem[abName];
                item.AddAssetRef(refRequest, assetName);
                m_RefToABItem.Add(refRequest, item);
            }
        }

        private void AddDependAssetRef(string abName)
        {
            var abItem = m_BundleNameToItem[abName];
            if (abItem.DependAssetRef == null)
            {
                var dependAbNames = GetDependABNames(abName);
                if (dependAbNames != null)
                {
                    Dictionary<long, string> dependRefs = new Dictionary<long, string>(); 
                    foreach (var dependAbName in dependAbNames)
                    {
                        var dependRef = GetNewRefIndex();
                        AddAssetRef(dependRef, dependAbName, "");
                        dependRefs.Add(dependRef, dependAbName);
                    }
                    abItem.SetDependAssetRef(dependRefs);
                }
            }
        }

        private bool TryUnLoadABItemByName(string abName)
        {
            if (m_BundleNameToItem.ContainsKey(abName))
            {
                var abItem = m_BundleNameToItem[abName];
                var isSuccess = abItem.TryUnLoadBundle();
                if (isSuccess)
                {
                    m_BundleNameToItem.Remove(abName);
                    var dependRefs = abItem.DependAssetRef;
                    if (dependRefs != null)
                    {
                        foreach (var dependRef in dependRefs)
                        {
                            UnLoadAsset(dependRef.Key);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private IEnumerator LoadAssetSyncEnumerator<T>(long refRequest, string abName, string assetName, Action<T> loadedAction, bool isLoadDepend) where T : Object
        {
            if (!m_RefToABItem.ContainsKey(refRequest))
            {
                yield break;
            }
            var abItem = m_RefToABItem[refRequest];
            var assetItem = abItem.AssetNameToAssetItem[assetName];
            var dependRefs = abItem.DependAssetRef;
            if (isLoadDepend && dependRefs != null)
            {
                foreach (var dependItem in dependRefs)
                {
                    yield return LoadAssetSyncEnumerator<Object>(dependItem.Key, dependItem.Value, "", null, false);
                }
            }
            abItem.TryLoadBundleSync();
            if (abItem.State == LoadState.Loading)
            {
                var request = abItem.LoadRequest;
                while (!request.isDone)
                {
                    yield return null;
                }
                abItem.SetAssetBundle(request.assetBundle);
                if (TryUnLoadABItemByName(abName))
                {
                    yield break;
                }
            }
            abItem.TryLoadAssetSync(assetName);
            if (assetItem.State == LoadState.Loading)
            {
                var request = assetItem.LoadRequest;
                while (!request.isDone)
                {
                    yield return null;
                }
                assetItem.SetAsset(request.asset);
                if (TryUnLoadABItemByName(abName))
                {
                    yield break;
                }
            }

            if (m_RefToABItem.ContainsKey(refRequest) && loadedAction != null)
            {
                loadedAction(assetItem.Asset as T);
            }
        }

        public long LoadAssetSync<T>(string abName, string assetName, Action<T> loadedAction, bool isLoadDepend) where T : Object
        {
            var nowRef = GetNewRefIndex();
            AddAssetRef(nowRef, abName, assetName);
            if (isLoadDepend)
            {
                AddDependAssetRef(abName);
            }
            GameRoot.Instance.StartCoroutine(LoadAssetSyncEnumerator<T>(nowRef, abName, assetName, loadedAction, isLoadDepend));
            return nowRef;
        }

        public void UnLoadAsset(long refRequest)
        {
            if (m_RefToABItem.ContainsKey(refRequest))
            {
                var abItem = m_RefToABItem[refRequest];
                m_RefToABItem.Remove(refRequest);
                TryUnLoadABItemByName(abItem.Name);
            }
        }
    }
}