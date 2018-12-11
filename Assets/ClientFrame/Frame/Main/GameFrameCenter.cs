using U3dClient.Frame;

namespace U3dClient.Game
{
    public static class GameFrameCenter
    {
        public static FsmManager s_FsmManager;
        public static ResourceManager s_ResourceManager;
        public static UpgradeManager s_UpgradeManager;

        public static void Awake()
        {
            s_FsmManager = new FsmManager();
            s_ResourceManager = new ResourceManager();
            s_UpgradeManager = new UpgradeManager();
        }

        public static void Start()
        {
            s_FsmManager.Init();
            s_ResourceManager.Init();
            s_UpgradeManager.Init();
        }

        public static void Update()
        {
            s_FsmManager.Update();
        }

        public static void OnApplicationFocus(bool hasFocus)
        {

        }

        public static void OnApplicationPause(bool pauseStatus)
        {

        }

        public static void OnDestroy()
        {
            s_FsmManager.Release();
            s_ResourceManager.Release();
            s_UpgradeManager.Release();
        }

        public static void OnApplicationQuit()
        {
        }
    }
}