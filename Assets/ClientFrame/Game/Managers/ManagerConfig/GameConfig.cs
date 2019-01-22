using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public class GameConfig
    {
        public enum AssetLoadModeEnum
        {
            EditMode,
            AssetBundleMode,
        }

        public AssetLoadModeEnum AssetLoadMode;
    }
} 
