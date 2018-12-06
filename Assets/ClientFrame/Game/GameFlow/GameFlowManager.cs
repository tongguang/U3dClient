using System.Collections.Generic;
using U3dClient.Frame;

namespace U3dClient.Game
{
    public static class GameFlowManager
    {
        public enum GameFlowState
        {
            EnterGame,
            LuaLoop,
        }
        public static FsmBase GameFlowFsm;


        public static void Init()
        {
            GameFlowFsm = FsmManager.CreateFsm<FsmBase>(new Dictionary<int, IFsmState>
            {
                { (int)GameFlowState.EnterGame, new EnterGameState()},
                { (int)GameFlowState.LuaLoop, new LuaLoopState()},
            }, (int) GameFlowState.EnterGame);
        }

        public static void Release()
        {
            if (GameFlowFsm != null)
            {
                FsmManager.ReleaseFsm(GameFlowFsm);
                GameFlowFsm = null;
            }
        }

    }
}