using System;
using System.Collections.Generic;

namespace U3dClient
{
    public class LoopContain<T1, T2>
    {
        private Dictionary<T1, T2> m_Items = new Dictionary<T1, T2>();
        private Dictionary<T1, T2> m_ItemsToAdd = new Dictionary<T1, T2>();
        private HashSet<T1> m_ItemKeysToRemove = new HashSet<T1>();
        private Action<T2> m_ForeachAction = null;
        private Action<T2> m_RemoveItemAction = null;

        public void SetForeachAction(Action<T2> action)
        {
            m_ForeachAction = action;
        }

        public void SetRemoveItemAction(Action<T2> action)
        {
            m_RemoveItemAction = action;
        }

        public void AddLoop(T1 key, T2 value)
        {
            m_ItemsToAdd.Add(key, value);
        }

        public void RemoveLoop(T1 key)
        {
            T2 item;
            m_ItemsToAdd.TryGetValue(key, out item);
            if (item != null)
            {
                m_RemoveItemAction?.Invoke(item);
                m_ItemsToAdd.Remove(key);
                return;
            }
            m_ItemKeysToRemove.Add(key);
        }

        public void Foreach()
        {
            foreach (var itemPair in m_ItemsToAdd)
            {
                m_Items.Add(itemPair.Key, itemPair.Value);
            }
            m_ItemsToAdd.Clear();
            foreach (var itemPair in m_Items)
            {
                if (!m_ItemKeysToRemove.Contains(itemPair.Key))
                {
                    m_ForeachAction(itemPair.Value);
                }
            }

            foreach (var itemKey in m_ItemKeysToRemove)
            {
                var item = m_Items[itemKey];
                m_RemoveItemAction(item);
                m_Items.Remove(itemKey);
            }
            m_ItemKeysToRemove.Clear();
        }

        public void Clear(bool isImmediate = true)
        {
            foreach (var itemPair in m_ItemsToAdd)
            {
                RemoveLoop(itemPair.Key);
            }
            foreach (var itemPair in m_Items)
            {
                RemoveLoop(itemPair.Key);
            }
            if (isImmediate)
            {
                foreach (var itemKey in m_ItemKeysToRemove)
                {
                    var item = m_Items[itemKey];
                    m_RemoveItemAction(item);
                    m_Items.Remove(itemKey);
                }
                m_ItemKeysToRemove.Clear();
                m_Items.Clear();
            }
        }
    }
}