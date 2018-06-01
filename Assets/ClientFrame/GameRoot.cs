using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance;

        public ResourceManager ResourceMgr = new ResourceManager();
        public UpdateManager UpdateMgr = new UpdateManager();


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("GameRoot 初始化两次");
                return;
            }

            ResourceMgr.Awake();
            UpdateMgr.Awake();
        }

        private void Start()
        {
            UpdateMgr.SetResUrl("http://111.231.215.248/AssetBundles1/");
            UpdateMgr.StartUpdate(() => {Debug.Log("下载结束");});
        }

        private void Update()
        {

        }

        private void OnApplicationFocus(bool hasFocus)
        {

        }

        private void OnApplicationPause(bool pauseStatus)
        {
            
        }

        private void OnDestroy()
        {
            
        }
       
        private void OnApplicationQuit()
        {
        }
    }
}

