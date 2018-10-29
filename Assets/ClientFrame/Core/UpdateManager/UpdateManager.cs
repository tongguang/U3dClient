using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace U3dClient.UpdateMgr
{
    public static class UpdateManager
    {
        public class FileData
        {
            public string filePath;
            public int fileSize;
            public string fileMD5;
            public string fileDataStr;
        }

        private static string s_BundleDotSuffixName = "." + GlobalDefine.s_BundleSuffixName;
        public static string ResUrl;

        public static void Awake()
        {
        }

        public static void Start()
        {

        }

        public static void Update()
        {
        }

        public static void OnApplicationFocus(bool hasFocus)
        {

        }

        public static void OnApplicationPause(bool pauseStatus)
        {

        }

        public static void OnDestroy()
        {

        }

        public static void OnApplicationQuit()
        {

        }

        public static void SetResUrl(string resUrl)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            ResUrl = resUrl + "/Win";
#elif UNITY_ANDROID
            ResUrl = resUrl + "/Android";
#else
            ResUrl = resUrl + "/Ios";   
#endif
        }

        public static void StartUpdate(Action endAction, Action<long, long> progressAction)
        {
            MainThreadDispatcher.StartCoroutine(StartUpdateEnumerator(endAction, progressAction));
        }

        private static IEnumerator StartUpdateEnumerator(Action endAction, Action<long, long> progressAction)
        {
            var resInfoFileExten = GlobalDefine.s_ResInfoFileExtension;
            var versionFileName = GlobalDefine.s_VersionFileName;

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
                        var fileData = GetFileData(data);
                        if (fileData != null)
                        {
                            baseVersionDatas.Add(fileData.filePath, fileData);
                        }
                    }
                }
            }

            var newVersionData = new Dictionary<string, FileData>();
            using (UnityWebRequest www =
                UnityWebRequest.Get(Path.Combine(ResUrl, versionFileName)))
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
                        var fileData = GetFileData(data);
                        if (fileData != null)
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
                    newFileData.filePath.Replace(s_BundleDotSuffixName, "") + resInfoFileExten);
                FileData oldFileData = null;
                if (File.Exists(oldFileInfoPath))
                {
                    oldFileData = GetFileData(File.ReadAllText(oldFileInfoPath));
                }
                else if (baseVersionDatas.ContainsKey(newFileData.filePath))
                {
                    oldFileData = baseVersionDatas[newFileData.filePath];
                }

                if (oldFileData != null)
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
                using (UnityWebRequest www =
                    UnityWebRequest.Get(Path.Combine(ResUrl, filePath)))
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
                        var fullFileInfoPath = fullFilePath.Replace(s_BundleDotSuffixName, "") + resInfoFileExten;
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

        private static FileData GetFileData(string fileDataStr)
        {
            var fileDatas = fileDataStr.Split(' ');
            if (fileDatas.Length >= 3)
            {
                var fileData = new FileData
                {
                    filePath = fileDatas[0],
                    fileSize = int.Parse(fileDatas[1]),
                    fileMD5 = fileDatas[2],
                    fileDataStr = fileDataStr
                };
                return fileData;
            }

            return null;
        }
    }
}