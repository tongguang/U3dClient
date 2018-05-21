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

        public ResourceManager()
        {
            RefCounter = new AssetRefCounter();
        }
    }
}
