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
        public AssetRefCounter AssetRefCount;

        public ResourceManager(AssetRefCounter assetRefC)
        {
            AssetRefCount = new AssetRefCounter();
        }
    }
}
