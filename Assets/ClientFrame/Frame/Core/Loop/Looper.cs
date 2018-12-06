using System;
using System.Collections.Generic;

namespace U3dClient.Frame
{
    public class LoopItem
    {
        public int Index = 0;
        public bool IsValid = false;
        public int Priority = 0;
        public Action UpdateAction = null;
    }

    public class Looper
    {
        private ObjectPool<LoopItem> LoopItemPool = new ObjectPool<LoopItem>(null, (item) =>
        {
            item.Index = 0;
            item.IsValid = false;
            item.UpdateAction = null;
            item.Priority = 0;
        });
        private Dictionary<int, LoopItem> m_LoopDict = new Dictionary<int, LoopItem>();
        private List<LoopItem> m_TempLoopList = new List<LoopItem>();

        private int m_UpdateIndex = 0;


        public int GetNewUpdateIndex()
        {
            return m_UpdateIndex++;
        }

        public int AddLoopAction(Action action, int priority = 0)
        {
            var newIndex = GetNewUpdateIndex();
            var item = LoopItemPool.Get();
            item.Index = newIndex;
            item.IsValid = true;
            item.UpdateAction = action;
            item.Priority = priority;
            m_LoopDict.Add(newIndex, item);
            return newIndex;
        }

        public void RemoveLoopAction(int index)
        {
            LoopItem loopItem;
            m_LoopDict.TryGetValue(index, out loopItem);
            if (loopItem != null)
            {
                m_LoopDict.Remove(index);
                LoopItemPool.Release(loopItem);
            }
        }

        public LoopItem GetLoop(int index)
        {
            LoopItem loopItem;
            m_LoopDict.TryGetValue(index, out loopItem);
            return loopItem;
        }

        public void Update()
        {
            m_TempLoopList.Clear();
            foreach (var loop in m_LoopDict)
            {
                m_TempLoopList.Add(loop.Value);
            }

            foreach (var loop in m_TempLoopList)
            {
                if (loop.IsValid && loop.UpdateAction != null)
                {
                    loop.UpdateAction();
                }
            }
        }
    }
}