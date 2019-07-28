using System.Collections.Generic;

namespace U3dClient
{
    public class Fsm<T>
    {
        #region PrivateVal

        private readonly Dictionary<T, IFsmState> m_StateDict = new Dictionary<T, IFsmState>();
        private T m_CurStateID;
        private IFsmState m_CurState;
        private int m_UpdateIndex;
        private bool m_IsExecute = false;

        #endregion

        #region PrivateFunc

        private void Update()
        {
            m_CurState?.OnUpdate();
        }

        #endregion

        #region PublicFunc

        public void Init(bool isExecute)
        {
            SetExecute(isExecute);
        }

        public void Release()
        {
            SetExecute(false);
            m_CurState?.OnExit();
            m_CurState = null;
        }

        public void SetExecute(bool isExecute)
        {
            if (m_IsExecute == isExecute)
            {
                return;
            }

            m_IsExecute = isExecute;
            if (isExecute)
            {
                m_UpdateIndex = GameCenter.s_UpdateRunManager.AddRun(Update);
            }
            else
            {
                if (m_UpdateIndex == 0) return;
                GameCenter.s_UpdateRunManager.RemoveRun(m_UpdateIndex);
                m_UpdateIndex = 0;
            }
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