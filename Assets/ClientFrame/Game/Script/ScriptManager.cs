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

        private Dictionary<string, LuaFileBytes> m_LuaFileBytesDict;
        public LuaRunner MainLuaRunner;

        public void SetLuaFileBytesDict(Dictionary<string, LuaFileBytes> luaFileBytesDict)
        {
            m_LuaFileBytesDict = luaFileBytesDict;
        }

        public void InitMainLuaRunner()
        {
            MainLuaRunner = new LuaRunner();
            MainLuaRunner.Init((ref string filename) =>
            {
                LuaFileBytes fileBytes;
                m_LuaFileBytesDict.TryGetValue(filename, out fileBytes);
                return fileBytes?.GetBytes();
            });
        }

        public void ReleaseMainLuaRunner()
        {
            if (MainLuaRunner != null)
            {
                MainLuaRunner.Release();
                MainLuaRunner = null;
            }
        }
    }
}

