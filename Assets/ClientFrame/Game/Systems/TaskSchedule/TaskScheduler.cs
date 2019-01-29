using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace U3dClient
{
    public class TaskScheduler
    {
        private Queue<ITask> m_WillDoTasks = new Queue<ITask>();
        private LoopSetContain<ITask> m_DoingTasks = new LoopSetContain<ITask>();
        private HashSet<ITask> m_WillRemoveTasks = new HashSet<ITask>();
        private int m_RunIndex = 0;

        public int MaxDoingTask = -1;

        #region PrivateFunc

        private void UpdateRunTask()
        {
            m_DoingTasks.Foreach();
        }

        private void ForeachDoingTask(ITask task)
        {
            var result = task.Exec();
            if (!result)
            {
                RemoveTask(task);
            }
        }

        #endregion

        #region PublicFunc

        public void Init(int maxDoingTask, Action<ITask> removeItemAction)
        {
            MaxDoingTask = maxDoingTask;
            m_DoingTasks.SetRemoveItemAction(removeItemAction);
            m_DoingTasks.SetForeachAction(ForeachDoingTask);

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
            m_WillDoTasks.Enqueue(task);
        }

        public void RemoveTask(ITask task)
        {
            m_WillRemoveTasks.Add(task);
        }

        #endregion
    }
}