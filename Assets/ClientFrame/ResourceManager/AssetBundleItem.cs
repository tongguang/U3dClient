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
        public Dictionary<long, string> AssetRefToAssetName = new Dictionary<long, string>();
        public Dictionary<long, string> DependAssetRef;
        public AssetBundleItem(string name)
        {
            State = LoadState.Init;
            Name = name;
        }

        #region 引用数据
        public void AddAssetRef(long requestIndex, string assetName)
        {
            if (!AssetNameToAssetItem.ContainsKey(assetName))
            {
                AssetNameToAssetItem.Add(assetName, new AssetBundleAssetItem(assetName));
            }
            AssetRefToAssetName.Add(requestIndex, assetName);
        }

        public void RemoveAssetRef(long requestIndex)
        {
            if (AssetRefToAssetName.ContainsKey(requestIndex))
            {
                AssetRefToAssetName.Remove(requestIndex);
            }
        }

        public void SetDependAssetRef(Dictionary<long, string> dependAssetRef)
        {
            DependAssetRef = dependAssetRef;
        }
        #endregion

        #region 加载

        public bool TryLoadBundleSync()
        {
            if (State == LoadState.Init)
            {
                State = LoadState.Loading;
                var bundlePath = PathTool.GetBundlePath(Name);
                if (bundlePath != "")
                {
                    var request = AssetBundle.LoadFromFileAsync(bundlePath);
                    SetAssetBundleCreateRequest(request);
                }
                else
                {
                    Debug.LogWarning(string.Format("加载不存在的AB {0}", Name));
                    return false;
                }
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
        }

        public bool TryLoadAssetSync(string assetName)
        {
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

        #endregion

        #region 卸载

        public bool TryUnLoadBundle()
        {
            if (State == LoadState.Loading)
            {
                return false;
            }
            if (AssetRefToAssetName.Count > 0)
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