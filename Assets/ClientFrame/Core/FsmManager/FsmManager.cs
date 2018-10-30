using System.Collections.Generic;

namespace U3dClient.FsmMgr
{
    public static class FsmManager
    {
        public static bool s_IsUpdate = false;
        public static Dictionary<int, FsmBase> s_FsmDict = new Dictionary<int, FsmBase>();
        public static Dictionary<int, FsmBase> s_DelayAddFsmDict = new Dictionary<int, FsmBase>();
        public static Dictionary<int, FsmBase> s_DelayRemoveFsmDict = new Dictionary<int, FsmBase>();

        private static int s_FsmIndex = 0;


        public static int GetNewFsmIndex()
        {
            return s_FsmIndex++;
        }

        public static T CreateFsm<T>(Dictionary<int, IFsmState> stateDict, int initStateID) where T:FsmBase, new()
        {
            T fsm = new T();
            var newIndex = GetNewFsmIndex();
            if (s_IsUpdate)
            {
                s_DelayAddFsmDict.Add(newIndex, fsm);
            }
            else
            {
                s_FsmDict.Add(newIndex, fsm);
            }
            fsm.Init(newIndex, stateDict, initStateID);

            return fsm;
        }

        public static void ReleaseFsm(int fsmIndex)
        {
            FsmBase fsm;
            s_FsmDict.TryGetValue(fsmIndex, out fsm);
            if (fsm != null)
            {
                if (!fsm.IsVaild)
                {
                    return;
                }
                fsm.Release();
                if (s_IsUpdate)
                {
                    s_DelayRemoveFsmDict.Add(fsmIndex, fsm);
                }
                else
                {
                    s_FsmDict.Remove(fsmIndex);
                }

                return;
            }
            s_DelayAddFsmDict.TryGetValue(fsmIndex, out fsm);
            if (fsm != null)
            {
                if (!fsm.IsVaild)
                {
                    return;
                }
                fsm.Release();
                s_DelayAddFsmDict.Remove(fsmIndex);
            }
        }

        public static void Update()
        {
            s_IsUpdate = true;

            foreach (var fsm in s_FsmDict)
            {
                if (fsm.Value.IsVaild)
                {
                    fsm.Value.Update();
                }
            }

            if (s_DelayAddFsmDict.Count > 0)
            {
                foreach (var fsm in s_DelayAddFsmDict)
                {
                    s_FsmDict.Add(fsm.Key, fsm.Value);
                }
                s_DelayAddFsmDict.Clear();
            }

            if (s_DelayRemoveFsmDict.Count > 0)
            {
                foreach (var fsm in s_DelayRemoveFsmDict)
                {
                    s_FsmDict.Remove(fsm.Key);
                }
                s_DelayRemoveFsmDict.Clear();
            }

            s_IsUpdate = false;
        }
    }
}