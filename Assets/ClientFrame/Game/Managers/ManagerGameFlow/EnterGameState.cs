using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace U3dClient
{
    public class EnterGameState : IFsmState
    {
        #region PrivateInt

        private int m_LuaFileResIndex = -1;
        private int m_Step;

        #endregion


        #region IFsmState

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
                if (GameCenter.s_ConfigManager.GlobalGameConfig.LuaScriptLoadMode ==
                    GameConfig.LuaScriptLoadModeEnum.RawFileMode)
                {
                    m_Step = 4;
                }
                else
                {
                    m_Step = 2;
                    m_LuaFileResIndex = BundleAssetLoader.LoadAsync<LuaFileRef>(
                        CommonDefine.s_ScriptAssetBundleName + "." + CommonDefine.s_BundleSuffixName,
                        CommonDefine.s_ScriptFileDescName, (b, fileRef) =>
                        {
                            m_Step = 3;
                            var fileBytesDict = new Dictionary<string, ScriptManager.LuaFileBytes>();
                            foreach (var asset in fileRef.AssetsRefDict)
                            {
                                var fileBytes = new ScriptManager.LuaFileBytes();
                                fileBytes.SetBytes(Encoding.UTF8.GetBytes(asset.Value.text));
                                fileBytesDict.Add(asset.Key, fileBytes);
                            }

                            GameCenter.s_ScriptManager.SetLuaFileBytesDict(fileBytesDict);
                        }
                    );
                }
            }
            else if (m_Step == 2)
            {
            }
            else if (m_Step == 3)
            {
                if (m_LuaFileResIndex != -1) BundleAssetLoader.UnLoad(m_LuaFileResIndex);
                m_Step = 4;
            }
            else if (m_Step == 4)
            {
                GameCenter.s_GameFlowManager.GameFlowFsm.ChangeState(GameFlowManager.GameFlowState.LuaLoop);
            }
        }

        public void OnExit()
        {
            Debug.Log("EnterGameState OnExit");
        }

        #endregion
    }
}