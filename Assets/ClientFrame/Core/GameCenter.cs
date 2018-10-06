using U3dClient.ResourceMgr;
namespace U3dClient
{
    public static class GameCenter
    {
        public static UpdateManager UpdateMgr = new UpdateManager();

        static GameCenter()
        {

        }

        public static void Awake()
        {
            ResourceMgr.ResourceManager.Awake();
//            UpdateMgr.Awake();
        }

        public static void Start()
        {
            UpdateMgr.SetResUrl("http://111.231.215.248/AssetBundles1/");
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