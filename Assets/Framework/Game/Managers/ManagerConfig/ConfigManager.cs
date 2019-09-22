using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace U3dClient
{
    public class ConfigManager : IGameManager
    {
        #region PublicVal

        public GameConfig GlobalGameConfig;

        #endregion

        #region IGameManager

        public void Awake()
        {
            var configTextAsset = Resources.Load<TextAsset>("gameConfig");
            GlobalGameConfig = JsonUtility.FromJson<GameConfig>(configTextAsset.text);
            Debug.Log(GlobalGameConfig.AssetLoadMode);
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

        private Dictionary<string, string> s_BundleNameToPath = new Dictionary<string, string>();

        public string GetBundlePath(string bundleName)
        {
            if (bundleName == "")
            {
                return "";
            }

            {
                string bundlePath = null;
                s_BundleNameToPath.TryGetValue(bundleName, out bundlePath);
                if (bundlePath != null)
                {
                    return bundlePath;
                }
            }
            {
                var bundlePath = Path.Combine(FileUtlis.s_PersistentDataPath, bundleName);
                if (File.Exists(bundlePath))
                {
                    s_BundleNameToPath.Add(bundleName, bundlePath);
                    return bundlePath;
                }
            }
            {
                var bundlePath = Path.Combine(FileUtlis.s_StreamingAssetsPath, bundleName);
                s_BundleNameToPath.Add(bundleName, bundlePath);
                return bundlePath;
            }
        }
    }
}