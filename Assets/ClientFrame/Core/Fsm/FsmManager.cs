using System.Collections.Generic;

namespace U3dClient.Frame
{
    public static class FsmManager
    {
        private static Looper _sFsmLooper = new Looper();

        public static T CreateFsm<T>(Dictionary<int, IFsmState> stateDict, int initStateID) where T:FsmBase, new()
        {
            var fsm = _sFsmLooper.CreateItem<T>();
            fsm.Init(stateDict, initStateID);
            return fsm;
        }

        public static void ReleaseFsm(FsmBase fsm)
        {
            _sFsmLooper.ReleaseItem(fsm);
            fsm.Release();
        }

        public static void Update()
        {
            _sFsmLooper.Update();
        }
    }
}