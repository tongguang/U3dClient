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
        public Fsm<GameFlowState> GameFlowFsm;


        public void Init()
        {
            GameFlowFsm = new Fsm<GameFlowState>();
            GameFlowFsm.AddState(GameFlowState.EnterGame, new EnterGameState());
            GameFlowFsm.AddState(GameFlowState.LuaLoop, new LuaLoopState());
            GameFlowFsm.ChangeState(GameFlowState.EnterGame);
        }

        public void Update()
        {
            GameFlowFsm.Update();
        }

        public void Release()
        {
            if (GameFlowFsm != null)
            {
                GameFlowFsm.Release();
                GameFlowFsm = null;
            }
        }

    }
}