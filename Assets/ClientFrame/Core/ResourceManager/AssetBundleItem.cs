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
        public Dictionary<string, AssetBundleAssetItem> AssetNameToAssetItem;
        public List<long> DependAssetRef;

        public void Init(string name)
        {
            AssetNameToAssetItem = new Dictionary<string, AssetBundleAssetItem>();
            Name = name;
            State = LoadState.Init;
        }

        public void OnReuse()
        {
        }

        public void OnRecycle()
        {
            Name = "";
            State = LoadState.Init;
            Bundle = null;
            LoadRequest = null;
            AssetNameToAssetItem = null;
            DependAssetRef = null;
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
                LoadRequest = request;
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
                var item = new AssetBundleAssetItem();
                item.Init(assetName);
                AssetNameToAssetItem.Add(assetName, item);
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
                var item = new AssetBundleAssetItem();
                item.Init(assetName);
                AssetNameToAssetItem.Add(assetName, item);
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
            if (GameCenter.ResourceMgr.RefCounter.GetBundleRefNum(Name) > 0)
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