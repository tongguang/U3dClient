using U3dClient.Frame;
using UnityEngine;

namespace U3dClient.Game
{
    public class LuaLoopState : IFsmState
    {
        public void OnEnter()
        {
            GameLogicCenter.s_ScriptManager.InitMainLuaRunner();
            GameLogicCenter.s_ScriptManager.MainLuaRunner.DoInit();
        }

        public void OnUpdate()
        {
            GameLogicCenter.s_ScriptManager.MainLuaRunner.DoUpdate();
        }

        public void OnExit()
        {
            GameLogicCenter.s_ScriptManager.MainLuaRunner.DoRelease();
            GameLogicCenter.s_ScriptManager.ReleaseMainLuaRunner();
        }
    }
}