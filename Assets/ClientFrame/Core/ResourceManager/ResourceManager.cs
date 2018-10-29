using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient.ResourceMgr
{
    public static class ResourceManager
    {
        private static int s_ResourceIndex = 0;

        public static void Awake()
        {
            FullBundleBaseLoader.SInitBundleManifest();
        }

        public static void Start()
        {

        }

        public static void Update()
        {
        }

        public static void OnApplicationFocus(bool hasFocus)
        {

        }

        public static void OnApplicationPause(bool pauseStatus)
        {

        }

        public static void OnDestroy()
        {

        }

        public static void OnApplicationQuit()
        {

        }

        public static int GetNewResourceIndex()
        {
            return s_ResourceIndex++;
        }
    }
}
