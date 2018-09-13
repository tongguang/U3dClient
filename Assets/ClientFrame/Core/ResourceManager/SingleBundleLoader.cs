using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U3dClient.GamePool;
using UniRx;

namespace U3dClient.ResourceMgr
{
    public class SingleBundleLoader : ILoaderBase
    {
        private string m_BundleName;
        private LoadState m_LoadState;
        private AssetBundle m_Bundle;

        private readonly Dictionary<int, Action<bool, AssetBundle>> m_LoadedCallbackDict =
            new Dictionary<int, Action<bool, AssetBundle>>();
        private readonly HashSet<int> m_ResouceIndexSet = new HashSet<int>();

        public SingleBundleLoader()
        {
            ResetData();
        }

        public void OnReuse()
        {
        }

        public void OnRecycle()
        {
            if (m_Bundle)
            {
                m_Bundle.Unload(true);
            }

            if (m_ResouceIndexSet.Count > 0)
            {
                Debug.LogError(string.Format("SingleBundleLoader回收错误 {0}", m_BundleName));
            }
            ResetData();
        }

        public void ResetData()
        {
            m_BundleName = "";
            m_Bundle = null;
            m_LoadState = LoadState.Init;
            m_LoadedCallbackDict.Clear();
        }

        private int InternalLoad(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            m_BundleName = bundleName;
            var index = ResourceManager.GetNewResourceIndex();
            if (loadedAction == null)
            {
                loadedAction = s_DefaultLoadedCallback;
            }

            m_ResouceIndexSet.Add(index);
            if (m_LoadState == LoadState.Init)
            {
                m_LoadedCallbackDict.Add(index, loadedAction);
                m_LoadState = LoadState.Loading;
                MainThreadDispatcher.StartCoroutine(LoadFuncEnumerator());
            }
            else if (m_LoadState == LoadState.Loading)
            {
                m_LoadedCallbackDict.Add(index, loadedAction);
            }
            else
            {
                loadedAction(m_Bundle != null, m_Bundle);
            }
            return index;
        }

        public void InternalUnload(int resourceIndex)
        {
            if (m_LoadedCallbackDict.ContainsKey(resourceIndex))
            {
                m_LoadedCallbackDict.Remove(resourceIndex);
            }

            if (m_ResouceIndexSet.Contains(resourceIndex))
            {
                m_ResouceIndexSet.Remove(resourceIndex);
            }
        }

        public IEnumerator LoadFuncEnumerator()
        {
            var bundlePath = FileTool.GetBundlePath(m_BundleName);
            var request = AssetBundle.LoadFromFileAsync(bundlePath);
            if (!request.isDone)
            {
                yield return null;
            }

            m_Bundle = request.assetBundle;
            m_LoadState = LoadState.Complete;
            if (m_LoadedCallbackDict.Count > 0)
            {
                foreach (var action in m_LoadedCallbackDict)
                {
                    var callback = action.Value;
                    callback(m_Bundle != null, m_Bundle);
                }
            }
            else
            {
            }
        }

        static ObjectPool<SingleBundleLoader> s_LoaderPool =
            new ObjectPool<SingleBundleLoader>(
                (loader) => { loader.OnReuse(); },
                (loader) => { loader.OnRecycle(); });

        static Action<bool, AssetBundle> s_DefaultLoadedCallback = (isOk, bundle) => { };

        static public int Load(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            return 0;
        }

        static public void UnLoad(int resouceIndex)
        {

        }
    }
}