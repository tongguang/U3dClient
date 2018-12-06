using System.Collections.Generic;

namespace U3dClient.Frame
{
    public class FsmBase
    {
        public int LoopIndex = -1;
        private Dictionary<int, IFsmState> m_StateDict;
        private int m_CurStateID;
        private IFsmState m_CurState;
        public void Init(Dictionary<int, IFsmState> stateDict, int initStateID)
        {
            m_StateDict = stateDict;
            ChangeState(initStateID);
        }

        public void Release()
        {
            if (m_CurState != null)
            {
                m_CurState.OnExit();
            }

            m_CurState = null;
        }

        public void Update()
        {
            if (m_CurState!=null)
            {
                m_CurState.OnUpdate();
            }
        }

        public void ChangeState(int stateID)
        {
            if (m_CurState != null)
            {
                m_CurState.OnExit();
            }
            m_CurStateID = stateID;
            m_CurState = m_StateDict[stateID];
            m_CurState.OnEnter();
        }
    }
}