using System;
using Object = UnityEngine.Object;

namespace U3dClient
{
    public class ResourceManager : IGameManager
    {
        private static int s_ResourceIndex = 1;

        #region PublicFunc

        public void InitBundleManifest()
        {
            FullBundleLoader.InitBundleManifest();
        }

        public int LoadAssetAsync<T>(string bundleName, string assetName, Action<bool, T> loadedAction) where T : Object
        {
            var assetLoadMode = GameCenter.s_ConfigManager.GlobalGameConfig.AssetLoadMode;
#if UNITY_EDITOR
            if (assetLoadMode == GameConfig.AssetLoadModeEnum.EditMode)
                return EditorModeAssetLoader.LoadAsync(bundleName, assetName, loadedAction);
#endif
            return BundleAssetLoader.LoadAsync(bundleName, assetName, loadedAction);
        }

        public int LoadAssetSync<T>(string bundleName, string assetName, Action<bool, T> loadedAction) where T : Object
        {
            var assetLoadMode = GameCenter.s_ConfigManager.GlobalGameConfig.AssetLoadMode;
#if UNITY_EDITOR
            if (assetLoadMode == GameConfig.AssetLoadModeEnum.EditMode)
                return EditorModeAssetLoader.LoadSync(bundleName, assetName, loadedAction);
#endif
            return BundleAssetLoader.LoadSync(bundleName, assetName, loadedAction);
        }

        public void UnLoadAsset(int resourceIndex)
        {
            var assetLoadMode = GameCenter.s_ConfigManager.GlobalGameConfig.AssetLoadMode;
#if UNITY_EDITOR
            if (assetLoadMode == GameConfig.AssetLoadModeEnum.EditMode)
                EditorModeAssetLoader.UnLoad(resourceIndex);
            else
#endif
                BundleAssetLoader.UnLoad(resourceIndex);
        }

        #endregion

        #region IGameManager

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

        #endregion

        #region PublicStaticFunc

        public static int GetNewResourceIndex()
        {
            return s_ResourceIndex++;
        }

        #endregion
    }
}