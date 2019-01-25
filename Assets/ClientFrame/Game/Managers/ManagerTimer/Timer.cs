using System;
using UnityEngine;

namespace U3dClient
{
    public class Timer
    {
        #region PrivateVal

        private Action m_OnComplete;
        private Action<float> m_OnUpdate;
        private float m_StartTime;
        private float m_LastUpdateTime;
        private float? m_TimeElapsedBeforeCancel;
        private float? m_TimeElapsedBeforePause;

        #endregion

        #region PublicVal

        public int TimerIndex { get; private set; }

        public float Duration { get; private set; }

        public bool IsLooped { get; set; }

        public bool IsCompleted { get; private set; }

        public bool UsesRealTime { get; private set; }

        public bool IsPaused => m_TimeElapsedBeforePause.HasValue;

        public bool IsCancelled => m_TimeElapsedBeforeCancel.HasValue;

        public bool IsDone => IsCompleted || IsCancelled;

        #endregion

        #region PublicFunc

        public void Cancel()
        {
            if (IsDone) return;

            m_TimeElapsedBeforeCancel = GetTimeElapsed();
            m_TimeElapsedBeforePause = null;
        }

        public void Pause()
        {
            if (IsPaused || IsDone) return;

            m_TimeElapsedBeforePause = GetTimeElapsed();
        }

        public void Resume()
        {
            if (!IsPaused || IsDone) return;

            m_TimeElapsedBeforePause = null;
        }

        public float GetTimeElapsed()
        {
            if (IsCompleted || GetWorldTime() >= GetFireTime()) return Duration;

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

        public void OnReuse()
        {
            ResetData();
        }

        public void OnRecycle()
        {
            ResetData();
        }

        public void ResetData()
        {
            TimerIndex = 0;
            m_OnComplete = null;
            m_OnUpdate = null;
            m_StartTime = 0;
            m_LastUpdateTime = 0;
            m_TimeElapsedBeforeCancel = null;
            m_TimeElapsedBeforePause = null;
            Duration = 0;
            IsLooped = false;
            IsCompleted = false;
            UsesRealTime = false;
        }

        public void Init(int timerIndex, float duration, Action onComplete, Action<float> onUpdate,
            bool isLooped, bool usesRealTime)
        {
            TimerIndex = timerIndex;

            Duration = duration;
            m_OnComplete = onComplete;
            m_OnUpdate = onUpdate;

            IsLooped = isLooped;
            UsesRealTime = usesRealTime;

            m_StartTime = GetWorldTime();
            m_LastUpdateTime = m_StartTime;
        }

        public void Update()
        {
            if (IsDone) return;

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
                    m_StartTime = GetWorldTime();
                else
                    IsCompleted = true;
            }
        }

        #endregion

        #region PrivateFunc

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

        #endregion
    }
}