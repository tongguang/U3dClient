using U3dClient.Frame;

namespace U3dClient.Frame
{
    public static class GameFrameCenter
    {
        public static ResourceManager s_ResourceManager;
        public static UpgradeManager s_UpgradeManager;

        public static void Awake()
        {
            s_ResourceManager = new ResourceManager();
            s_ResourceManager.Init();
            s_UpgradeManager = new UpgradeManager();
            s_UpgradeManager.Init();
        }

        public static void Start()
        {
        }

        public static void Update()
        {
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
            s_UpgradeManager.Release();
            s_ResourceManager.Release();
        }

        public static void OnApplicationQuit()
        {
        }
    }
}