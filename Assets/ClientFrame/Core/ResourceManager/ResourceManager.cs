﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient.ResourceMgr
{
    public static class ResourceManager
    {
        private static int ResourceIndex = 0;

        public static void Awake()
        {
            FullBundleLoader.SInitBundleManifest();
        }

        public static int GetNewResourceIndex()
        {
            return ResourceIndex++;
        }
    }
}
