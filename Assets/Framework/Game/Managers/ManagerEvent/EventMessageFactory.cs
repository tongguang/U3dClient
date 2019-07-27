using UnityEngine;

namespace U3dClient
{
    public class EventMessageFactory<T> where T : IEventMessage, new()
    {
        private static ObjectPool<T> m_Pool;

        #region PublicStaticFunc

        public static T GetEventMessage()
        {
            if (m_Pool == null) m_Pool = new ObjectPool<T>(null, eventMessage => { eventMessage.OnRecycle(); });

            var message = m_Pool.Get();
            return message;
        }

        public static void ReleaseEventMessage(T message)
        {
            m_Pool?.Release(message);
        }

        public static void DebugPoolData()
        {
            Debug.Log(string.Format("poolType:{0} countAll:{1} countActive:{2} countInactive:{3}", typeof(T),
                m_Pool.countAll, m_Pool.countActive, m_Pool.countInactive));
        }

        #endregion
    }
}