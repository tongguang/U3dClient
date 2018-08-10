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


        public void Init(string name)
        {
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
            LoadRequest = null;
        }

        public void SetAssetRequest(AssetBundleRequest assetRequest)
        {
            State = LoadState.Loading;
            LoadRequest = assetRequest;
        }

        public void SetAsset(System.Object asset)
        {
            State = LoadState.Loaded;
            Asset = asset;
            LoadRequest = null;
        }
    }
}