using U3dClient;
using UnityEngine;

namespace U3dClient
{
    public class AssetBundleAssetItem
    {
        public string Name;
        public LoadState State { private set; get; }
        public AssetBundleRequest LoadRequest { private set; get; }
        public System.Object Asset { private set; get; }

        public AssetBundleAssetItem(string name)
        {
            Name = name;
            State = LoadState.Init;
        }

        public void SetAssetRequest(AssetBundleRequest assetRequest)
        {
            State = LoadState.Loading;
            LoadRequest = LoadRequest;
        }

        public void SetAsset(System.Object asset)
        {
            State = LoadState.Loaded;
            Asset = asset;
            LoadRequest = null;
        }
    }
}