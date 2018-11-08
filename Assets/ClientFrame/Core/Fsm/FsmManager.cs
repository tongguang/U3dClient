using System.Collections.Generic;
using U3dClient.Update;

namespace U3dClient.Fsm
{
    public static class FsmManager
    {
        private static Updater s_FsmUpdater = new Updater();

        public static T CreateFsm<T>(Dictionary<int, IFsmState> stateDict, int initStateID) where T:FsmBase, new()
        {
            var fsm = s_FsmUpdater.CreateItem<T>();
            fsm.Init(stateDict, initStateID);
            return fsm;
        }

        public static void ReleaseFsm(FsmBase fsm)
        {
            s_FsmUpdater.ReleaseItem(fsm);
            fsm.Release();
        }

        public static void Update()
        {
            s_FsmUpdater.Update();
        }
    }
}