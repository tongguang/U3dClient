using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLua;

namespace U3dClient.Game
{
    public class ScriptManager
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

        public void Release()
        {
            ReleaseMainLuaRunner();
        }

        public static Dictionary<string, LuaFileBytes> s_LuaFileBytesDict;
        public static MainLuaRunner s_MainMainLuaRunner;

        public void SetLuaFileBytesDict(Dictionary<string, LuaFileBytes> luaFileBytesDict)
        {
            s_LuaFileBytesDict = luaFileBytesDict;
        }

        public void InitMainLuaRunner()
        {
            s_MainMainLuaRunner = new MainLuaRunner();
            s_MainMainLuaRunner.Init((ref string filename) =>
            {
                LuaFileBytes fileBytes;
                s_LuaFileBytesDict.TryGetValue(filename, out fileBytes);
                return fileBytes?.GetBytes();
            });
        }

        public void ReleaseMainLuaRunner()
        {
            if (s_MainMainLuaRunner != null)
            {
                s_MainMainLuaRunner.Release();
                s_MainMainLuaRunner = null;
            }
        }

        public void UpdateMainLuaRunner()
        {
            s_MainMainLuaRunner?.Update();
        }
    }
}

