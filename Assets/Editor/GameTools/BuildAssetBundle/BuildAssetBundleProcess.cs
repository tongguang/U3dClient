using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using U3dClient.ScriptMgr;
using UnityEditor;
using UnityEngine;

namespace U3dClient.GameTools
{
    public static class BuildAssetBundleProcess
    {
        public static string s_AbsProjectPath = CommonHelper.NormalPath(System.Environment.CurrentDirectory);
        public static string s_RelativeNormalResRawPath = CommonHelper.CombinePath("Assets", "Resource");
        public static string s_RelativeTempAssetBundlesPath = CommonHelper.CombinePath("GameTemp", GlobalDefine.s_AssetBundlesName);
        
#if UNITY_STANDALONE
        public static string s_RelativeReleaseAssetBundlesPath = "GameRelease/AssetBundles/Win";
#elif UNITY_ANDROID
        public static string s_RelativeReleaseAssetBundlesPath = "GameRelease/AssetBundles/Android";
#else
        public static string s_RelativeReleaseAssetBundlesPath = "GameRelease/AssetBundles/Ios";
#endif
        public static string s_AssetBundlesName = GlobalDefine.s_AssetBundlesName;
        public static string s_VersionFileName = GlobalDefine.s_VersionFileName;


        public static string s_RelativeScriptResRawPath = CommonHelper.CombinePath("Assets", "Script");
        public static string s_RelativeScriptResTempPackPath = CommonHelper.CombinePath("Assets", "TempPackScript");
        public static string s_ScriptAssetBundleName = GlobalDefine.s_ScriptAssetBundleName;

