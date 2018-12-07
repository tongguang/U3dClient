using System.Collections.Generic;

namespace U3dClient.Frame
{
    public class FsmManager
    {
        private  LoopContain m_FsmLoopContain = new LoopContain();

        public void Init()
        {

        }

        public void Release()
        {

        }

        public T CreateFsm<T>(Dictionary<int, IFsmState> stateDict, int initStateID) where T:FsmBase, new()
        {
            var fsm = new T();
            fsm.Init(stateDict, initStateID);
            fsm.LoopIndex = m_FsmLoopContain.AddLoopAction(fsm.Update);
            return fsm;
        }

        public void ReleaseFsm(FsmBase fsm)
        {
            m_FsmLoopContain.RemoveLoopAction(fsm.LoopIndex);
            fsm.Release();
        }

        public void Update()
        {
            m_FsmLoopContain.Update();
        }
    }
}