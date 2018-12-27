using U3dClient.Frame;
using UnityEngine;

namespace U3dClient.Game
{
    public class LuaLoopState : IFsmState
    {
        private int m_Step;
        public void OnEnter()
        {
            GameLogicCenter.s_ScriptManager.InitMainLuaRunner();
        }

        public void OnUpdate()
        {
            GameLogicCenter.s_ScriptManager.UpdateMainLuaRunner();
        }

        public void OnExit()
        {
            GameLogicCenter.s_ScriptManager.ReleaseMainLuaRunner();
        }
    }
}