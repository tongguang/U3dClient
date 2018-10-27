using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient.ScriptMgr
{
    public class LuaFileDesc : ScriptableObject
    {
        public List<String> LuaModuleName = new List<string>();
        public List<TextAsset> LuaFleAssets = new List<TextAsset>();
    }
}

