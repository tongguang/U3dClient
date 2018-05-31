using System;
using System.Collections.Generic;
using U3dClient;
using UnityEngine;

namespace U3dClient
{
    public class AssetBundleItem
    {
        public string Name { private set; get; }
        public LoadState State { private set; get; }
        public AssetBundle Bundle { private set; get; }
        public AssetBundleCreateRequest LoadRequest { private set; get; }
        public Dictionary<string, AssetBundleAssetItem> AssetNameToAssetItem = new Dictionary<string, AssetBundleAssetItem>();
        public List<long> DependAssetRef;
        public AssetBundleItem(string name)
        {
            State = LoadState.Init;
            Name = name;
        }

        public void SetDependAssetRef(List<long> dependAssetRef)
        {
            DependAssetRef = dependAssetRef;
        }

        #region 加载

        public bool TryLoadBundleAsync()
        {
            if (State == LoadState.Init)
            {
                State = LoadState.Loading;
                var bundlePath = FileTool.GetBundlePath(Name);
                var request = AssetBundle.LoadFromFileAsync(bundlePath);
                SetAssetBundleCreateRequest(request);
            }
            return true;
        }

        public bool TryLoadBundle()
        {
            if (State == LoadState.Init)
            {
                var bundlePath = FileTool.GetBundlePath(Name);
                var bundle = AssetBundle.LoadFromFile(bundlePath);
                SetAssetBundle(bundle);
            }
            return true;
        }

        public void SetAssetBundleCreateRequest(AssetBundleCreateRequest bundleRequest)
        {
            State = LoadState.Loading;
            LoadRequest = bundleRequest;
        }

        public void SetAssetBundle(AssetBundle assetBundle)
        {
            State = LoadState.Loaded;
            Bundle = assetBundle;
            LoadRequest = null;
            if (Bundle == null)
            {
                Debug.LogWarning(string.Format("加载不存在的AB {0}", Name));
            }
        }

        public bool TryLoadAssetAsync(string assetName)
        {
            if (!AssetNameToAssetItem.ContainsKey(assetName))
            {
                AssetNameToAssetItem.Add(assetName, new AssetBundleAssetItem(assetName));
            }
            var assetItem = AssetNameToAssetItem[assetName];
            if (assetItem.State == LoadState.Init)
            {
                if (assetName == "")
                {
                    assetItem.SetAsset(null);
                }
                else
                {
                    if (Bundle)
                    {
                        var assetRequest = Bundle.LoadAssetAsync(assetName);
                        assetItem.SetAssetRequest(assetRequest);
                    }
                    else
                    {
                        assetItem.SetAsset(null);
                    }
                }
            }
            
            return true;
        }

        public bool TryLoadAsset(string assetName)
        {
            if (!AssetNameToAssetItem.ContainsKey(assetName))
            {
                AssetNameToAssetItem.Add(assetName, new AssetBundleAssetItem(assetName));
            }
            var assetItem = AssetNameToAssetItem[assetName];
            if (assetItem.State == LoadState.Init)
            {
                if (assetName == "")
                {
                    assetItem.SetAsset(null);
                }
                else
                {
                    if (Bundle)
                    {
                        var asset = Bundle.LoadAsset(assetName);
                        assetItem.SetAsset(asset);
                    }
                    else
                    {
                        assetItem.SetAsset(null);
                    }
                }
            }

            return true;
        }

        #endregion

        #region 卸载

        public bool TryUnLoadBundle()
        {
            if (State == LoadState.Loading)
            {
                return false;
            }
            if (GameRoot.Instance.ResourceMgr.RefCounter.GetBundleRefNum(Name) > 0)
            {
                return false;
            }
            foreach (var item in AssetNameToAssetItem)
            {
                if (item.Value.State == LoadState.Loading)
                {
                    return false;
                }
            }

            if (Bundle)
            {
                Bundle.Unload(true);
            }

            return true;
        }

        #endregion
    }
}