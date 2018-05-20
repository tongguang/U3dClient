using System;
using System.Collections.Generic;

namespace U3dClient
{
    public class AssetRefData
    {
        public string BundleName;
        public string AssetName;
        public long RefNums;

        public AssetRefData(string bundleName, string assetName)
        {
            BundleName = bundleName;
            AssetName = assetName;
        }

        public void AddRef()
        {
            RefNums++;
        }

        public void RemoveRef()
        {
            RefNums--;
        }
    }

    public class AssetRefCounter
    {
        private long m_NowIndex; 
        private Dictionary<long, AssetRefData> m_IndexToRefDatas;
        private Dictionary<string, Dictionary<string, AssetRefData>> m_NameToRefDatas;
        public AssetRefCounter()
        {
            m_NowIndex = 0;
            m_IndexToRefDatas = new Dictionary<long, AssetRefData>();
            m_NameToRefDatas = new Dictionary<string, Dictionary<string, AssetRefData>>();
        }

        public long GetNewRefIndex()
        {
            return m_NowIndex++;
        }

        public long AddAssetRef(string bundleName, string assetName)
        {
            var index = GetNewRefIndex();
            if (!m_NameToRefDatas.ContainsKey(bundleName))
            {
                m_NameToRefDatas.Add(bundleName, new Dictionary<string, AssetRefData>());
            }

            if (!m_NameToRefDatas[bundleName].ContainsKey(assetName))
            {
                m_NameToRefDatas[bundleName].Add(assetName, new AssetRefData(bundleName, assetName));
            }
            var assetData = m_NameToRefDatas[bundleName][assetName];
            m_IndexToRefDatas.Add(index, assetData);
            assetData.AddRef();
            return index;
        }

        public bool RemoveAssetRef(long refIndex)
        {
            if (m_IndexToRefDatas.ContainsKey(refIndex))
            {
                var assetData = m_IndexToRefDatas[refIndex];
                assetData.RemoveRef();
                m_IndexToRefDatas.Remove(refIndex);
                if (assetData.RefNums == 0)
                {
                    m_NameToRefDatas[assetData.BundleName].Remove(assetData.AssetName);
                    if (m_NameToRefDatas[assetData.BundleName].Count == 0)
                    {
                        m_NameToRefDatas.Remove(assetData.BundleName);
                    }
                }
                return true;
            }
            return false;
        }

        public AssetRefData GetRefData(long refIndex)
        {
            AssetRefData refData;
            m_IndexToRefDatas.TryGetValue(refIndex, out refData);
            return refData;
        }

        public long GetBundleRefNum(string bundleName)
        {
            if (m_NameToRefDatas.ContainsKey(bundleName))
            {
                return m_NameToRefDatas[bundleName].Count;
            }
            return 0;
        }

        public long GetAssetRefNum(string bundleName, string assetName)
        {
            if (m_NameToRefDatas.ContainsKey(bundleName))
            {
                if (m_NameToRefDatas[bundleName].ContainsKey(assetName))
                {
                    return m_NameToRefDatas[bundleName][assetName].RefNums;
                }
            }
            return 0;
        }
    }
}