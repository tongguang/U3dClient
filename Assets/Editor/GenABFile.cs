using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class GenABFile
{
    private static string _ResPath = "Res";
    private static string _ScriptPath = "Script/Lua";
    private static string _ResouceRootPath = Application.dataPath + "/" + "Resource";
    private static string _AssetBundleDirectory = "Assets/AssetBundles";
#if UNITY_EDITOR || UNITY_STANDALONE
    private static string _AssetBundleTempDirectory = "TempAssetBundles/Win";
#elif UNITY_ANDROID
    private static string _AssetBundleTempDirectory = "TempAssetBundles/Android";
#else
    private static string _AssetBundleTempDirectory = "TempAssetBundles/Ios";
#endif
    private static string _AssetBundlesName = "AssetBundles";
    private static string _VersionName = "Version.txt";

    [MenuItem("AB/Test")]
    public static void Test()
    {
        var lines = File.ReadAllLines(Path.Combine(_AssetBundleDirectory, _VersionName));
    }

    [MenuItem("AB/GenAllPackDataToTemp")]
    public static void GenAllPackDataToTempDirectory()
    {
        GenResABName();
        BuildAllAssetBundles();
        GenVersionFile();
        CopyPackDataToTempDirectory();
        AssetDatabase.Refresh();
    }

    public static void GenResABFiles()
    {
        GenResABName();
        BuildAllAssetBundles();
    }

    public static void GenScriptABFiles()
    {
        GenScriptABName();
        BuildAllAssetBundles();
        CopyPackDataToTempDirectory();
    }

    public static void GenResABName()
    {
        var dataPath = Application.dataPath + "/";
        var projectPath = dataPath.Replace("Assets/", "");
        var realPath = _ResouceRootPath + "/" + _ResPath;
        var filePaths = Directory.GetFiles(realPath,
            "*.*", SearchOption.AllDirectories);
        foreach (var filePath in filePaths)
        {
            if (filePath.EndsWith(".meta"))
            {
                continue;
            }

            var filePath2 = filePath.Replace("\\", "/");
            var assetPath = filePath2.Replace(projectPath, "");
            var dirName = Path.GetDirectoryName(filePath2);
            var abName = dirName.Replace(_ResouceRootPath + "/", "").ToLower();
            var asset = AssetImporter.GetAtPath(assetPath);
            asset.assetBundleName = abName;
            asset.assetBundleVariant = "ab";
            asset.SaveAndReimport();
            Debug.Log(abName);
        }
        AssetDatabase.Refresh();
    }

    public static void GenScriptABName()
    {
    }

    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = _AssetBundleDirectory;
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    public static void GenVersionFile()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(GetVersionInfoStr(Path.Combine(_AssetBundleDirectory, _AssetBundlesName)));
        var filePaths = Directory.GetFiles(_AssetBundleDirectory,
            "*.ab", SearchOption.AllDirectories);
        foreach (var filePath in filePaths)
        {
            stringBuilder.Append(GetVersionInfoStr(filePath));
        }
        File.WriteAllText(Path.Combine(_AssetBundleDirectory, _VersionName),stringBuilder.ToString());
    }

    public static string GetVersionInfoStr(string path)
    {
        var newfilePath = path.Replace("\\", "/");
        var fileInfo = new FileInfo(newfilePath);
        var fileSize = fileInfo.Length;
        var md5 = GetFileMD5(newfilePath);
        return string.Format("{0} {1} {2}\n", newfilePath.Replace(_AssetBundleDirectory + "/", ""), fileSize, md5);
    }

    public static string GetFileMD5(string path)
    {
        if (!File.Exists(path))
            throw new ArgumentException(string.Format("<{0}>, 不存在", path));
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
        byte[] buffer = md5Provider.ComputeHash(fs);
        string resule = BitConverter.ToString(buffer);
        resule = resule.Replace("-", "");
        md5Provider.Clear();
        fs.Close();
        return resule;
    }

    [MenuItem("AB/CopyPackDataToTemp")]
    public static void CopyPackDataToTempDirectory()
    {
        DiffCopyPackDatas(_AssetBundleDirectory, _AssetBundleTempDirectory);
        AssetDatabase.Refresh();
        Debug.Log("复制结束。。");
    }

    [MenuItem("AB/CopyPackDataToStreamingAssets")]
    public static void CopyPackDataToStreamingAssetsPath()
    {
        DiffCopyPackDatas(_AssetBundleTempDirectory, Application.streamingAssetsPath);
        AssetDatabase.Refresh();
    }

    public static void CopyFile(string sourcePath, string destPath)
    {
        var dirName = Path.GetDirectoryName(destPath);
        if (!Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        File.Copy(sourcePath, destPath, true);
    }

    public static void DiffCopyPackDatas(string sourcePath, string destPath)
    {
        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
        }
        if (!File.Exists(Path.Combine(sourcePath, _VersionName)))
        {
            Debug.Log("复制失败");
            return;
        }


        string srcVerPath = Path.Combine(sourcePath, _VersionName);
        var srcFileInfos = File.ReadAllLines(srcVerPath);
        var srcFileMD5s = new Dictionary<string, string>();
        foreach (var destFileInfo in srcFileInfos)
        {
            var info = destFileInfo.Split(' ');
            var fileName = info[0];
            var md5 = info[2];
            srcFileMD5s[fileName] = md5;
        }

        string destVerPath = Path.Combine(destPath, _VersionName);
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
                    CopyFile(Path.Combine(sourcePath, name), Path.Combine(destPath, name));
                }
            }
            else
            {
                CopyFile(Path.Combine(sourcePath, name), Path.Combine(destPath, name));
            }
        }

        CopyFile(Path.Combine(sourcePath, _VersionName), Path.Combine(destPath, _VersionName));//覆盖模式

        AssetDatabase.Refresh();
        Debug.Log("复制结束。。");
    }

}