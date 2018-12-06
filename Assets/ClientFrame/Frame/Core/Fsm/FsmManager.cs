using System.Collections.Generic;

namespace U3dClient.Frame
{
    public class FsmManager
    {
        private  Looper m_FsmLooper = new Looper();

        public void Init()
        {

        }

        public T CreateFsm<T>(Dictionary<int, IFsmState> stateDict, int initStateID) where T:FsmBase, new()
        {
            var fsm = m_FsmLooper.CreateItem<T>();
            fsm.Init(stateDict, initStateID);
            return fsm;
        }

        public void ReleaseFsm(FsmBase fsm)
        {
            m_FsmLooper.ReleaseItem(fsm);
            fsm.Release();
        }

        public void Update()
        {
            m_FsmLooper.Update();
        }
    }
}