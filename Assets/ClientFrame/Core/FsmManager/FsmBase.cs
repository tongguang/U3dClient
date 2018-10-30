using System.Collections.Generic;

namespace U3dClient.FsmMgr
{
    public class FsmBase
    {
        private Dictionary<int, IFsmState> m_StateDict;
        private int m_CurStateID;
        private IFsmState m_CurState;
        public int FsmIndex = -1;
        public bool IsVaild = true;
        public void Init(int fsmIndex, Dictionary<int, IFsmState> stateDict, int initStateID)
        {
            FsmIndex = fsmIndex;
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
            IsVaild = false;
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