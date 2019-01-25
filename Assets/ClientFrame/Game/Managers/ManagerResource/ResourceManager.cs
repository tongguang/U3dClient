using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace U3dClient
{
    public class ResourceManager : IGameManager
    {
        private static int s_ResourceIndex = 1;

        public static int GetNewResourceIndex()
        {
            return s_ResourceIndex++;
        }

//        private GameConfig.AssetLoadModeEnum AssetLoadMode;

        public void InitBundleManifest()
        {
            FullBundleLoader.InitBundleManifest();
        }

        public int LoadAssetAsync<T>(string bundleName, string assetName, Action<bool, T> loadedAction) where T : Object
        {
            var assetLoadMode = GameCenter.s_ConfigManager.GlobalGameConfig.AssetLoadMode;
#if UNITY_EDITOR
            if (assetLoadMode == GameConfig.AssetLoadModeEnum.EditMode)
            {
                return EditorModeAssetLoader.LoadAsync<T>(bundleName, assetName, loadedAction);
            }
            else
#endif
            {
                return BundleAssetLoader.LoadAsync<T>(bundleName, assetName, loadedAction);
            }
        }

        public int LoadAssetSync<T>(string bundleName, string assetName, Action<bool, T> loadedAction) where T : Object
        {
            var assetLoadMode = GameCenter.s_ConfigManager.GlobalGameConfig.AssetLoadMode;
#if UNITY_EDITOR
            if (assetLoadMode == GameConfig.AssetLoadModeEnum.EditMode)
            {
                return EditorModeAssetLoader.LoadSync<T>(bundleName, assetName, loadedAction);
            }
            else
#endif
            {
                return BundleAssetLoader.LoadSync<T>(bundleName, assetName, loadedAction);
            }
        }

        public void UnLoadAsset(int resourceIndex)
        {
            var assetLoadMode = GameCenter.s_ConfigManager.GlobalGameConfig.AssetLoadMode;
#if UNITY_EDITOR
            if (assetLoadMode == GameConfig.AssetLoadModeEnum.EditMode)
            {
                EditorModeAssetLoader.UnLoad(resourceIndex);
            }
            else
#endif
            {
                BundleAssetLoader.UnLoad(resourceIndex);
            }
        }

        public void Awake()
        {
        }

        public void Start()
        {
        }

        public void OnApplicationFocus(bool hasFocus)
        {
        }

        public void OnApplicationPause(bool pauseStatus)
        {
        }

        public void OnDestroy()
        {
        }

        public void OnApplicationQuit()
        {
        }
    }
}
