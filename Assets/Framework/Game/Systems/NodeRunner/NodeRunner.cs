using System.Collections.Generic;

namespace U3dClient
{
    public class NodeRunner
    {
        private int m_UpdateRunIndex = 0;
        private List<INode> m_NodeList = new List<INode>();
        private int m_RunningIndex = -1;
        private bool m_IsEnd = false;
        public void Init()
        {
            m_RunningIndex = -1;
            m_IsEnd = false;
            m_UpdateRunIndex = GameCenter.s_UpdateRunManager.AddRun(Execute);
        }

        public void Execute()
        {
            if (m_RunningIndex < 0)
            {
                m_RunningIndex = m_RunningIndex + 1;
                if (m_RunningIndex < m_NodeList.Count)
                {
                    var node = m_NodeList[m_RunningIndex];
                    node.OnStart();
                }
            }
            else if (m_RunningIndex >= m_NodeList.Count)
            {
                var node = m_NodeList[m_RunningIndex];
                if (!node.Execute())
                {
                    node.OnEnd();
                    m_RunningIndex = m_RunningIndex + 1;
                    if (m_RunningIndex < m_NodeList.Count)
                    {
                        node = m_NodeList[m_RunningIndex];
                        node.OnStart();
                    }
                }
            }
            else
            {
                Release();
            }
        }

        public void Release()
        {
            m_IsEnd = true;
            if (m_UpdateRunIndex != 0)
            {
                GameCenter.s_UpdateRunManager.RemoveRun(m_UpdateRunIndex);
            }
        }
    }
}