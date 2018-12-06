using U3dClient.Frame;

namespace U3dClient.Game
{
    public static class GameCenter
    {
        public static void Awake()
        {
            GameFlowManager.Init();
        }

        public static void Start()
        {
        }

        public static void Update()
        {
            FsmManager.Update();
        }

        public static void OnApplicationFocus(bool hasFocus)
        {

        }

        public static void OnApplicationPause(bool pauseStatus)
        {

        }

        public static void OnDestroy()
        {
            GameFlowManager.Release();
        }

        public static void OnApplicationQuit()
        {
        }
    }
}