using System;

namespace U3dClient
{
    public class UpdateRunManager : IGameManager, IGameUpdate
    {
        #region PrivateVal

        private readonly SafeDictContain<int, Action> m_Runners = new SafeDictContain<int, Action>();
        private int m_NowIndex = 1;

        #endregion

        #region PrivateFunc

        private int GetNewIndex()
        {
            return m_NowIndex++;
        }

        private void UpdateRunner(Action action)
        {
            action();
        }

        #endregion

        #region PublicFunc

        public int AddRun(Action action)
        {
            var index = GetNewIndex();
            m_Runners.TryAddLoop(index, action);
            return index;
        }

        public void RemoveRun(int index)
        {
            m_Runners.TryRemoveLoop(index);
        }

        #endregion

        #region IGameManager

        public void Awake()
        {
            m_Runners.SetForeachAction(UpdateRunner);
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

        #region IGameUpdate

        public void Update()
        {
            m_Runners.Foreach();
        }

        #endregion
    }
}