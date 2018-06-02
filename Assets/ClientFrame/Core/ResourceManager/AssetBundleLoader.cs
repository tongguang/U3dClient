using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
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

        private IEnumerator LoadAssetAsyncEnumerator<T>(long refRequest, string abName, string assetName, Action<T> loadedAction, bool isLoadDepend) where T : Object
        {
            var refData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(refRequest);
            if (refData == null)
            {
                yield break;
            }
            var abItem = m_BundleNameToItem[refData.BundleName];
            var dependRefs = abItem.DependAssetRef;
            if (isLoadDepend && dependRefs != null)
            {
                foreach (var dependItem in dependRefs)
                {
                    var dependRefData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(dependItem);
                    var loadDependAssetEnu = LoadAssetAsyncEnumerator<Object>(dependItem, dependRefData.BundleName, "", null, false);
                    while (loadDependAssetEnu.MoveNext())
                    {
                        yield return loadDependAssetEnu.Current;
                    }
                }
            }
            abItem.TryLoadBundleAsync();
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
            abItem.TryLoadAssetAsync(assetName);
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

            {
                refData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(refRequest);
                if (refData != null && refData.RefNums > 0 && loadedAction != null)
                {
                    loadedAction(assetItem.Asset as T);
                }
            }
        }

        // 同一个资源不支持同步和异步加载同时使用
        public long LoadAsset<T>(string abName, string assetName, Action<T> loadedAction, bool isLoadDepend) where T : Object
        {
            var nowRef = AddAssetRef(abName, assetName);
            var refData = GameRoot.Instance.ResourceMgr.RefCounter.GetRefData(nowRef);
            var abItem = m_BundleNameToItem[refData.BundleName];
            if (isLoadDepend && abItem.DependAssetRef == null)
            {
                var dependAbNames = GetDependABNames(abName);
                if (dependAbNames != null)
                {
                    List<long> dependRefs = new List<long>();
                    foreach (var dependAbName in dependAbNames)
                    {
                        var dependRef = LoadAsset<Object>(dependAbName, "", null, false);
                        dependRefs.Add(dependRef);
                    }
                    abItem.SetDependAssetRef(dependRefs);
                }
            }
            abItem.TryLoadBundle();
            abItem.TryLoadAsset(assetName);
            var assetItem = abItem.AssetNameToAssetItem[assetName];
            if (loadedAction != null)
            {
                loadedAction(assetItem.Asset as T);
            }
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

        public long LoadAssetAsync<T>(string abName, string assetName, Action<T> loadedAction, bool isLoadDepend) where T : Object
        {
            var nowRef = AddAssetRef(abName, assetName);
            if (isLoadDepend)
            {
                AddDependAssetRef(abName);
            }
            MainThreadDispatcher.StartCoroutine(LoadAssetAsyncEnumerator<T>(nowRef, abName, assetName, loadedAction, isLoadDepend));
            return nowRef;
        }

        public void InitBundlesManifest()
        {
            m_BundlesManifestRef = LoadAsset<AssetBundleManifest>(m_ManifestBundleName, m_ManifestAssetName,
                asset =>
                {
                    m_BundlesManifest = asset;
                    m_Inited = true;
                }, false);
        }

        public void Init()
        {
            InitBundlesManifest();
        }

        public void ReInitBundlesManifest()
        {
            if (m_BundlesManifest)
            {
                m_BundlesManifest = null;
                UnLoadAsset(m_BundlesManifestRef);
                InitBundlesManifest();
            }
        }

        public void InitAsync(Action loadedAction)
        {
            m_BundlesManifestRef = LoadAssetAsync<AssetBundleManifest>(m_ManifestBundleName, m_ManifestAssetName, 
            asset=>
            {
                m_BundlesManifest = asset;
                m_Inited = true;
                loadedAction();
            }, false);
        }
    }
}