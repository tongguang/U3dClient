using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            string[] destFileInfos = null;
            var destFileMD5s = new Dictionary<string, string>();
            if (File.Exists(destVerPath))
            {
                destFileInfos = File.ReadAllLines(destVerPath);
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
            Debug.Log("复制结束。。");
        }

        public static void GeneratePackDataToTempPath()
        {
            GenerateNormalResAssetBundleName();
            BuildAssetBundlesToTempPath();
            GenerateVersionFile();
        }

        public static void CopyPackDataFromTempToReleasePath()
        {
            DiffCopyPackDatas(s_RelativeTempAssetBundlesPath, s_RelativeReleaseAssetBundlesPath);
        }

        public static void CopyPackDataFromReleaseToStreamAssetPath()
        {
            DiffCopyPackDatas(CommonHelper.CombinePath(s_AbsProjectPath, s_RelativeTempAssetBundlesPath) , Application.streamingAssetsPath);
        }
    }
}