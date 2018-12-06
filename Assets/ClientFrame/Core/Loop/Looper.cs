using System.Collections.Generic;

namespace U3dClient.Frame
{
    public class Looper
    {
        private bool m_IsUpdate = false;
        private Dictionary<int, LoopItemBase> m_LoopDict = new Dictionary<int, LoopItemBase>();
        private List<LoopItemBase> m_TempLoopList = new List<LoopItemBase>();

        private int m_UpdateIndex = 0;


        public int GetNewUpdateIndex()
        {
            return m_UpdateIndex++;
        }

        public T CreateItem<T>() where T: LoopItemBase, new()
        {
            var newIndex = GetNewUpdateIndex();
            var updateItem = new T();
            m_LoopDict.Add(newIndex, updateItem);
            updateItem.UpdateIndex = newIndex;
            updateItem.IsValid = true;
            return updateItem;
        }

        public void ReleaseItem(LoopItemBase loopItem)
        {
            ReleaseItem(loopItem.UpdateIndex);
        }

        public void ReleaseItem(int updateIndex)
        {
            LoopItemBase loopItem;
            m_LoopDict.TryGetValue(updateIndex, out loopItem);
            if (loopItem != null)
            {
                m_LoopDict.Remove(updateIndex);
                loopItem.IsValid = false;
            }
        }

        public LoopItemBase GetUpdate(int updateIndex)
        {
            LoopItemBase loopItem;
            m_LoopDict.TryGetValue(updateIndex, out loopItem);
            return loopItem;
        }

        public void Update()
        {
            m_TempLoopList.Clear();
            foreach (var updateItemBase in m_LoopDict)
            {
                m_TempLoopList.Add(updateItemBase.Value);
            }

            foreach (var updateItemBase in m_TempLoopList)
            {
                if (updateItemBase.IsValid)
                {
                    updateItemBase.Update();
                }
            }
        }
    }
}