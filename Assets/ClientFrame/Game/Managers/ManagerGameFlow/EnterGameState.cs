using System.Collections.Generic;
using System.Text;
using U3dClient;
using UnityEngine;

namespace U3dClient
{
    public class EnterGameState:IFsmState
    {
        private int m_Step;
        private int m_LuaFileResIndex = -1;
        public void OnEnter()
        {
            Debug.Log("EnterGameState OnEnter");
            m_Step = 1;
            GameCenter.s_ResourceManager.InitBundleManifest();
            GameCenter.s_UpgradeManager.SetResUrl("http://111.231.215.248/AssetBundles1/");
            //            UpdateMgr.StartUpdate(() => {Debug.Log("下载结束");});
        }

        public void OnUpdate()
        {
            if (m_Step == 1)
            {
                m_Step = 2;
                m_LuaFileResIndex = BundleAssetLoader.LoadAsync<LuaFileRef>(CommonDefine.s_ScriptAssetBundleName + "." + CommonDefine.s_BundleSuffixName,
                    CommonDefine.s_ScriptFileDescName, (b, fileRef) =>
                    {
                        m_Step = 3;
                        Dictionary<string, ScriptManager.LuaFileBytes> fileBytesDict = new Dictionary<string, ScriptManager.LuaFileBytes>();
                        foreach (var asset in fileRef.AssetsRefDict)
                        {
                            ScriptManager.LuaFileBytes fileBytes = new ScriptManager.LuaFileBytes();
                            fileBytes.SetBytes(Encoding.UTF8.GetBytes(asset.Value.text));
                            fileBytesDict.Add(asset.Key, fileBytes);
                        }
                        GameCenter.s_ScriptManager.SetLuaFileBytesDict(fileBytesDict);
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
                m_Step = 4;
//                GameCenter.s_GameFlowManager.GameFlowFsm.ChangeState(GameFlowManager.GameFlowState.LuaLoop);
                return;
            }
            else
            {
                
            }
        }

        public void OnExit()
        {
            Debug.Log("EnterGameState OnExit");
        }
    }
}