        private static void GenerateNormalResAssetBundleName()
        {
            var filePaths = Directory.GetFiles(s_RelativeNormalResRawPath,
                "*.*", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                if (filePath.EndsWith(".meta"))
                {
                    continue;
                }
                var normalFilePath = CommonHelper.NormalPath(filePath);
                var assetPath = normalFilePath;
                var dirName = Path.GetDirectoryName(normalFilePath);
                if (dirName != null)
                {
                    var abName = dirName.Replace(s_RelativeNormalResRawPath + "/", "").ToLower();
                    var asset = AssetImporter.GetAtPath(assetPath);
                    if (asset)
                    {
                        asset.SetAssetBundleNameAndVariant(abName, GlobalDefine.s_BundleSuffixName);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        private static void ClearNormalResAssetBundleName()
        {
            var filePaths = Directory.GetFiles(s_RelativeNormalResRawPath,
                "*.*", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                if (filePath.EndsWith(".meta"))
                {
                    continue;
                }
                var normalFilePath = CommonHelper.NormalPath(filePath);
                var assetPath = normalFilePath;
                var asset = AssetImporter.GetAtPath(assetPath);
                if (asset)
                {
                    asset.SetAssetBundleNameAndVariant(null, null);
                }
            }
            AssetDatabase.Refresh();
        }

        private static void GenerateVersionFile()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(GetVersionInfoStr(Path.Combine(s_RelativeTempAssetBundlesPath, s_AssetBundlesName)));
            stringBuilder.Append("\n");
            var filePaths = Directory.GetFiles(s_RelativeTempAssetBundlesPath,
                "*." + GlobalDefine.s_BundleSuffixName, SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                stringBuilder.Append(GetVersionInfoStr(filePath));
                stringBuilder.Append("\n");
            }

            File.WriteAllText(Path.Combine(s_RelativeTempAssetBundlesPath, s_VersionFileName), stringBuilder.ToString());
        }

        private static string GetVersionInfoStr(string path)
        {
            var newfilePath = CommonHelper.NormalPath(path);
            var fileInfo = new FileInfo(newfilePath);
            var fileSize = fileInfo.Length;
            var md5 = CommonHelper.GetFileMD5(newfilePath);
            return string.Format("{0} {1} {2}", newfilePath.Replace(s_RelativeTempAssetBundlesPath + "/", ""), fileSize, md5);
        }

        public static void GenerateScriptResAssetBundleName()
        {
            if (Directory.Exists(s_RelativeScriptResTempPackPath))
            {
                FileUtil.DeleteFileOrDirectory(s_RelativeScriptResTempPackPath);
            }
            var filePaths = Directory.GetFiles(s_RelativeScriptResRawPath,
                "*.lua", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                var normalFilePath = CommonHelper.NormalPath(filePath);
                var luaDataPath = normalFilePath.Replace(s_RelativeScriptResRawPath + "/", "").Replace(".lua", ".bytes").Replace("/", ".");
                luaDataPath = CommonHelper.CombinePath(s_RelativeScriptResTempPackPath, luaDataPath);
                var luaDataDirPath = Path.GetDirectoryName(luaDataPath);
                if (!Directory.Exists(luaDataDirPath))
                {
                    if (luaDataDirPath != null) Directory.CreateDirectory(luaDataDirPath);
                }
                var datas = File.ReadAllBytes(normalFilePath);
                File.WriteAllBytes(luaDataPath, datas);
            }
            AssetDatabase.Refresh();

            var fileDesc = ScriptableObject.CreateInstance<LuaFileRef>();
            Dictionary<string, TextAsset> fileRefDict = new Dictionary<string, TextAsset>();

            var abName = s_ScriptAssetBundleName;
            filePaths = Directory.GetFiles(s_RelativeScriptResTempPackPath,
                "*.bytes", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
               
                var normalFilePath = CommonHelper.NormalPath(filePath);
                var assetPath = normalFilePath;
                var dirName = Path.GetDirectoryName(normalFilePath);
                if (dirName != null)
                {
                    var asset = AssetImporter.GetAtPath(assetPath);
                    if (asset)
                    {
                        asset.SetAssetBundleNameAndVariant(abName, GlobalDefine.s_BundleSuffixName);
                        var fileName = Path.GetFileNameWithoutExtension(assetPath);
                        fileRefDict[fileName] = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                    }
                }
            }

            fileDesc.AssetsRefDict = fileRefDict;
            var fileDescPath = CommonHelper.CombinePath(s_RelativeScriptResTempPackPath, GlobalDefine.s_ScriptFileDescName + ".asset");
            AssetDatabase.CreateAsset(fileDesc, fileDescPath);
            var fileDescAsset = AssetImporter.GetAtPath(fileDescPath);
            fileDescAsset.SetAssetBundleNameAndVariant(abName, GlobalDefine.s_BundleSuffixName);

            AssetDatabase.Refresh();
        }

        private static void BuildAssetBundlesToTempPath()
        {
            string assetBundleDirectory = s_RelativeTempAssetBundlesPath;
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);
        }

        public static void DiffCopyPackDatas(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            if (!File.Exists(CommonHelper.CombinePath(sourcePath, s_VersionFileName)))
            {
                Debug.Log("复制失败");
                return;
            }


            string srcVerPath = CommonHelper.CombinePath(sourcePath, s_VersionFileName);
            var srcFileInfos = File.ReadAllLines(srcVerPath);
            var srcFileMD5s = new Dictionary<string, string>();
            foreach (var destFileInfo in srcFileInfos)
            {
                var info = destFileInfo.Split(' ');
                var fileName = info[0];
                var md5 = info[2];
                srcFileMD5s[fileName] = md5;
            }

            string destVerPath = CommonHelper.CombinePath(destPath, s_VersionFileName);
            var destFileMD5s = new Dictionary<string, string>();
            if (File.Exists(destVerPath))
            {
                var destFileInfos = File.ReadAllLines(destVerPath);
                foreach (var destFileInfo in destFileInfos)
                {
                    var info = destFileInfo.Split(' ');
                    var fileName = info[0];
                    var md5 = info[2];
                    destFileMD5s[fileName] = md5;
                }
            }

            foreach (var srcFileMd5 in srcFileMD5s)
            {
                var name = srcFileMd5.Key;
                var md5 = srcFileMd5.Value;
                if (destFileMD5s.ContainsKey(name))
                {
                    if (destFileMD5s[name] != md5)
                    {
                        CommonHelper.CopyFile(CommonHelper.CombinePath(sourcePath, name), CommonHelper.CombinePath(destPath, name));
                    }
                }
                else
                {
                    CommonHelper.CopyFile(CommonHelper.CombinePath(sourcePath, name), CommonHelper.CombinePath(destPath, name));
                }
            }

            CommonHelper.CopyFile(CommonHelper.CombinePath(sourcePath, s_VersionFileName), CommonHelper.CombinePath(destPath, s_VersionFileName));//覆盖模式

//            foreach (var destFileMd5 in destFileMD5s)
//            {
//                var name = destFileMd5.Key;
//                var md5 = destFileMd5.Value;
//                if (!srcFileMD5s.ContainsKey(name))
//                {
//                    Debug.Log(String.Format("删除{0}", CommonHelper.CombinePath(destPath, name)));
//                    FileUtil.DeleteFileOrDirectory(CommonHelper.CombinePath(destPath, name));
//                }
//            }

            AssetDatabase.Refresh();
            Debug.Log(string.Format("复制{0}到{1}结束。。", sourcePath, destPath));
        }

        public static void GeneratePackDataToTempPath()
        {
            GenerateNormalResAssetBundleName();
            GenerateScriptResAssetBundleName();
            BuildAssetBundlesToTempPath();
            GenerateVersionFile();
        }

        public static void CopyPackDataFromTempToReleasePath()
        {
            DiffCopyPackDatas(s_RelativeTempAssetBundlesPath, s_RelativeReleaseAssetBundlesPath);
        }

        public static void CopyPackDataFromReleaseToStreamAssetPath()
        {
            DiffCopyPackDatas(CommonHelper.CombinePath(s_AbsProjectPath, s_RelativeReleaseAssetBundlesPath) , Application.streamingAssetsPath);
        }
    }
}