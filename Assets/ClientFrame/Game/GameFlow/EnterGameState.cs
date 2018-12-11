using System.Collections.Generic;
using System.Text;
using U3dClient.Frame;
using UnityEngine;

namespace U3dClient.Game
{
    public class EnterGameState:IFsmState
    {
        private int m_Step;
        private int m_LuaFileResIndex = -1;
        public void OnEnter()
        {
            Debug.Log("EnterGameState OnEnter");
            m_Step = 1;
            GameFrameCenter.s_ResourceManager.InitBundleManifest();
            GameFrameCenter.s_UpgradeManager.SetResUrl("http://111.231.215.248/AssetBundles1/");
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
                GameLogicCenter.s_GameFlowManager.GameFlowFsm.ChangeState((int) GameFlowManager.GameFlowState.LuaLoop);
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