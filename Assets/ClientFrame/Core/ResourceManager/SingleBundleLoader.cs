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
        private bool m_IsComplete;
        private AssetBundle m_Bundle;

        private readonly Dictionary<int, Action<bool, AssetBundle>> m_LoadedCallbackDict =
            new Dictionary<int, Action<bool, AssetBundle>>();

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

            if (m_LoadedCallbackDict.Count > 0)
            {
                Debug.LogError(string.Format("SingleBundleLoader回收错误 {0}", m_BundleName));
            }
            ResetData();
        }

        public void ResetData()
        {
            m_BundleName = "";
            m_Bundle = null;
            m_IsComplete = false;
            m_LoadedCallbackDict.Clear();
        }

        public int Load(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            var index = ResourceManager.GetNewResourceIndex();
            if (loadedAction == null)
            {
                loadedAction = s_DefaultLoadedCallback;
            }
            m_LoadedCallbackDict.Add(index, loadedAction);
            if (m_IsComplete)
            {
                loadedAction(m_Bundle != null, m_Bundle);
            }
            else
            {
                MainThreadDispatcher.StartCoroutine(LoadFunc());
            }

            return index;
        }

        public void UnLoad(int resourceIndex)
        {
            if (m_LoadedCallbackDict.ContainsKey(resourceIndex))
            {
                m_LoadedCallbackDict.Remove(resourceIndex);
            }
        }

        public IEnumerator LoadFunc()
        {
            var bundlePath = FileTool.GetBundlePath(m_BundleName);
            var request = AssetBundle.LoadFromFileAsync(bundlePath);
            if (!request.isDone)
            {
                yield return null;
            }

            m_Bundle = request.assetBundle;
            m_IsComplete = true;
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

//        static int Load()
    }
}