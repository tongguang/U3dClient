namespace U3dClient
{
    public class GameFlowManager : IGameManager
    {
        #region Enum

        public enum GameFlowState
        {
            EnterGame,
            LuaLoop
        }

        #endregion

        #region PublicVal

        public Fsm<GameFlowState> GameFlowFsm;

        #endregion

        #region IGameManager

        public void Awake()
        {
            GameFlowFsm = new Fsm<GameFlowState>();
            GameFlowFsm.Init();
            GameFlowFsm.AddState(GameFlowState.EnterGame, new EnterGameState());
            GameFlowFsm.AddState(GameFlowState.LuaLoop, new LuaLoopState());
            GameFlowFsm.ChangeState(GameFlowState.EnterGame);
        }

        public void Start()
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

        #endregion
    }
}