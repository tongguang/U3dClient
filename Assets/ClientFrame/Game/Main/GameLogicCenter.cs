namespace U3dClient.Game
{
    public static class GameLogicCenter
    {

        public static GameFlowManager s_GameFlowManager;
        public static ScriptManager s_ScriptManager;

        public static void Awake()
        {
            s_ScriptManager = new ScriptManager();
            s_GameFlowManager = new GameFlowManager();
            s_GameFlowManager.Init();
        }

        public static void Start()
        {
        }

        public static void Update()
        {
            s_GameFlowManager.Update();
        }

        public static void FixedUpdate()
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
            s_ScriptManager.Release();
        }

        public static void OnApplicationQuit()
        {
        }
    }
}