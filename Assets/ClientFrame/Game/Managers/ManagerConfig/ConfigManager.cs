using UnityEngine;

namespace U3dClient
{
    public class ConfigManager :IGameManager
    {
        public GameConfig GlobalGameConfig;
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
    }
}