using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U3dClient
{
    public class EventManager : IGameManager, IGameUpdate
    {
        private int m_EventIndex = 1;
        private Dictionary<string, Dictionary<int, Action<IEventMessage>>> m_NameToEventActions = new Dictionary<string, Dictionary<int, Action<IEventMessage>>>();
        private Dictionary<int, string> m_EventIndexToName = new Dictionary<int, string>();
        private Dictionary<string, List<IEventMessage>> m_DelayEventMessages = new Dictionary<string, List<IEventMessage>>();
        private Dictionary<string, List<IEventMessage>> m_TempDelayEventMessages = new Dictionary<string, List<IEventMessage>>();
        private Dictionary<int, Action<IEventMessage>> m_TempIndexToEventActions = new Dictionary<int, Action<IEventMessage>>();

        private int GetNewEventIndex()
        {
            return m_EventIndex++;
        }

        public int RegisterEvent(string eventName, Action<IEventMessage> action)
        {
            var index = GetNewEventIndex();
            Dictionary<int, Action<IEventMessage>> actions;
            m_NameToEventActions.TryGetValue(eventName, out actions);
            if (actions == null)
            {

                actions = DictionaryPool<int, Action<IEventMessage>>.Get();
                m_NameToEventActions.Add(eventName, actions);
            }
            actions.Add(index, action);
            m_EventIndexToName.Add(index, eventName);
            return index;
        }

        public void UnRegisterEvent(int eventIndex)
        {
            string eventName;
            m_EventIndexToName.TryGetValue(eventIndex, out eventName);
            if (eventName != null)
            {
                m_EventIndexToName.Remove(eventIndex);
                Dictionary<int, Action<IEventMessage>> actions;
                m_NameToEventActions.TryGetValue(eventName, out actions);
                if (actions != null)
                {
                    actions.Remove(eventIndex);
                    if (actions.Count == 0)
                    {
                        m_NameToEventActions.Remove(eventName);
                        DictionaryPool<int, Action<IEventMessage>>.Release(actions);
                    }
                }
            }
            else
            {
                Debug.LogError($"UnRegisterEvent error  {eventIndex}");
            }
        }

        public void FireEvent(string eventName, IEventMessage eventMessage = null, bool isImmediate = true)
        {
            if (isImmediate)
            {
                Dictionary<int, Action<IEventMessage>> actions;
                m_NameToEventActions.TryGetValue(eventName, out actions);
                m_TempIndexToEventActions.Clear();
                if (actions != null)
                {
                    foreach (var actionPair in actions)
                    {
                        m_TempIndexToEventActions.Add(actionPair.Key, actionPair.Value);
                    }
                }
                foreach (var actionPair in m_TempIndexToEventActions)
                {
                    if (m_EventIndexToName.ContainsKey(actionPair.Key))
                    {
                        actionPair.Value?.Invoke(eventMessage);
                    }
                }
                eventMessage?.ReleaseSelf();
                m_TempIndexToEventActions.Clear();
            }
            else
            {
                List<IEventMessage> eventMessages;
                m_DelayEventMessages.TryGetValue(eventName, out eventMessages);
                if (eventMessages == null)
                {
                    eventMessages = ListPool<IEventMessage>.Get();
                    m_DelayEventMessages.Add(eventName, eventMessages);
                }
                eventMessages.Add(eventMessage);
            }
        }

        private void UpdateDelayFireEvents()
        {
            if (m_DelayEventMessages.Count > 0)
            {
                foreach (var eventMessagesPair in m_TempDelayEventMessages)
                {
                    var eventName = eventMessagesPair.Key;
                    var eventMessages = eventMessagesPair.Value;
                    ListPool<IEventMessage>.Release(eventMessages);
                }
                m_TempDelayEventMessages.Clear();

                foreach (var eventMessagesPair in m_DelayEventMessages)
                {
                    var eventName = eventMessagesPair.Key;
                    var eventMessages = eventMessagesPair.Value;
                    m_TempDelayEventMessages.Add(eventName, eventMessages);

                }
                m_DelayEventMessages.Clear();

                foreach (var eventMessagesPair in m_TempDelayEventMessages)
                {
                    var eventName = eventMessagesPair.Key;
                    var eventMessages = eventMessagesPair.Value;
                    Dictionary<int, Action<IEventMessage>> actions;
                    m_NameToEventActions.TryGetValue(eventName, out actions);
                    m_TempIndexToEventActions.Clear();
                    if (actions != null)
                    {
                        foreach (var actionPair in actions)
                        {
                            m_TempIndexToEventActions.Add(actionPair.Key, actionPair.Value);
                        }
                    }
                    foreach (var eventMessage in eventMessages)
                    {
                        foreach (var actionPair in m_TempIndexToEventActions)
                        {
                            if (m_EventIndexToName.ContainsKey(actionPair.Key))
                            {
                                actionPair.Value?.Invoke(eventMessage);
                            }
                        }
                        eventMessage?.ReleaseSelf();
                    }
                    m_TempIndexToEventActions.Clear();
                    ListPool<IEventMessage>.Release(eventMessages);
                }
                m_TempDelayEventMessages.Clear();
            }
        }

        public void Awake()
        {
        }

        public void Start()
        {
        }

        public void Update()
        {
            UpdateDelayFireEvents();
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

