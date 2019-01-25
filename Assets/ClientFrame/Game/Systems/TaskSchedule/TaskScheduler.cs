using System;
using System.Collections.Generic;

namespace U3dClient
{
    public interface ITask
    {
        bool Exec();
    }
    public class TaskScheduler
    {
        private Queue<ITask> m_WillDoTasks;
        private Queue<ITask> m_DoingTasks;

        public int MaxDoingTask = -1;

        public void Init()
        {

        }

        public void Release()
        {

        }

        public void AddTask(ITask task)
        {

        }

        private void UpdateRunTask()
        {

        }
    }
}