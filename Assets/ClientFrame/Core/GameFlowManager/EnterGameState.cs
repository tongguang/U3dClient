using U3dClient.FsmMgr;
using U3dClient.ResourceMgr;
using U3dClient.UpdateMgr;
using UnityEngine;

namespace U3dClient.GameFlowMgr
{
    public class EnterGameState:IFsmState
    {
        public void OnEnter()
        {
            Debug.Log("EnterGameState OnEnter");
            ResourceManager.InitResourceManager();
            UpdateManager.SetResUrl("http://111.231.215.248/AssetBundles1/");
            //            UpdateMgr.StartUpdate(() => {Debug.Log("下载结束");});
        }

        public void OnUpdate()
        {
            Debug.Log("EnterGameState OnUpdate");
        }

        public void OnExit()
        {
            Debug.Log("EnterGameState OnExit");
        }
    }
}