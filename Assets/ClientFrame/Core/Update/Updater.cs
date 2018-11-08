using System.Collections.Generic;

namespace U3dClient.Update
{
    public class Updater
    {
        private bool m_IsUpdate = false;
        private Dictionary<int, UpdateItemBase> m_UpdateDict = new Dictionary<int, UpdateItemBase>();
        private List<UpdateItemBase> m_TempUpdateList = new List<UpdateItemBase>();

        private int m_UpdateIndex = 0;


        public int GetNewUpdateIndex()
        {
            return m_UpdateIndex++;
        }

        public T CreateItem<T>() where T: UpdateItemBase, new()
        {
            var newIndex = GetNewUpdateIndex();
            var updateItem = new T();
            m_UpdateDict.Add(newIndex, updateItem);
            updateItem.UpdateIndex = newIndex;
            updateItem.IsValid = true;
            return updateItem;
        }

        public void ReleaseItem(UpdateItemBase updateItem)
        {
            ReleaseItem(updateItem.UpdateIndex);
        }

        public void ReleaseItem(int updateIndex)
        {
            UpdateItemBase updateItem;
            m_UpdateDict.TryGetValue(updateIndex, out updateItem);
            if (updateItem != null)
            {
                m_UpdateDict.Remove(updateIndex);
                updateItem.IsValid = false;
            }
        }

        public UpdateItemBase GetUpdate(int updateIndex)
        {
            UpdateItemBase updateItem;
            m_UpdateDict.TryGetValue(updateIndex, out updateItem);
            return updateItem;
        }

        public void Update()
        {
            m_TempUpdateList.Clear();
            foreach (var updateItemBase in m_UpdateDict)
            {
                m_TempUpdateList.Add(updateItemBase.Value);
            }

            foreach (var updateItemBase in m_TempUpdateList)
            {
                if (updateItemBase.IsValid)
                {
                    updateItemBase.Update();
                }
            }
        }
    }
}