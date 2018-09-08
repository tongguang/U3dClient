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

namespace U3dClient.ResourceMgr
{
    public static class ResourceManager
    {
        private static int ResourceIndex = 0;

        public static void Awake()
        {

        }

        public static int GetNewResourceIndex()
        {
            return ResourceIndex++;
        }
    }
}
