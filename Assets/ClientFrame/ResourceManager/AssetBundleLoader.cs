using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace U3dClient
{
    public class AssetBundleLoader : IResourceLoader
    {
        private Dictionary<string, AssetBundleItem> m_BundleNameToItem;
        private AssetBundleManifest m_BundlesManifest;
        private long m_BundlesManifestRef;
        private string m_ManifestBundleName = "AssetBundles";
        private string m_ManifestAssetName = "AssetBundleManifest";
        private bool m_Inited = false;

        public AssetBundleLoader()
        {
            m_BundleNameToItem = new Dictionary<string, AssetBundleItem>();
        }

        private string[] GetDependABNames(string abName)
        {
            if (!m_BundlesManifest)
            {
                return null;
            }
            string[] dependencies = m_BundlesManifest.GetAllDependencies(abName);
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
            var dependRefs = abItem.DependAssetRef;
		    Debug.Log("111111111111111");                                                                                           
            if (isLoadDepend && dependRefs != null)
            {
                foreach (var dependItem in dependRefs)
                {
                    var dependRefData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(dependItem);
                    var loadDependAssetEnu = LoadAssetSyncEnumerator<Object>(dependItem, dependRefData.BundleName, "", null, false);
                    while (loadDependAssetEnu.MoveNext())
                    {
                        yield return loadDependAssetEnu.Current;
                    }
                }
            }
		    Debug.Log("222222222222");
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
		    Debug.Log("3333333333");
            abItem.TryLoadAssetSync(assetName);
            var assetItem = abItem.AssetNameToAssetItem[assetName];
            if (assetItem.State == LoadState.Loading)
            {
                var assetRequest = assetItem.LoadRequest;
                while (!assetRequest.isDone)
                {
                    yield return null;
                }
                assetItem.SetAsset(assetRequest.asset);
                if (TryUnLoadABItemByName(abName))
                {
                    yield break;
                }
            }
		    Debug.Log("44444444444");

            {
                refData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(refRequest);
                if (refData != null && refData.RefNums > 0 && loadedAction != null)
                {
                    loadedAction(assetItem.Asset as T);
                }
            }
            yield break;
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

        // public void Init()
        // {

        // }

        public void InitSync(Action loadedAction)
        {
            m_BundlesManifestRef = LoadAssetSync<AssetBundleManifest>(m_ManifestBundleName, m_ManifestAssetName, 
            asset=>
            {
                m_BundlesManifest = asset;
                m_Inited = true;
                loadedAction();
            }, false);
        }
    }
}