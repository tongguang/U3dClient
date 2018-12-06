using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace U3dClient.Frame
{
    public class LuaFileRef : SerializedScriptableObject
    {
        public Dictionary<string, TextAsset> AssetsRefDict = new Dictionary<string, TextAsset>();
    }
}

