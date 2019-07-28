using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace U3dClient
{
    public class TaskScheduler
    {
        private SortedSet<TaskBase> m_WillDoTasks = new SortedSet<TaskBase>();
        private SafeSetContain<TaskBase> m_DoingTasks = new SafeSetContain<TaskBase>();
        private int m_UpdateRunIndex = 0;
        private int m_NowTaskIndex = 1;

        public int MaxDoingTask = -1;

        #region PrivateFunc

        private void UpdateRunTask()
        {
            if (MaxDoingTask == -1)
            {
                foreach (var willDoTask in m_WillDoTasks)
                {
                    m_DoingTasks.TryAddLoop(willDoTask);
                }
                m_WillDoTasks.Clear();
            }
            else if (m_DoingTasks.GetItemCount() < MaxDoingTask)
            {
                while ((m_DoingTasks.GetItemCount() < MaxDoingTask) && m_WillDoTasks.Count > 0 )
                {
                    var task = m_WillDoTasks.Min;
                    m_WillDoTasks.Remove(task);
                    m_DoingTasks.TryAddLoop(task);
                }
            }
            m_DoingTasks.Foreach();
        }

        private void DoingTask(TaskBase taskBase)
        {
            var result = taskBase.Execute();
            if (!result)
            {
                RemoveTask(taskBase);
            }
        }

        #endregion

        #region PublicFunc

        public void Init(int maxDoingTask, Action<TaskBase> removeItemAction)
        {
            MaxDoingTask = maxDoingTask;
            m_DoingTasks.SetRemoveItemAction(removeItemAction);
            m_DoingTasks.SetForeachAction(DoingTask);

            m_UpdateRunIndex = GameCenter.s_UpdateRunManager.AddRun(UpdateRunTask);
        }

        public void Release()
        {
            if (m_UpdateRunIndex != 0)
            {
                GameCenter.s_UpdateRunManager.RemoveRun(m_UpdateRunIndex);
                m_UpdateRunIndex = 0;
            }
        }

        public void AddTask(TaskBase task)
        {
            task.Index = m_NowTaskIndex++;
            m_WillDoTasks.Add(task);
        }

        public void RemoveTask(TaskBase task)
        {
            if (m_WillDoTasks.Contains(task))
            {
                m_WillDoTasks.Remove(task);
                return;
            }

            m_DoingTasks.TryRemoveLoop(task);
        }

        #endregion
    }
}