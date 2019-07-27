using System.Collections.Generic;

namespace U3dClient
{
    public class Fsm<T>
    {
        #region PrivateVal

        private readonly Dictionary<T, IFsmState> m_StateDict = new Dictionary<T, IFsmState>();
        private T m_CurStateID;
        private IFsmState m_CurState;
        private int m_RunIndex;

        #endregion

        #region PrivateFunc

        private void Update()
        {
            m_CurState?.OnUpdate();
        }

        #endregion

        #region PublicFunc

        public void Init()
        {
            m_RunIndex = GameCenter.s_UpdateRunManager.AddRun(Update);
        }

        public void Release()
        {
            if (m_RunIndex != 0)
            {
                GameCenter.s_UpdateRunManager.RemoveRun(m_RunIndex);
                m_RunIndex = 0;
            }
            m_CurState?.OnExit();
            m_CurState = null;
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

        #endregion
    }
}