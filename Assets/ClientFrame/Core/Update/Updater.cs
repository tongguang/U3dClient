using System.Collections.Generic;

namespace U3dClient.Update
{
    public class Updater
    {
        private bool m_IsUpdate = false;
        private Dictionary<int, UpdateItemBase> m_UpdateDict = new Dictionary<int, UpdateItemBase>();
        private Dictionary<int, UpdateItemBase> m_DelayAddUpdateDict = new Dictionary<int, UpdateItemBase>();
        private Dictionary<int, UpdateItemBase> m_DelayRemoveUpdateDict = new Dictionary<int, UpdateItemBase>();

        private int m_UpdateIndex = 0;


        public int GetNewUpdateIndex()
        {
            return m_UpdateIndex++;
        }

        public void AddUpdate(UpdateItemBase updateItem) 
        {
            var newIndex = GetNewUpdateIndex();
            if (m_IsUpdate)
            {
                m_DelayAddUpdateDict.Add(newIndex, updateItem);
            }
            else
            {
                m_UpdateDict.Add(newIndex, updateItem);
            }

            updateItem.UpdateIndex = newIndex;
        }

        public void RemoveUpdate(UpdateItemBase updateItem)
        {
            RemoveUpdate(updateItem.UpdateIndex);
        }

        public void RemoveUpdate(int updateIndex)
        {
            UpdateItemBase updateItem;
            m_UpdateDict.TryGetValue(updateIndex, out updateItem);
            if (updateItem != null)
            {
                if (m_IsUpdate)
                {
                    m_DelayRemoveUpdateDict.Add(updateIndex, updateItem);
                }
                else
                {
                    m_DelayRemoveUpdateDict.Remove(updateIndex);
                }

                return;
            }
            m_DelayAddUpdateDict.TryGetValue(updateIndex, out updateItem);
            if (updateItem != null)
            {
                m_DelayAddUpdateDict.Remove(updateIndex);
            }
        }

        public UpdateItemBase GetUpdate(int updateIndex)
        {
            UpdateItemBase updateItem;
            m_UpdateDict.TryGetValue(updateIndex, out updateItem);
            if (updateItem != null)
            {
                return updateItem;
            }
            m_DelayAddUpdateDict.TryGetValue(updateIndex, out updateItem);
            return updateItem;
        }

        public void Update()
        {
            m_IsUpdate = true;

            foreach (var update in m_UpdateDict)
            {
                if (!m_DelayRemoveUpdateDict.ContainsKey(update.Key))
                {
                    update.Value.Update();
                }
            }

            if (m_DelayAddUpdateDict.Count > 0)
            {
                foreach (var update in m_DelayAddUpdateDict)
                {
                    m_UpdateDict.Add(update.Key, update.Value);
                }
                m_DelayAddUpdateDict.Clear();
            }

            if (m_DelayRemoveUpdateDict.Count > 0)
            {
                foreach (var update in m_DelayRemoveUpdateDict)
                {
                    m_UpdateDict.Remove(update.Key);
                }
                m_DelayRemoveUpdateDict.Clear();
            }

            m_IsUpdate = false;
        }
    }
}