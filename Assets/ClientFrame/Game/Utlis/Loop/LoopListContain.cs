using System;
using System.Collections.Generic;

namespace U3dClient
{
    public class LoopListContain<T>
    {
        #region PrivateVal

        private readonly List<T> m_Items = new List<T>();
        private readonly List<T> m_ItemsToAdd = new List<T>();
        private readonly HashSet<T> m_ItemsToRemove = new HashSet<T>();
        private Action<T> m_ForeachAction;
        private Action<T> m_RemoveItemAction;

        #endregion

        #region PrivateFunc

        private void ForeachToAdd()
        {
            foreach (var item in m_ItemsToAdd) m_Items.Add(item);
            m_ItemsToAdd.Clear();
        }

        private void ForeachToRemove()
        {
            foreach (var item in m_ItemsToRemove)
            {
                m_Items.Remove(item);
            }
            m_ItemsToRemove.Clear();
        }

        private void ForeachItems()
        {
            foreach (var item in m_Items)
                if (!m_ItemsToRemove.Contains(item))
                    m_ForeachAction?.Invoke(item);
        }

        #endregion

        #region PublicFunc

        public void SetForeachAction(Action<T> action)
        {
            m_ForeachAction = action;
        }

        public void SetRemoveItemAction(Action<T> action)
        {
            m_RemoveItemAction = action;
        }

        public bool TryAddLoop(T value)
        {
            m_ItemsToAdd.Add(value);
            return true;
        }

        public bool TryRemoveLoop(T value)
        {
            m_ItemsToRemove.Add(value);
            return true;
        }

        public void Foreach()
        {
            ForeachToAdd();
            ForeachItems();
            ForeachToRemove();
        }

        public void Clear(bool isImmediate = true)
        {
            foreach (var item in m_ItemsToAdd) TryRemoveLoop(item);

            foreach (var item in m_Items) TryRemoveLoop(item);

            if (isImmediate)
            {
               ForeachToRemove();
            }
        }

        #endregion
    }
}