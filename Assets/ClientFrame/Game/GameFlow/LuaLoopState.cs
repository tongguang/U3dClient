using U3dClient.Frame;
using UnityEngine;

namespace U3dClient.Game
{
    public class LuaLoopState : IFsmState
    {
        private int m_Step;
        public void OnEnter()
        {
            ScriptManager.InitMainLuaRunner();
        }

        public void OnUpdate()
        {
            ScriptManager.UpdateMainLuaRunner();
        }

        public void OnExit()
        {
            ScriptManager.ReleaseMainLuaRunner();
        }
    }
}