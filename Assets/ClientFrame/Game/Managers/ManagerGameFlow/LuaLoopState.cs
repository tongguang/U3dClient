using U3dClient;
using UnityEngine;

namespace U3dClient
{
    public class LuaLoopState : IFsmState
    {
        public void OnEnter()
        {
            GameCenter.s_ScriptManager.InitMainLuaRunner();
            GameCenter.s_ScriptManager.MainLuaRunner.DoInit();
        }

        public void OnUpdate()
        {
            GameCenter.s_ScriptManager.MainLuaRunner.DoUpdate();
        }

        public void OnExit()
        {
            GameCenter.s_ScriptManager.MainLuaRunner.DoRelease();
            GameCenter.s_ScriptManager.ReleaseMainLuaRunner();
        }
    }
}