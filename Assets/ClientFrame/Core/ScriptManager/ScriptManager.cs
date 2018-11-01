using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLua;

namespace U3dClient.ScriptMgr
{
    public static class ScriptManager
    {
        public static Dictionary<string, TextAsset> s_LuaFileAssetDict;
        public static MainLuaRunner SMainMainLuaRunner;

        public static void SetLuaFileAssetDict(Dictionary<string, TextAsset> luaFileAssetDict)
        {
            s_LuaFileAssetDict = luaFileAssetDict;
        }

        public static void InitMainLuaRunner()
        {
            SMainMainLuaRunner = new MainLuaRunner();
            SMainMainLuaRunner.Init((ref string filename) =>
            {
                TextAsset textAsset;
                s_LuaFileAssetDict.TryGetValue(filename, out textAsset);
                if (textAsset)
                {
                    Debug.Log(textAsset.text);
                    return Encoding.UTF8.GetBytes(textAsset.text);
                }
                return null;
            });
        }

        public static void ReleaseMainLuaRunner()
        {
            if (SMainMainLuaRunner != null)
            {
                SMainMainLuaRunner.Release();
                SMainMainLuaRunner = null;
            }
        }

        public static void UpdateMainLuaRunner()
        {
            if (SMainMainLuaRunner != null)
            {
                SMainMainLuaRunner.Update();
            }
        }
    }
}

