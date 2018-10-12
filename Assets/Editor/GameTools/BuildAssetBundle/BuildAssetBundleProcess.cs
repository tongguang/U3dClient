using System;
using System.IO;
using System.Security.Cryptography;
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

        public static void GeneratePackDataToTempPath()
        {
            GenerateNormalResAssetBundleName();
            BuildAssetBundlesToTempPath();
            GenerateVersionFile();
        }
    }
}