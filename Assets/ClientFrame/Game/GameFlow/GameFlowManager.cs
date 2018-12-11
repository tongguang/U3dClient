using System.Collections.Generic;
using U3dClient.Frame;

namespace U3dClient.Game
{
    public class GameFlowManager
    {
        public enum GameFlowState
        {
            EnterGame,
            LuaLoop,
        }
        public FsmBase GameFlowFsm;


        public void Init()
        {
            GameFlowFsm = GameFrameCenter.s_FsmManager.CreateFsm<FsmBase>(new Dictionary<int, IFsmState>
            {
                { (int)GameFlowState.EnterGame, new EnterGameState()},
                { (int)GameFlowState.LuaLoop, new LuaLoopState()},
            }, (int) GameFlowState.EnterGame);
        }

        public void Release()
        {
            if (GameFlowFsm != null)
            {
                GameFrameCenter.s_FsmManager.ReleaseFsm(GameFlowFsm);
                GameFlowFsm = null;
            }
        }

    }
}