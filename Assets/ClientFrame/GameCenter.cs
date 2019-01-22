using System.Collections.Generic;

namespace U3dClient
{
    public static class GameCenter
    {
        private static List<IGameManager> m_ManagerList = new List<IGameManager>();
        private static Dictionary<string, IGameManager> m_ManagerDict = new Dictionary<string, IGameManager>();

        public static TimerManager s_TimerManager { get; private set; }

        public static EventManager s_EventManager { get; private set; }

        public static ResourceManager s_ResourceManager { get; private set; }

        public static UpgradeManager s_UpgradeManager { get; private set; }

        public static ScriptManager s_ScriptManager { get; private set; }

        public static GameFlowManager s_GameFlowManager { get; private set; }

        public static void Awake()
        {
            #region addManager

            s_TimerManager = AddManager<TimerManager>();
            s_EventManager = AddManager<EventManager>();
            s_ResourceManager = AddManager<ResourceManager>();
            s_UpgradeManager = AddManager<UpgradeManager>();
            s_ScriptManager = AddManager<ScriptManager>();
            s_GameFlowManager = AddManager<GameFlowManager>();

            #endregion

            foreach (var manager in m_ManagerList)
            {
                manager.Awake();
            }
        }

        public static void Start()
        {
            foreach (var manager in m_ManagerList)
            {
                manager.Start();
            }
        }

        public static void Update()
        {
            foreach (var manager in m_ManagerList)
            {
                manager.Update();
            }
        }

        public static void FixedUpdate()
        {
            foreach (var manager in m_ManagerList)
            {
                manager.FixedUpdate();
            }
        }

        public static void OnApplicationFocus(bool hasFocus)
        {
            foreach (var manager in m_ManagerList)
            {
                manager.OnApplicationFocus(hasFocus);
            }
        }

        public static void OnApplicationPause(bool pauseStatus)
        {
            foreach (var manager in m_ManagerList)
            {
                manager.OnApplicationPause(pauseStatus);
            }
        }

        public static void OnDestroy()
        {
            for (int i = m_ManagerList.Count - 1; i >= 0; i--)
            {
                var manager = m_ManagerList[i];
                manager.OnDestroy();
            }
        }

        public static void OnApplicationQuit()
        {
            for (int i = m_ManagerList.Count - 1; i >= 0; i--)
            {
                var manager = m_ManagerList[i];
                manager.OnApplicationQuit();
            }
        }

        public static T AddManager<T>() where T: class, IGameManager, new()
        {
            string managerName = typeof(T).Name;
            if (m_ManagerDict.ContainsKey(managerName))
            {
                return m_ManagerDict[managerName] as T;
            }
            var manager = new T();
            m_ManagerDict.Add(managerName, manager);
            m_ManagerList.Add(manager);
            return manager;
        }

        public static T GetManager<T>() where T : class, IGameManager
        {
            string managerName = typeof(T).Name;
            IGameManager manager;
            m_ManagerDict.TryGetValue(managerName, out manager);
            return manager as T;
        }
    }
}