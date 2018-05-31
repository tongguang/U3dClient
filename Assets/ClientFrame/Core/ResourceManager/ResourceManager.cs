﻿using System;
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
        public AssetBundleLoader ResourceLoader;

        public ResourceManager()
        {
            RefCounter = new AssetRefCounter();
            ResourceLoader = new AssetBundleLoader();
        }

        public void InitResourceLoader(Action loadedAction)
        {
            ResourceLoader.InitAsync(loadedAction);
        }        
    }
}