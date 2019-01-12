// 魔改自 https://github.com/akbiggs/UnityTimer

using System;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public class TimerManager : IGameManager
    {
        private ObjectPool<Timer> m_TimerPool = new ObjectPool<Timer>((timer) => { timer.OnReuse();}, (timer) => { timer.OnRecycle(); });

        private int m_TimerIndex = 1;
        private Dictionary<int, Timer> m_Timers = new Dictionary<int, Timer>();
        private Dictionary<int, Timer> m_TimersToAdd = new Dictionary<int, Timer>();
        private List<Timer> m_TimersDone = new List<Timer>();

        public int RegisterTimer(float duration, Action onComplete, Action<float> onUpdate = null,
            bool isLooped = false, bool useRealTime = false)
        {
            var index = GetNewTimerIndex();
            Timer timer = m_TimerPool.Get();
            timer.Init(index, duration, onComplete, onUpdate, isLooped, useRealTime);
            m_TimersToAdd.Add(index, timer);
            return index;
        }

        public void CancelTimer(int timerIndex)
        {
            var timer = GetTimer(timerIndex);
            timer?.Cancel();
        }

        public Timer GetTimer(int timerIndex)
        {
            Timer timer;
            m_Timers.TryGetValue(timerIndex, out timer);
            if (timer == null)
            {
                m_TimersToAdd.TryGetValue(timerIndex, out timer);
            }
            return timer;
        }

        public void PauseTimer(int timerIndex)
        {
            var timer = GetTimer(timerIndex);
            timer?.Pause();
        }

        public void ResumeTimer(int timerIndex)
        {
            var timer = GetTimer(timerIndex);
            timer?.Resume();
        }

        public void CancelAllTimers()
        {
            foreach (var timerPair in m_Timers)
            {
                timerPair.Value.Cancel();
            }
            foreach (var timerPair in m_Timers)
            {
                m_TimerPool.Release(timerPair.Value);
            }
            foreach (var timerPair in m_TimersToAdd)
            {
                m_TimerPool.Release(timerPair.Value);
            }
            m_Timers.Clear();
            m_TimersToAdd.Clear();
        }

        public void PauseAllTimers()
        {
            foreach (var timerPair in m_Timers)
            {
                timerPair.Value.Pause();
            }
            foreach (var timerPair in m_TimersToAdd)
            {
                timerPair.Value.Pause();
            }
        }

        public void ResumeAllTimers()
        {
            foreach (var timerPair in m_Timers)
            {
                timerPair.Value.Resume();
            }
            foreach (var timerPair in m_TimersToAdd)
            {
                timerPair.Value.Resume();
            }
        }

        private int GetNewTimerIndex()
        {
            return m_TimerIndex++;
        }

        private void UpdateAllTimers()
        {
            if (m_TimersToAdd.Count > 0)
            {
                foreach (var timerPair in m_TimersToAdd)
                {
                    m_Timers.Add(timerPair.Key, timerPair.Value);
                }
                m_TimersToAdd.Clear();
            }

            foreach (var timerPair in m_Timers)
            {
                var timer = timerPair.Value;
                timer.Update();
                if (timer.IsDone)
                {
                    m_TimersDone.Add(timer);
                }
            }

            foreach (var timer in m_TimersDone)
            {
                m_TimerPool.Release(timer);
                m_Timers.Remove(timer.TimerIndex);
            }
            m_TimersDone.Clear();
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