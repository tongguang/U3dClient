namespace U3dClient
{
    public interface IGameManager
    {
        void Awake();
        void Start();
        void OnApplicationFocus(bool hasFocus);
        void OnApplicationPause(bool pauseStatus);
        void OnDestroy();
        void OnApplicationQuit();
    }
}