using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public class ResourceManager : IGameManager
    {
        public void Awake()
        {
        }

        public void Start()
        {
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void OnApplicationFocus(bool hasFocus)
        {
        }

        public void OnApplicationPause(bool pauseStatus)
        {
        }

        public void OnDestroy()
        {
        }

        public void OnApplicationQuit()
        {
        }

        private static int s_ResourceIndex = 0;

        public static int GetNewResourceIndex()
        {
            return s_ResourceIndex++;
        }

        public void InitBundleManifest()
        {
            FullBundleLoader.InitBundleManifest();
        }
    }
}
