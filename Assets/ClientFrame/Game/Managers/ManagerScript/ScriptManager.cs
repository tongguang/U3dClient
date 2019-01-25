using System.Collections.Generic;
using System.IO;
using System.Text;

namespace U3dClient
{
    public class ScriptManager : IGameManager
    {
        public class LuaFileBytes
        {
            private byte[] m_Utf8Bytes;

            public void SetBytes(byte[] utf8Bytes)
            {
                m_Utf8Bytes = utf8Bytes;
            }

            public byte[] GetBytes()
            {
                return m_Utf8Bytes;
            }
        }

        #region privateVal

        private Dictionary<string, LuaFileBytes> m_LuaFileBytesDict;

        #endregion

        #region PublicVal

        public LuaRunner MainLuaRunner;

        #endregion

        #region PublicFunc

        public void SetLuaFileBytesDict(Dictionary<string, LuaFileBytes> luaFileBytesDict)
        {
            m_LuaFileBytesDict = luaFileBytesDict;
        }

        public void InitMainLuaRunner()
        {
            MainLuaRunner = new LuaRunner();
            if (GameCenter.s_ConfigManager.GlobalGameConfig.LuaScriptLoadMode ==
                GameConfig.LuaScriptLoadModeEnum.RawFileMode)
                MainLuaRunner.Init(OnRawFileModeLoad);
            else
                MainLuaRunner.Init(OnBundleModeLoad);
        }

        public void ReleaseMainLuaRunner()
        {
            if (MainLuaRunner != null)
            {
                MainLuaRunner.Release();
                MainLuaRunner = null;
            }
        }

        #endregion

        #region PrivateFunc

        private byte[] OnBundleModeLoad(ref string filename)
        {
            LuaFileBytes fileBytes;
            m_LuaFileBytesDict.TryGetValue(filename, out fileBytes);
            return fileBytes?.GetBytes();
        }

        private byte[] OnRawFileModeLoad(ref string filename)
        {
            var path = CommonUtlis.CombinePath(CommonDefine.s_RelativeScriptResRawPath, filename) + ".lua";
            var texts = File.ReadAllText(path);
            var bytes = Encoding.UTF8.GetBytes(texts);
            return bytes;
        }

        #endregion

        #region IGameManager

        public void Awake()
        {
        }

        public void Start()
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
            ReleaseMainLuaRunner();
        }

        public void OnApplicationQuit()
        {
        }

        #endregion
    }
}