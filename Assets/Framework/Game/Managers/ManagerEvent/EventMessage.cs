namespace U3dClient
{
    public interface IEventMessage
    {
        //        void OnReuse();
        void OnRecycle();
        void ReleaseSelf();
    }

    public class EventMessage<T1> : IEventMessage
    {
        public T1 Value1;

        #region IEventMessage

        public void OnRecycle()
        {
            Value1 = default(T1);
        }

        public void ReleaseSelf()
        {
            ReleaseEventMessage(this);
        }

        #endregion

        #region PublicStaticFunc

        public static EventMessage<T1> GetEventMessage()
        {
            var message = EventMessageFactory<EventMessage<T1>>.GetEventMessage();
            return message;
        }

        private static void ReleaseEventMessage(EventMessage<T1> message)
        {
            EventMessageFactory<EventMessage<T1>>.ReleaseEventMessage(message);
        }

        #endregion
    }

    public class EventMessage<T1, T2> : IEventMessage
    {
        public T1 Value1;
        public T2 Value2;

        #region IEventMessage

        public void OnRecycle()
        {
            Value1 = default(T1);
            Value2 = default(T2);
        }

        public void ReleaseSelf()
        {
            ReleaseEventMessage(this);
        }

        #endregion

        #region PublicStaticFunc

        public static EventMessage<T1, T2> GetEventMessage()
        {
            var message = EventMessageFactory<EventMessage<T1, T2>>.GetEventMessage();
            return message;
        }

        private static void ReleaseEventMessage(EventMessage<T1, T2> message)
        {
            EventMessageFactory<EventMessage<T1, T2>>.ReleaseEventMessage(message);
        }

        #endregion

    }

    public class EventMessage<T1, T2, T3> : IEventMessage
    {
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;

        #region IEventMessage

        public void OnRecycle()
        {
            Value1 = default(T1);
            Value2 = default(T2);
            Value3 = default(T3);
        }

        public void ReleaseSelf()
        {
            ReleaseEventMessage(this);
        }

        #endregion

        #region PublicStaticFunc

        public static EventMessage<T1, T2, T3> GetEventMessage()
        {
            var message = EventMessageFactory<EventMessage<T1, T2, T3>>.GetEventMessage();
            return message;
        }

        private static void ReleaseEventMessage(EventMessage<T1, T2, T3> message)
        {
            EventMessageFactory<EventMessage<T1, T2, T3>>.ReleaseEventMessage(message);
        }

        #endregion

    }

    public class EventMessage<T1, T2, T3, T4> : IEventMessage
    {
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
        public T4 Value4;

        #region IEventMessage

        public void OnRecycle()
        {
            Value1 = default(T1);
            Value2 = default(T2);
            Value3 = default(T3);
            Value4 = default(T4);
        }

        public void ReleaseSelf()
        {
            ReleaseEventMessage(this);
        }

        #endregion

        #region PublicStaticFunc

        public static EventMessage<T1, T2, T3, T4> GetEventMessage()
        {
            var message = EventMessageFactory<EventMessage<T1, T2, T3, T4>>.GetEventMessage();
            return message;
        }

        private static void ReleaseEventMessage(EventMessage<T1, T2, T3, T4> message)
        {
            EventMessageFactory<EventMessage<T1, T2, T3, T4>>.ReleaseEventMessage(message);
        }

        #endregion

    }
}