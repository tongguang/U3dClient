using System.Collections.Generic;
using U3dClient;

namespace U3dClient
{
    public class GameFlowManager : IGameManager
    {
        public enum GameFlowState
        {
            EnterGame,
            LuaLoop,
        }
        public Fsm<GameFlowState> GameFlowFsm;

        public void Awake()
        {
            GameFlowFsm = new Fsm<GameFlowState>();
            GameFlowFsm.AddState(GameFlowState.EnterGame, new EnterGameState());
            GameFlowFsm.AddState(GameFlowState.LuaLoop, new LuaLoopState());
            GameFlowFsm.ChangeState(GameFlowState.EnterGame);
        }

        public void Start()
        {
        }

        public void Update()
        {
            GameFlowFsm.Update();
        }

        public void FixedUpdate()
        {
        }

        public void OnApplicationFocus(bool hasFocus)
        {
        }

        public void OnApplicationPause(bool pauseStatus)
        {
        }

        public void OnDestroy()
        {
            if (GameFlowFsm != null)
            {
                GameFlowFsm.Release();
                GameFlowFsm = null;
            }
        }

        public void OnApplicationQuit()
        {
        }
    }
}