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

        public bool TryAddLoop(T1 key, T2 value)
        {
            if (m_Items.ContainsKey(key))
            {
                return false;
            }

            if (m_ItemsToAdd.ContainsKey(key))
            {
                return false;
            }

            m_ItemsToAdd.Add(key, value);
            return true;
        }

        public bool TryRemoveLoop(T1 key)
        {
            T2 item;
            m_ItemsToAdd.TryGetValue(key, out item);
            if (item != null)
            {
                m_RemoveItemAction?.Invoke(item);
                m_ItemsToAdd.Remove(key);
                return true;
            }

            if (!m_ItemKeysToRemove.Contains(key) && m_Items.ContainsKey(key))
            {
                m_ItemKeysToRemove.Add(key);
                return true;
            }

            return false;
        }

        public T2 TryGetLoop(T1 key)
        {
            T2 item;
            m_Items.TryGetValue(key, out item);
            if (item != null)
            {
                return item;
            }

            m_ItemsToAdd.TryGetValue(key, out item);
            if (item != null)
            {
                return item;
            }

            return item;
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
                    m_ForeachAction?.Invoke(itemPair.Value);
                }
            }

            foreach (var itemKey in m_ItemKeysToRemove)
            {
                T2 item;
                m_Items.TryGetValue(itemKey, out item);
                if (item != null)
                {
                    m_RemoveItemAction?.Invoke(item);
                    m_Items.Remove(itemKey);
                }
            }

            m_ItemKeysToRemove.Clear();
        }

        public void Clear(bool isImmediate = true)
        {
            foreach (var itemPair in m_ItemsToAdd)
            {
                TryRemoveLoop(itemPair.Key);
            }

            foreach (var itemPair in m_Items)
            {
                TryRemoveLoop(itemPair.Key);
            }

            if (isImmediate)
            {
                foreach (var itemKey in m_ItemKeysToRemove)
                {
                    var item = m_Items[itemKey];
                    m_RemoveItemAction?.Invoke(item);
                    m_Items.Remove(itemKey);
                }

                m_ItemKeysToRemove.Clear();
                m_Items.Clear();
            }
        }
    }
}