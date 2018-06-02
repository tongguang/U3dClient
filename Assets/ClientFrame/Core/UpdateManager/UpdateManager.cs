using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

namespace U3dClient
{
    public class UpdateManager
    {
        public class FileData
        {
            public string filePath;
            public int fileSize;
            public string fileMD5;
            public string fileDataStr;
        }

        public string ResUrl;
        public void Awake()
        {

        }

        public void SetResUrl(string resUrl)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            ResUrl = resUrl + "/Win";
#elif UNITY_ANDROID
            ResUrl = resUrl + "/Android";
#else
            ResUrl = resUrl + "/Ios";   
#endif
        }

        public void StartUpdate(Action endAction, Action<long, long> progressAction)
        {
            MainThreadDispatcher.StartCoroutine(StartUpdateEnumerator(endAction, progressAction));
        }

        private IEnumerator StartUpdateEnumerator(Action endAction, Action<long, long> progressAction)
        {
            var resInfoFileExten = FileTool.ResInfoFileExtension;
            var versionFileName = FileTool.VersionFileName;

            var baseVersionDatas = new Dictionary<string, FileData>();
            var obWWWText = ObservableWWW.Get(Path.Combine(FileTool.WWWStreamingAssetsPath, versionFileName)).ToYieldInstruction();
            yield return obWWWText;
            if (obWWWText.HasResult)
            {
                var datas = obWWWText.Result.Split('\n');
                foreach (var data in datas)
                {
                    var fileData = GetFileData(data);
                    if (fileData != null)
                    {
                        baseVersionDatas.Add(fileData.filePath, fileData);
                    }
                }
            }
            if (obWWWText.HasError)
            {
                Debug.Log(string.Format("读取原始版本文件错误"));
            }

            var newVersionData = new Dictionary<string, FileData>();
            obWWWText = ObservableWWW.Get(Path.Combine(ResUrl, versionFileName)).ToYieldInstruction();
            yield return obWWWText;
            if (obWWWText.HasResult)
            {
                var datas = obWWWText.Result.Split('\n');
                foreach (var data in datas)
                {
                    var fileData = GetFileData(data);
                    if (fileData != null)
                    {
                        newVersionData.Add(fileData.filePath, fileData);
                    }
                }
            }
            if (obWWWText.HasError)
            {
                Debug.Log(string.Format("读取新版本文件错误"));
            }

            long totalUpdateSize = 0;
            var addFileDatas = new List<FileData>();
            foreach (var data in newVersionData)
            {
                var newFileData = data.Value;

                var oldFileInfoPath = Path.Combine(FileTool.PersistentDataPath, newFileData.filePath.Replace(".ab", "") + resInfoFileExten);
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
                var obWWWByte = ObservableWWW.GetAndGetBytes(Path.Combine(ResUrl, filePath)).ToYieldInstruction();
                Debug.Log(string.Format("开始下载 {0} 大小:{1}", filePath, fileSize));
                yield return obWWWByte;
                if (obWWWByte.HasResult)
                {
                    var fullFilePath = Path.Combine(FileTool.PersistentDataPath, filePath);
                    var fullFileInfoPath = fullFilePath.Replace(".ab", "") + resInfoFileExten;
                    var dirName = Path.GetDirectoryName(fullFilePath);
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                    File.WriteAllBytes(fullFilePath, obWWWByte.Result);
                    File.WriteAllText(fullFileInfoPath, fileDataStr);
                    updatedSize += fileSize;
                    progressAction(updatedSize, totalUpdateSize);
                    Debug.Log(string.Format("下载完成 {0} 大小:{1}", filePath, fileSize));
                }
                if (obWWWByte.HasError)
                {
                    Debug.Log(string.Format("下载错误 {0} 大小:{1}", filePath, fileSize));
                }
            }
            if (endAction != null)
            {
                endAction();
            }
        }

        private FileData GetFileData(string fileDataStr)
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
