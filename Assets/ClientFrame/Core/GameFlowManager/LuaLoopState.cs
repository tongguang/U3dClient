using U3dClient.Component;
using U3dClient.FsmMgr;
using U3dClient.ResourceMgr;
using U3dClient.ScriptMgr;
using U3dClient.UpgradeMgr;
using UnityEngine;

namespace U3dClient.GameFlowMgr
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