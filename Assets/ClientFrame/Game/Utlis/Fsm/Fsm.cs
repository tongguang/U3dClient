using System.Collections.Generic;

namespace U3dClient
{
    public class Fsm<T>
    {
        public int LoopIndex = -1;
        private Dictionary<T, IFsmState> m_StateDict = new Dictionary<T, IFsmState>();
        private T m_CurStateID;
        private IFsmState m_CurState;

       
        public void Release()
        {
            m_CurState?.OnExit();
            m_CurState = null;
        }

        public void Update()
        {
            m_CurState?.OnUpdate();
        }

        public void AddState(T stateKey, IFsmState state)
        {
            m_StateDict.Add(stateKey, state);
        }

        public void ChangeState(T stateKey)
        {
            m_CurState?.OnExit();
            m_CurStateID = stateKey;
            m_CurState = m_StateDict[stateKey];
            m_CurState.OnEnter();
        }
    }
}