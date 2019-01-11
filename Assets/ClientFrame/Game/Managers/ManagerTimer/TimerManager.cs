// 魔改自 https://github.com/akbiggs/UnityTimer

using System;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public class TimerManager : IGameManager
    {
        private List<Timer> m_Timers = new List<Timer>();
        private List<Timer> m_TimersToAdd = new List<Timer>();

        public Timer RegisterTimer(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool useRealTime = false)
        {
            Timer timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime);
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
            if (m_TimersToAdd.Count > 0)
            {
                m_Timers.AddRange(m_TimersToAdd);
                m_TimersToAdd.Clear();
            }

            foreach (Timer timer in m_Timers)
            {
                timer.Update();
            }

            m_Timers.RemoveAll(t => t.IsDone);
        }

        public void Awake()
        {
        }

        public void Start()
        {
        }

        public void Update()
        {
            UpdateAllTimers();
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