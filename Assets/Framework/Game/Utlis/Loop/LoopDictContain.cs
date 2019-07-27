using System;
using System.Collections.Generic;

namespace U3dClient
{
    public class LoopDictContain<T1, T2>
    {
        #region PrivateVal

        private readonly Dictionary<T1, T2> m_Items = new Dictionary<T1, T2>();
        private readonly Dictionary<T1, T2> m_ItemsToAdd = new Dictionary<T1, T2>();
        private readonly HashSet<T1> m_ItemKeysToRemove = new HashSet<T1>();
        private Action<T2> m_ForeachAction;
        private Action<T2> m_RemoveItemAction;

        #endregion

        #region PrivateFunc

        private void ForeachToAdd()
        {
            foreach (var itemPair in m_ItemsToAdd) m_Items.Add(itemPair.Key, itemPair.Value);
            m_ItemsToAdd.Clear();
        }

        private void ForeachToRemove()
        {
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

        private void ForeachItems()
        {
            foreach (var itemPair in m_Items)
                if (!m_ItemKeysToRemove.Contains(itemPair.Key))
                    m_ForeachAction?.Invoke(itemPair.Value);
        }

        #endregion

        #region PublicFunc

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
            if (m_Items.ContainsKey(key)) return false;

            if (m_ItemsToAdd.ContainsKey(key)) return false;

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
            if (item != null) return item;

            m_ItemsToAdd.TryGetValue(key, out item);
            if (item != null) return item;

            return default(T2);
        }

        public void Foreach()
        {
            ForeachToAdd();
            ForeachItems();
            ForeachToRemove();
        }

        public void Clear(bool isImmediate = true)
        {
            foreach (var itemPair in m_ItemsToAdd) TryRemoveLoop(itemPair.Key);

            foreach (var itemPair in m_Items) TryRemoveLoop(itemPair.Key);

            if (isImmediate)
            {
                ForeachToRemove();
            }
        }

        public int GetItemCount()
        {
            return m_ItemsToAdd.Count + m_Items.Count;
        }

        public bool IsContainItem(T1 key)
        {
            if (m_Items.ContainsKey(key))
            {
                return true;
            }
            if (m_ItemsToAdd.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}