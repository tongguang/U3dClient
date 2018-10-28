using XLua;
namespace U3dClient.ScriptMgr
{
    public class LuaRunner
    {
        private bool m_IsReady = false;
        private LuaEnv m_LuaEnv = null;

        public void Init()
        {
            m_LuaEnv = new LuaEnv();
        }

        public void Release()
        {
            m_LuaEnv.Dispose();
            m_LuaEnv = null;
        }

        void Update()
        {
            if (!m_IsReady)
            {
                return;
            }
            if (m_LuaEnv != null)
            {
                m_LuaEnv.Tick();
            }
        }
    }
}