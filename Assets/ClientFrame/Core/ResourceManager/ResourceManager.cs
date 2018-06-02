using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public enum LoadState
    {
        Init,
        Loading,
        Loaded,
    }
    public class ResourceManager
    {
        public AssetRefCounter RefCounter;
        public AssetBundleLoader BundleLoader;

        public ResourceManager()
        {
            RefCounter = new AssetRefCounter();
            BundleLoader = new AssetBundleLoader();
        }

        public void Awake()
        {
            BundleLoader.Init();
        }
    }
}
