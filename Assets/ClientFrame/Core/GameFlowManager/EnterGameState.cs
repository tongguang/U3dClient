using U3dClient.Component;
using U3dClient.FsmMgr;
using U3dClient.ResourceMgr;
using U3dClient.ScriptMgr;
using U3dClient.UpgradeMgr;
using UnityEngine;

namespace U3dClient.GameFlowMgr
{
    public class EnterGameState:IFsmState
    {
        private int m_Step;
        public void OnEnter()
        {
            Debug.Log("EnterGameState OnEnter");
            m_Step = 1;
            ResourceManager.Init();
            UpgradeManager.SetResUrl("http://111.231.215.248/AssetBundles1/");
            //            UpdateMgr.StartUpdate(() => {Debug.Log("下载结束");});
        }

        public void OnUpdate()
        {
            if (m_Step == 1)
            {
                m_Step = 2;
                BundleAssetLoader.LoadAsync<LuaFileRef>(GlobalDefine.s_ScriptAssetBundleName + "." + GlobalDefine.s_BundleSuffixName,
                    GlobalDefine.s_ScriptFileDescName, (b, fileRef) =>
                    {
                        m_Step = 3;
                        ScriptManager.SetLuaFileAssetDict(fileRef.AssetsRefDict);
                    }
                );
            }
            else if (m_Step == 2)
            {
            }
            else if (m_Step == 3)
            {
                GameFlowManager.GameFlowFsm.ChangeState((int) GameFlowManager.GameFlowState.LuaLoop);
                return;
            }
            else
            {
                
            }

            Debug.Log("EnterGameState OnUpdate");
        }

        public void OnExit()
        {
            Debug.Log("EnterGameState OnExit");
        }
    }
}