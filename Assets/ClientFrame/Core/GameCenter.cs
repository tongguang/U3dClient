using U3dClient.ResourceMgr;
using U3dClient.UpdateMgr;
using U3dClient.ScriptMgr;

namespace U3dClient
{
    public static class GameCenter
    {
        public static void Awake()
        {
            ResourceManager.Awake();
            UpdateManager.Awake();
            ScriptManager.Awake();
        }

        public static void Start()
        {
            UpdateManager.SetResUrl("http://111.231.215.248/AssetBundles1/");
            //            UpdateMgr.StartUpdate(() => {Debug.Log("下载结束");});
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

        }

        public static void OnApplicationQuit()
        {
        }
    }
}