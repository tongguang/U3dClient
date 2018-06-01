﻿using System;
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

        public void StartUpdate(Action endAction)
        {
            MainThreadDispatcher.StartUpdateMicroCoroutine(StartUpdateEnumerator(endAction));
        }

        private IEnumerator StartUpdateEnumerator(Action endAction)
        {
            var resInfoFileExten = FileTool.ResInfoFileExtension;
            var versionFileName = FileTool.VersionFileName;

            var baseVersionDatas = new Dictionary<string, FileData>();
            yield return ObservableWWW.GetWWW(Path.Combine(FileTool.WWWStreamingAssetsPath, versionFileName)).Subscribe(
                x =>
                {
                    var datas = x.text.Split('\n');
                    foreach (var data in datas)
                    {
                        var fileData = GetFileData(data);
                        if (fileData != null)
                        {
                            baseVersionDatas.Add(fileData.filePath, fileData);
                        }
                    }
                });
            var newVersionData = new Dictionary<string, FileData>();
            yield return ObservableWWW.GetWWW(Path.Combine(ResUrl, versionFileName)).Subscribe(
                x =>
                {
                    var datas = x.text.Split('\n');
                    foreach (var data in datas)
                    {
                        var fileData = GetFileData(data);
                        if (fileData != null)
                        {
                            newVersionData.Add(fileData.filePath, fileData);
                        }
                    }
                });

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
                    }
                }
                else
                {
                    addFileDatas.Add(newFileData);
                }
            }

            foreach (var addFileData in addFileDatas)
            {
                var filePath = addFileData.filePath;
                var fileDataStr = addFileData.fileDataStr;
                yield return ObservableWWW.GetAndGetBytes(Path.Combine(ResUrl, filePath)).Subscribe(
                    x =>
                    {
                        var fullFilePath = Path.Combine(FileTool.PersistentDataPath, filePath);
                        var fullFileInfoPath = fullFilePath.Replace(".ab", "") + resInfoFileExten;
                        var dirName = Path.GetDirectoryName(fullFilePath);
                        if (!Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }
                        File.WriteAllBytes(fullFilePath, x);
                        File.WriteAllText(fullFileInfoPath, fileDataStr);
                    });
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