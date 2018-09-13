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

        private void ResetData()
        {
            m_BundleName = "";
            m_Bundle = null;
            m_LoadState = LoadState.Init;
            m_LoadedCallbackDict.Clear();
            m_ResouceIndexSet.Clear();
        }

        private void Init(string bundleName)
        {
            m_BundleName = bundleName;
        }

        private int InternalLoadAsync(Action<bool, AssetBundle> loadedAction)
        {
            var index = ResourceManager.GetNewResourceIndex();
            if (loadedAction == null)
            {
                loadedAction = s_DefaultLoadedCallback;
            }

            m_ResouceIndexSet.Add(index);
            if (m_LoadState == LoadState.Init)
            {
                m_LoadedCallbackDict.Add(index, loadedAction);
                m_LoadState = LoadState.WaitLoad;
                MainThreadDispatcher.StartCoroutine(LoadFuncEnumerator());
            }
            else if (m_LoadState == LoadState.WaitLoad || m_LoadState == LoadState.Loading)
            {
                m_LoadedCallbackDict.Add(index, loadedAction);
            }
            else
            {
                loadedAction(m_Bundle != null, m_Bundle);
            }

            return index;
        }

        private int InternalLoadSync(Action<bool, AssetBundle> loadedAction)
        {
            var index = ResourceManager.GetNewResourceIndex();
            if (loadedAction == null)
            {
                loadedAction = s_DefaultLoadedCallback;
            }

            m_ResouceIndexSet.Add(index);
            if (m_LoadState == LoadState.Init || m_LoadState == LoadState.WaitLoad)
            {
                var bundlePath = FileTool.GetBundlePath(m_BundleName);
                m_Bundle = AssetBundle.LoadFromFile(bundlePath);
                m_LoadState = LoadState.Complete;
                loadedAction(m_Bundle != null, m_Bundle);
            }
            else if (m_LoadState == LoadState.Loading)
            {
                Debug.LogWarning("错误加载");
            }
            else
            {
                loadedAction(m_Bundle != null, m_Bundle);
            }

            return index;
        }

        private void InternalUnload(int resourceIndex)
        {
            if (m_LoadedCallbackDict.ContainsKey(resourceIndex))
            {
                m_LoadedCallbackDict.Remove(resourceIndex);
            }

            if (m_ResouceIndexSet.Contains(resourceIndex))
            {
                m_ResouceIndexSet.Remove(resourceIndex);
            }
            STryUnLoadByName(m_BundleName);
        }

        private bool CanRealUnload()
        {
            if (m_LoadState == LoadState.Init || m_LoadState == LoadState.WaitLoad)
            {
                return true;
            }
            else if (m_LoadState == LoadState.Loading)
            {
                return false;
            }
            else
            {
                if (m_ResouceIndexSet.Count > 0)
                {
                    return false;
                }

                return true;
            }
        }

        public IEnumerator LoadFuncEnumerator()
        {
            m_LoadState = LoadState.Loading;
            var bundlePath = FileTool.GetBundlePath(m_BundleName);
            var request = AssetBundle.LoadFromFileAsync(bundlePath);
            if (!request.isDone)
            {
                yield return null;
            }

            m_Bundle = request.assetBundle;
            m_LoadState = LoadState.Complete;

            foreach (var action in m_LoadedCallbackDict)
            {
                var callback = action.Value;
                callback(m_Bundle != null, m_Bundle);
            }

            m_LoadedCallbackDict.Clear();
            STryUnLoadByName(m_BundleName);
        }

        private static readonly ObjectPool<SingleBundleLoader> s_LoaderPool =
            new ObjectPool<SingleBundleLoader>(
                (loader) => { loader.OnReuse(); },
                (loader) => { loader.OnRecycle(); });

        private static readonly Action<bool, AssetBundle> s_DefaultLoadedCallback = (isOk, bundle) => { };
        private static readonly Dictionary<string, SingleBundleLoader> s_NameToLoader = new Dictionary<string, SingleBundleLoader>();
        private static readonly Dictionary<int, SingleBundleLoader> s_ResIndexToLoader = new Dictionary<int, SingleBundleLoader>();

        public static int SLoadAsync(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            SingleBundleLoader loader;
            s_NameToLoader.TryGetValue(bundleName, out loader);
            if (loader == null)
            {
                loader = s_LoaderPool.Get();
                loader.Init(bundleName);
                s_NameToLoader.Add(bundleName, loader);
            }

            var resIndex = loader.InternalLoadAsync(loadedAction);
            s_ResIndexToLoader.Add(resIndex, loader);

            return resIndex;
        }

        public static int SLoadSync(string bundleName, Action<bool, AssetBundle> loadedAction)
        {
            SingleBundleLoader loader;
            s_NameToLoader.TryGetValue(bundleName, out loader);
            if (loader == null)
            {
                loader = s_LoaderPool.Get();
                loader.Init(bundleName);
                s_NameToLoader.Add(bundleName, loader);
            }

            var resIndex = loader.InternalLoadSync(loadedAction);
            s_ResIndexToLoader.Add(resIndex, loader);

            return resIndex;
        }

        public static void SUnLoad(int resouceIndex)
        {
            SingleBundleLoader loader;
            s_ResIndexToLoader.TryGetValue(resouceIndex, out loader);
            if (loader != null)
            {
                loader.InternalUnload(resouceIndex);
            }
        }

        private static void STryUnLoadByName(string bundleName)
        {
            SingleBundleLoader loader;
            s_NameToLoader.TryGetValue(bundleName, out loader);
            if (loader != null && loader.CanRealUnload())
            {
                s_NameToLoader.Remove(bundleName);
                s_LoaderPool.Release(loader);
            }
        }
    }
}