using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient.ResourceMgr
{
    public static class ResourceManager
    {
        private static int s_ResourceIndex = 0;

        public static int GetNewResourceIndex()
        {
            return s_ResourceIndex++;
        }

        public static void Init()
        {
            FullBundleLoader.InitBundleManifest();
        }

    }
}
