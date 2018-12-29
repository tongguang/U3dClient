using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

namespace U3dClient
{
    public class UpgradeManager : IGameManager
    {
        public struct FileData
        {
            public string filePath;
            public int fileSize;
            public string fileMD5;
            public string fileDataStr;
        }

        private string m_BundleDotSuffixName = "." + CommonDefine.s_BundleSuffixName;
        private string m_ResUrl;

        public void Awake()
        {
        }

        public void Start()
        {
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void OnApplicationFocus(bool hasFocus)
        {
        }

        public void OnApplicationPause(bool pauseStatus)
        {
        }

        public void OnDestroy()
        {
        }

        public void OnApplicationQuit()
        {
        }


        public void SetResUrl(string resUrl)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            m_ResUrl = resUrl + "/Win";
#elif UNITY_ANDROID
            m_ResUrl = resUrl + "/Android";
#else
            m_ResUrl = resUrl + "/Ios";   
#endif
        }

        public void StartUpdate(Action endAction, Action<long, long> progressAction)
        {
            MainThreadDispatcher.StartCoroutine(StartUpdateEnumerator(endAction, progressAction));
        }

        private IEnumerator StartUpdateEnumerator(Action endAction, Action<long, long> progressAction)
        {
            var resInfoFileExten = CommonDefine.s_ResInfoFileExtension;
            var versionFileName = CommonDefine.s_VersionFileName;

            var baseVersionDatas = new Dictionary<string, FileData>();
            using (UnityWebRequest www =
                UnityWebRequest.Get(Path.Combine(FileTool.s_WWWStreamingAssetsPath, versionFileName)))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(string.Format("读取原始版本文件错误"));
                }
                else
                {
                    var datas = www.downloadHandler.text.Split('\n');
                    foreach (var data in datas)
                    {
                        FileData fileData = new FileData();
                        GetFileData(data, ref fileData);
                        if (fileData.filePath != null)
                        {
                            baseVersionDatas.Add(fileData.filePath, fileData);
                        }
                    }
                }
            }

            var newVersionData = new Dictionary<string, FileData>();
            using (UnityWebRequest www =
                UnityWebRequest.Get(Path.Combine(m_ResUrl, versionFileName)))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(string.Format("读取新版本文件错误"));
                }
                else
                {
                    var datas = www.downloadHandler.text.Split('\n');
                    foreach (var data in datas)
                    {
                        FileData fileData = new FileData();
                        GetFileData(data, ref fileData);
                        if (fileData.filePath != null)
                        {
                            newVersionData.Add(fileData.filePath, fileData);
                        }
                    }
                }
            }

            long totalUpdateSize = 0;
            var addFileDatas = new List<FileData>();
            foreach (var data in newVersionData)
            {
                var newFileData = data.Value;

                var oldFileInfoPath = Path.Combine(FileTool.s_PersistentDataPath,
                    newFileData.filePath.Replace(m_BundleDotSuffixName, "") + resInfoFileExten);
                FileData oldFileData = new FileData {filePath = null};
                if (File.Exists(oldFileInfoPath))
                {
                    GetFileData(File.ReadAllText(oldFileInfoPath), ref oldFileData);
                }
                else if (baseVersionDatas.ContainsKey(newFileData.filePath))
                {
                    oldFileData = baseVersionDatas[newFileData.filePath];
                }

                if (oldFileData.filePath != null)
                {
                    if (newFileData.fileMD5 != oldFileData.fileMD5)
                    {
                        addFileDatas.Add(newFileData);
                        totalUpdateSize += newFileData.fileSize;
                    }
                }
                else
                {
                    addFileDatas.Add(newFileData);
                    totalUpdateSize += newFileData.fileSize;
                }
            }

            long updatedSize = 0;
            progressAction(updatedSize, totalUpdateSize);
            foreach (var addFileData in addFileDatas)
            {
                var filePath = addFileData.filePath;
                var fileDataStr = addFileData.fileDataStr;
                var fileSize = addFileData.fileSize;
                var fileMD5 = addFileData.fileSize;
                var url = string.Format("{0}?{1}{2}", Path.Combine(m_ResUrl, filePath), fileMD5, UnityEngine.Random.Range(1, 10000));
                using (UnityWebRequest www =
                    UnityWebRequest.Get(url))
                {
                    Debug.Log(string.Format("开始下载 {0} 大小:{1}", filePath, fileSize));
                    yield return www.SendWebRequest();
                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log(string.Format("下载错误 {0} 大小:{1}", filePath, fileSize));
                    }
                    else
                    {
                        var fullFilePath = Path.Combine(FileTool.s_PersistentDataPath, filePath);
                        var fullFileInfoPath = fullFilePath.Replace(m_BundleDotSuffixName, "") + resInfoFileExten;
                        var dirName = Path.GetDirectoryName(fullFilePath);
                        if (!Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }

                        File.WriteAllBytes(fullFilePath, www.downloadHandler.data);
                        File.WriteAllText(fullFileInfoPath, fileDataStr);
                        updatedSize += fileSize;
                        progressAction(updatedSize, totalUpdateSize);
                        Debug.Log(string.Format("下载完成 {0} 大小:{1}", filePath, fileSize));
                    }
                }
            }

            if (endAction != null)
            {
                endAction();
            }
        }

        private void GetFileData(string fileDataStr, ref FileData fileData)
        {
            var fileDatas = fileDataStr.Split(' ');
            if (fileDatas.Length >= 3)
            {
                fileData.filePath = fileDatas[0];
                fileData.fileSize = int.Parse(fileDatas[1]);
                fileData.fileMD5 = fileDatas[2];
                fileData.fileDataStr = fileDataStr;
            }
            else
            {
                fileData.filePath = null;
            }
        }
    }
}