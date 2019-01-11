// https://github.com/akbiggs/UnityTimer

using UnityEngine;
using System;
using System.Collections.Generic;
using U3dClient;

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
        if (this.IsPaused || this.IsDone)
        {
            return;
        }

        this.m_TimeElapsedBeforePause = this.GetTimeElapsed();
    }

    public void Resume()
    {
        if (!this.IsPaused || this.IsDone)
        {
            return;
        }

        this.m_TimeElapsedBeforePause = null;
    }

    public float GetTimeElapsed()
    {
        if (this.IsCompleted || this.GetWorldTime() >= this.GetFireTime())
        {
            return this.Duration;
        }

        return this.m_TimeElapsedBeforeCancel ??
               this.m_TimeElapsedBeforePause ??
               this.GetWorldTime() - this.m_StartTime;
    }

    public float GetTimeRemaining()
    {
        return this.Duration - this.GetTimeElapsed();
    }

    public float GetRatioComplete()
    {
        return this.GetTimeElapsed() / this.Duration;
    }


    public float GetRatioRemaining()
    {
        return this.GetTimeRemaining() / this.Duration;
    }

    private readonly Action _onComplete;
    private readonly Action<float> _onUpdate;
    private float m_StartTime;
    private float m_LastUpdateTime;

    private float? m_TimeElapsedBeforeCancel;
    private float? m_TimeElapsedBeforePause;


    private Timer(float duration, Action onComplete, Action<float> onUpdate,
        bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
    {
        this.Duration = duration;
        this._onComplete = onComplete;
        this._onUpdate = onUpdate;

        this.IsLooped = isLooped;
        this.UsesRealTime = usesRealTime;


        this.m_StartTime = this.GetWorldTime();
        this.m_LastUpdateTime = this.m_StartTime;
    }

    private float GetWorldTime()
    {
        return this.UsesRealTime ? Time.realtimeSinceStartup : Time.time;
    }

    private float GetFireTime()
    {
        return this.m_StartTime + this.Duration;
    }

    private float GetTimeDelta()
    {
        return this.GetWorldTime() - this.m_LastUpdateTime;
    }

    private void Update()
    {
        if (this.IsDone)
        {
            return;
        }

        if (this.IsPaused)
        {
            this.m_StartTime += this.GetTimeDelta();
            this.m_LastUpdateTime = this.GetWorldTime();
            return;
        }

        this.m_LastUpdateTime = this.GetWorldTime();

        if (this._onUpdate != null)
        {
            this._onUpdate(this.GetTimeElapsed());
        }

        if (this.GetWorldTime() >= this.GetFireTime())
        {
            _onComplete?.Invoke();

            if (this.IsLooped)
            {
                this.m_StartTime = this.GetWorldTime();
            }
            else
            {
                this.IsCompleted = true;
            }
        }
    }

    private class TimerManager : IGameManager
    {
        private List<Timer> m_Timers = new List<Timer>();
        private List<Timer> m_TimersToAdd = new List<Timer>();

        public Timer RegisterTimer(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            Timer timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
            m_TimersToAdd.Add(timer);
            return timer;
        }

        public void CancelTimer(Timer timer)
        {
            timer?.Cancel();
        }

        public static void PauseTimer(Timer timer)
        {
            timer?.Pause();
        }

        public static void ResumeTimer(Timer timer)
        {
            timer?.Resume();
        }

        public void CancelAllTimers()
        {
            foreach (Timer timer in m_Timers)
            {
                timer.Cancel();
            }

            m_Timers.Clear();
            m_TimersToAdd.Clear();
        }

        public void PauseAllTimers()
        {
            foreach (Timer timer in m_Timers)
            {
                timer.Pause();
            }
        }

        public void ResumeAllTimers()
        {
            foreach (Timer timer in m_Timers)
            {
                timer.Resume();
            }
        }

        private void UpdateAllTimers()
        {
            if (this.m_TimersToAdd.Count > 0)
            {
                this.m_Timers.AddRange(this.m_TimersToAdd);
                this.m_TimersToAdd.Clear();
            }

            foreach (Timer timer in this.m_Timers)
            {
                timer.Update();
            }

            this.m_Timers.RemoveAll(t => t.IsDone);
        }

        public void Awake()
        {
        }

        public void Start()
        {
        }

        public void Update()
        {
            this.UpdateAllTimers();
        }

        public void FixedUpdate()
        {
        }

        public void OnApplicationFocus(bool hasFocus)
        {
        }

        public void OnApplicationPause(bool pauseStatus)
        {
        }

        public void OnDestroy()
        {
        }

        public void OnApplicationQuit()
        {
        }
    }
}