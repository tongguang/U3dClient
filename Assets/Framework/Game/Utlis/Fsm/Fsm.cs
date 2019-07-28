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
        private bool m_IsAutoRun = false;

        #endregion

        #region PrivateFunc

        private void Update()
        {
            m_CurState?.OnUpdate();
        }

        #endregion

        #region PublicFunc

        public void Init(bool isAutoRun)
        {
            SetAutoRun(isAutoRun);
        }

        public void Release()
        {
            SetAutoRun(false);
            m_CurState?.OnExit();
            m_CurState = null;
        }

        public void SetAutoRun(bool isAuto)
        {
            if (m_IsAutoRun == isAuto)
            {
                return;
            }

            m_IsAutoRun = isAuto;
            if (isAuto)
            {
                m_RunIndex = GameCenter.s_UpdateRunManager.AddRun(Update);
            }
            else
            {
                if (m_RunIndex == 0) return;
                GameCenter.s_UpdateRunManager.RemoveRun(m_RunIndex);
                m_RunIndex = 0;
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