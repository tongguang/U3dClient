using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLua;

namespace U3dClient.ScriptMgr
{
    public static class ScriptManager
    {
        public class LuaFileBytes
        {
            private byte[] m_Utf8Bytes = null;

            public void SetBytes(byte[] utf8Bytes)
            {
                m_Utf8Bytes = utf8Bytes;
            }

            public byte[] GetBytes()
            {
                return m_Utf8Bytes;
            }
        }

        public static Dictionary<string, LuaFileBytes> s_LuaFileBytesDict;
        public static MainLuaRunner SMainMainLuaRunner;

        public static void SetLuaFileBytesDict(Dictionary<string, LuaFileBytes> luaFileBytesDict)
        {
            s_LuaFileBytesDict = luaFileBytesDict;
        }

        public static void InitMainLuaRunner()
        {
            SMainMainLuaRunner = new MainLuaRunner();
            SMainMainLuaRunner.Init((ref string filename) =>
            {
                LuaFileBytes fileBytes;
                s_LuaFileBytesDict.TryGetValue(filename, out fileBytes);
                if (fileBytes!=null)
                {
                    return fileBytes.GetBytes();
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

