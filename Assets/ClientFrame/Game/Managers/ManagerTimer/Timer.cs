using System;
using UnityEngine;

namespace U3dClient
{
    public class Timer
    {

        public float Duration { get; private set; }

        public bool IsLooped { get; set; }

        public bool IsCompleted { get; private set; }

        public bool UsesRealTime { get; private set; }

        public bool IsPaused
        {
            get { return m_TimeElapsedBeforePause.HasValue; }
        }

        public bool IsCancelled
        {
            get { return m_TimeElapsedBeforeCancel.HasValue; }
        }

        public bool IsDone
        {
            get { return IsCompleted || IsCancelled; }
        }

        public void Cancel()
        {
            if (IsDone)
            {
                return;
            }

            m_TimeElapsedBeforeCancel = GetTimeElapsed();
            m_TimeElapsedBeforePause = null;
        }

        public void Pause()
        {
            if (IsPaused || IsDone)
            {
                return;
            }

            m_TimeElapsedBeforePause = GetTimeElapsed();
        }

        public void Resume()
        {
            if (!IsPaused || IsDone)
            {
                return;
            }

            m_TimeElapsedBeforePause = null;
        }

        public float GetTimeElapsed()
        {
            if (IsCompleted || GetWorldTime() >= GetFireTime())
            {
                return Duration;
            }

            return m_TimeElapsedBeforeCancel ??
                   m_TimeElapsedBeforePause ??
                   GetWorldTime() - m_StartTime;
        }

        public float GetTimeRemaining()
        {
            return Duration - GetTimeElapsed();
        }

        public float GetRatioComplete()
        {
            return GetTimeElapsed() / Duration;
        }


        public float GetRatioRemaining()
        {
            return GetTimeRemaining() / Duration;
        }

        private readonly Action m_OnComplete;
        private readonly Action<float> m_OnUpdate;
        private float m_StartTime;
        private float m_LastUpdateTime;

        private float? m_TimeElapsedBeforeCancel;
        private float? m_TimeElapsedBeforePause;


        public Timer(float duration, Action mOnComplete, Action<float> mOnUpdate,
            bool isLooped, bool usesRealTime)
        {
            Duration = duration;
            m_OnComplete = mOnComplete;
            m_OnUpdate = mOnUpdate;

            IsLooped = isLooped;
            UsesRealTime = usesRealTime;


            m_StartTime = GetWorldTime();
            m_LastUpdateTime = m_StartTime;
        }

        private float GetWorldTime()
        {
            return UsesRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime()
        {
            return m_StartTime + Duration;
        }

        private float GetTimeDelta()
        {
            return GetWorldTime() - m_LastUpdateTime;
        }

        public void Update()
        {
            if (IsDone)
            {
                return;
            }

            if (IsPaused)
            {
                m_StartTime += GetTimeDelta();
                m_LastUpdateTime = GetWorldTime();
                return;
            }

            m_LastUpdateTime = GetWorldTime();

            m_OnUpdate?.Invoke(GetTimeElapsed());

            if (GetWorldTime() >= GetFireTime())
            {
                m_OnComplete?.Invoke();

                if (IsLooped)
                {
                    m_StartTime = GetWorldTime();
                }
                else
                {
                    IsCompleted = true;
                }
            }
        }
    }
}