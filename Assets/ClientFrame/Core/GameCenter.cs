using U3dClient.Fsm;
using U3dClient.GameFlow;
using U3dClient.Resource;
using U3dClient.Upgrade;
using U3dClient.Script;

namespace U3dClient
{
    public static class GameCenter
    {
        public static void Awake()
        {
        }

        public static void Start()
        {
            GameFlowManager.Init();
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