using System.Collections.Generic;
using U3dClient.FsmMgr;

namespace U3dClient.GameFlowMgr
{
    public static class GameFlowManager
    {
        public enum GameFlowState
        {
            EnterGame,
        }
        public static FsmBase GameFlowFsm;


        public static void Awake()
        {

        }

        public static void Start()
        {
            GameFlowFsm = FsmManager.CreateFsm<FsmBase>(new Dictionary<int, IFsmState>
            {
                { (int)GameFlowState.EnterGame, new EnterGameState()},
            }, (int) GameFlowState.EnterGame);
        }

        public static void OnDestroy()
        {
            if (GameFlowFsm != null)
            {
                FsmManager.ReleaseFsm(GameFlowFsm.FsmIndex);
            }
        }

        public static void OnApplicationQuit()
        {

        }

    }
}