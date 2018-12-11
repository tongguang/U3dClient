using U3dClient.Frame;

namespace U3dClient.Game
{
    public static class GameLogicCenter
    {

        public static GameFlowManager s_GameFlowManager;

        public static void Awake()
        {
            s_GameFlowManager = new GameFlowManager();
        }

        public static void Start()
        {
            s_GameFlowManager.Init();
        }

        public static void Update()
        {

        }

        public static void OnApplicationFocus(bool hasFocus)
        {

        }

        public static void OnApplicationPause(bool pauseStatus)
        {

        }

        public static void OnDestroy()
        {
            s_GameFlowManager.Release();
        }

        public static void OnApplicationQuit()
        {
        }
    }
}