using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient.Frame
{
    public class ResourceManager
    {
        private bool m_IsInit = false;
        private static int s_ResourceIndex = 0;

        public static int GetNewResourceIndex()
        {
            return s_ResourceIndex++;
        }

        public void Init()
        {
            m_IsInit = true;
        }

        public void Release()
        {

        }

        public void InitBundleManifest()
        {
            FullBundleLoader.InitBundleManifest();
        }

    }
}
