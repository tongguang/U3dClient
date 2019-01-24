using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace U3dClient
{
    public class UpdateRunManager:IGameManager, IGameUpdate
    {
        private LoopContain<int, Action> m_Runners = new LoopContain<int, Action>();
        private int m_NowIndex = 1;

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

        private int GetNewIndex()
        {
            return m_NowIndex++;
        }

        private void UpdateRunner(Action action)
        {
            action();
        }

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

        public void Update()
        {
            m_Runners.Foreach();
        }
    }
}