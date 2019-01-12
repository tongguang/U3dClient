using System.Collections.Generic;

namespace U3dClient
{
    public static class GameCenter
    {
        private static List<IGameManager> m_ManagerList = new List<IGameManager>();
        private static Dictionary<string, IGameManager> m_ManagerDict = new Dictionary<string, IGameManager>();

        public static TimerManager s_TimerManager
        {
            get
            {
                return GetManager<TimerManager>();
            }
        }

        public static ResourceManager s_ResourceManager
        {
            get
            {
                return GetManager<ResourceManager>();
            }
        }

        public static UpgradeManager s_UpgradeManager
        {
            get
            {
                return GetManager<UpgradeManager>();
            }
        }

        public static ScriptManager s_ScriptManager
        {
            get
            {
                return GetManager<ScriptManager>();
            }
        }

        public static GameFlowManager s_GameFlowManager
        {
            get
            {
                return GetManager<GameFlowManager>();
            }
        }

        public static void Awake()
        {
            #region addManager

            AddManager<TimerManager>();
            AddManager<ResourceManager>();
            AddManager<UpgradeManager>();
            AddManager<ScriptManager>();
            AddManager<GameFlowManager>();

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

        public static void AddManager<T>() where T: class, IGameManager, new()
        {
            string managerName = typeof(T).Name;
            if (m_ManagerDict.ContainsKey(managerName))
            {
                return;
            }
            var manager = new T();
            m_ManagerDict.Add(managerName, manager);
            m_ManagerList.Add(manager);
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