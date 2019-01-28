using System;
using System.Collections.Generic;

namespace U3dClient
{
    public class TaskScheduler
    {
        private Queue<ITask> m_WillDoTasks;
        private Queue<ITask> m_DoingTasks;
        private int m_RunIndex = 0;

        public int MaxDoingTask = -1;

        #region PrivateFunc

        private void UpdateRunTask()
        {

        }

        #endregion

        #region PublicFunc

        public void Init(int maxDoingTask)
        {
            MaxDoingTask = maxDoingTask;
            m_RunIndex = GameCenter.s_UpdateRunManager.AddRun(UpdateRunTask);
        }

        public void Release()
        {
            if (m_RunIndex != 0)
            {
                GameCenter.s_UpdateRunManager.RemoveRun(m_RunIndex);
                m_RunIndex = 0;
            }
        }

        public void AddTask(ITask task)
        {

        }

        #endregion
    }
}