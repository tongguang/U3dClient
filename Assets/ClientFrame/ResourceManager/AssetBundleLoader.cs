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
        private AssetBundleManifest m_AssetBundlesManifest;
        private long m_AssetBundlesManifestRef;

        private string[] GetDependABNames(string abName)
        {
            if (!m_AssetBundlesManifest)
            {
                return null;
            }
            string[] dependencies = m_AssetBundlesManifest.GetAllDependencies(abName);
            return dependencies;
        }

        private long AddAssetRef(string abName, string assetName)
        {
            var nowRef = GameRoot.Instance.ResourceMgr.RefCounter.AddAssetRef(abName, assetName);
            if (!m_BundleNameToItem.ContainsKey(abName))
            {
                m_BundleNameToItem.Add(abName, new AssetBundleItem(abName));
            }

            return nowRef;
        }

        private void AddDependAssetRef(string abName)
        {
            var abItem = m_BundleNameToItem[abName];
            if (abItem.DependAssetRef == null)
            {
                var dependAbNames = GetDependABNames(abName);
                if (dependAbNames != null)
                {
                    List<long> dependRefs = new List<long>(); 
                    foreach (var dependAbName in dependAbNames)
                    {
                        var dependRef = AddAssetRef(dependAbName, "");
                        dependRefs.Add(dependRef);
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
                            UnLoadAsset(dependRef);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private IEnumerator LoadAssetSyncEnumerator<T>(long refRequest, string abName, string assetName, Action<T> loadedAction, bool isLoadDepend) where T : Object
        {
            var refData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(refRequest);
            if (refData == null)
            {
                yield break;
            }
            var abItem = m_BundleNameToItem[refData.BundleName];
            var assetItem = abItem.AssetNameToAssetItem[assetName];
            var dependRefs = abItem.DependAssetRef;
            if (isLoadDepend && dependRefs != null)
            {
                foreach (var dependItem in dependRefs)
                {
                    var dependRefData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(dependItem);
                    yield return LoadAssetSyncEnumerator<Object>(dependItem, dependRefData.BundleName, "", null, false);
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

            {
                refData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(refRequest);
                if (refData != null && refData.RefNums > 0 && loadedAction != null)
                {
                    loadedAction(assetItem.Asset as T);
                }
            }
            
        }

        public long LoadAssetSync<T>(string abName, string assetName, Action<T> loadedAction, bool isLoadDepend) where T : Object
        {
            var nowRef = AddAssetRef(abName, assetName);
            if (isLoadDepend)
            {
                AddDependAssetRef(abName);
            }
            GameRoot.Instance.StartCoroutine(LoadAssetSyncEnumerator<T>(nowRef, abName, assetName, loadedAction, isLoadDepend));
            return nowRef;
        }

        public void UnLoadAsset(long refRequest)
        {
            var refData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(refRequest);
            if (refData != null)
            {
                GameRoot.Instance.ResourceMgr.RefCounter.RemoveAssetRef(refRequest);
                var abItem = m_BundleNameToItem[refData.BundleName];
                TryUnLoadABItemByName(abItem.Name);
            }
        }
    }
}