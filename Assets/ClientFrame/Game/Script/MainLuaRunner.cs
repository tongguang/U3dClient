using System;
using XLua;
namespace U3dClient.Game
{
    public class MainLuaRunner
    {
        [CSharpCallLua]
        public interface ICallLuaLoopMap
        {
            void Init();
            void Update();
            void Release();
        }

        private LuaEnv m_LuaEnv = null;
        private ICallLuaLoopMap m_CallLuaLoopMap;

        public void Init(LuaEnv.CustomLoader loader)
        {
            m_LuaEnv = new LuaEnv();
            m_LuaEnv.AddLoader(loader);
            m_LuaEnv.DoString("require('main')");
            m_CallLuaLoopMap = m_LuaEnv.Global.Get<ICallLuaLoopMap>("MainLoop");
            m_CallLuaLoopMap.Init();
        }

        public void Release()
        {
            if (m_LuaEnv != null)
            {
                m_CallLuaLoopMap.Release();
                m_CallLuaLoopMap = null;
                m_LuaEnv.Dispose();
                m_LuaEnv = null;
            }
        }

        public void Update()
        {
            if (m_LuaEnv != null)
            {
                m_CallLuaLoopMap.Update();
                m_LuaEnv.Tick();
            }
        }
    }
}