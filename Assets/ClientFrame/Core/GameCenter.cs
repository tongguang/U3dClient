using U3dClient.FsmMgr;
using U3dClient.GameFlowMgr;
using U3dClient.ResourceMgr;
using U3dClient.UpgradeMgr;
using U3dClient.ScriptMgr;

namespace U3dClient
{
    public static class GameCenter
    {
        public static void Awake()
        {
            GameFlowManager.Awake();
        }

        public static void Start()
        {
            GameFlowManager.Start();
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
            GameFlowManager.OnDestroy();
        }

        public static void OnApplicationQuit()
        {
            GameFlowManager.OnApplicationQuit();
        }
    }
}