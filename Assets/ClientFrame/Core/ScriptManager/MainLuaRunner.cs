using System;
using XLua;
namespace U3dClient.ScriptMgr
{

    public class MainLuaLoopMap
    {
        public Action Init;
        public Action Update;
        public Action Release;
    }

    public class MainLuaRunner
    {
        private LuaEnv m_LuaEnv = null;
        private MainLuaLoopMap m_MainLuaLoopMap;

        public void Init(LuaEnv.CustomLoader loader)
        {
            m_LuaEnv = new LuaEnv();
            m_LuaEnv.AddLoader(loader);
            m_LuaEnv.DoString("require('main')");
            m_MainLuaLoopMap = m_LuaEnv.Global.Get<MainLuaLoopMap>("MainLoop");
            m_MainLuaLoopMap.Init();
        }

        public void Release()
        {
            if (m_LuaEnv != null)
            {
                m_MainLuaLoopMap.Release();
                m_MainLuaLoopMap = null;
                m_LuaEnv.Dispose();
                m_LuaEnv = null;
            }
        }

        public void Update()
        {
            if (m_LuaEnv != null)
            {
                m_MainLuaLoopMap.Update();
                m_LuaEnv.Tick();
            }
        }
    }
}