using System.Collections.Generic;
using U3dClient.Update;

namespace U3dClient.FsmMgr
{
    public static class FsmManager
    {
        private static Updater s_FsmUpdater = new Updater();

        public static T CreateFsm<T>(Dictionary<int, IFsmState> stateDict, int initStateID) where T:FsmBase, new()
        {
            T fsm = new T();
            s_FsmUpdater.AddUpdate(fsm);
            fsm.Init(stateDict, initStateID);
            return fsm;
        }

        public static void ReleaseFsm(FsmBase fsm)
        {
            s_FsmUpdater.RemoveUpdate(fsm);
            fsm.Release();
        }

        public static void Update()
        {
            s_FsmUpdater.Update();
        }
    }
}