using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLua;

namespace U3dClient.Frame
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
        public static MainLuaRunner s_MainMainLuaRunner;

        public static void SetLuaFileBytesDict(Dictionary<string, LuaFileBytes> luaFileBytesDict)
        {
            s_LuaFileBytesDict = luaFileBytesDict;
        }

        public static void InitMainLuaRunner()
        {
            s_MainMainLuaRunner = new MainLuaRunner();
            s_MainMainLuaRunner.Init((ref string filename) =>
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
            if (s_MainMainLuaRunner != null)
            {
                s_MainMainLuaRunner.Release();
                s_MainMainLuaRunner = null;
            }
        }

        public static void UpdateMainLuaRunner()
        {
            if (s_MainMainLuaRunner != null)
            {
                s_MainMainLuaRunner.Update();
            }
        }
    }
}

