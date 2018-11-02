using System.Collections.Generic;
using System.Text;
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
        private int m_LuaFileResIndex = -1;
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
                m_LuaFileResIndex = BundleAssetLoader.LoadAsync<LuaFileRef>(GlobalDefine.s_ScriptAssetBundleName + "." + GlobalDefine.s_BundleSuffixName,
                    GlobalDefine.s_ScriptFileDescName, (b, fileRef) =>
                    {
                        m_Step = 3;
                        Dictionary<string, ScriptManager.LuaFileBytes> fileBytesDict = new Dictionary<string, ScriptManager.LuaFileBytes>();
                        foreach (var asset in fileRef.AssetsRefDict)
                        {
                            ScriptManager.LuaFileBytes fileBytes = new ScriptManager.LuaFileBytes();
                            fileBytes.SetBytes(Encoding.UTF8.GetBytes(asset.Value.text));
                            fileBytesDict.Add(asset.Key, fileBytes);
                        }
                        ScriptManager.SetLuaFileBytesDict(fileBytesDict);
                    }
                );
            }
            else if (m_Step == 2)
            {
            }
            else if (m_Step == 3)
            {
                if (m_LuaFileResIndex != -1)
                {
                    BundleAssetLoader.UnLoad(m_LuaFileResIndex);
                }
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