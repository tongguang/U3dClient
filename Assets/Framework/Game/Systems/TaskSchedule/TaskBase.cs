using System;

namespace U3dClient
{
    public abstract class TaskBase:IComparable<TaskBase>
    {
        public int Index;
        public bool IsCancel;
//        public bool IsComplete;
//        public bool IsDone => IsCancel || IsComplete;
        public abstract bool Execute();
        public int CompareTo(TaskBase other)
        {
            return Index.CompareTo(other.Index);
        }
    }
}