using U3dClient.Component;
using U3dClient.Fsm;
using U3dClient.Resource;
using U3dClient.Script;
using U3dClient.Upgrade;
using UnityEngine;

namespace U3dClient.GameFlow
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