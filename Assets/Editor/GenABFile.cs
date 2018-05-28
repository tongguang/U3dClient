using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class GenABFile
{
    private static string _ResPath = "Res";
    private static string _ScriptPath = "Script/Lua";
    private static string _ResouceRootPath = Application.dataPath + "/" + "Resource";
    private static string _AssetBundleDirectory = "Assets/AssetBundles";
    private static string _AssetBundleTempDirectory = "Assets/TempAssetBundles";

    [MenuItem("AB/Test")]
    public static void Test()
    {
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.streamingAssetsPath);
    }

    [MenuItem("AB/GenAllAB")]
    public static void GenAllABFiles()
    {
        GenResABName();
        BuildAllAssetBundles();
        CopyBundleToTempDirectory();
    }

    [MenuItem("AB/GenResAB")]
    public static void GenResABFiles()
    {
        GenResABName();
        BuildAllAssetBundles();
        CopyBundleToTempDirectory();
    }

    public static void GenScriptABFiles()
    {
        GenScriptABName();
        BuildAllAssetBundles();
        CopyBundleToTempDirectory();
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

    [MenuItem("AB/CopyBundleToTempDir")]
    public static void CopyBundleToTempDirectory()
    {
        if (Directory.Exists(_AssetBundleTempDirectory))
        {
            Directory.Delete(_AssetBundleTempDirectory, true);
        }
        if (!Directory.Exists(_AssetBundleTempDirectory))
        {
            Directory.CreateDirectory(_AssetBundleTempDirectory);
        }
        CopyFolder(_AssetBundleDirectory, _AssetBundleTempDirectory, "*.ab");
        File.Copy(Path.Combine(_AssetBundleDirectory, "AssetBundles"), Path.Combine(_AssetBundleTempDirectory, "AssetBundles"), true);//覆盖模式
        AssetDatabase.Refresh();
        Debug.Log("复制结束。。");
    }

    [MenuItem("AB/CopyBundleToStreamingAssetsPath")]
    public static void CopyBundleToStreamingAssetsPath()
    {
        if (Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.Delete(Application.streamingAssetsPath, true);
        }
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        CopyFolder(_AssetBundleTempDirectory, Application.streamingAssetsPath, "*.*");
        AssetDatabase.Refresh();
        Debug.Log("复制结束。。");
    }

    public static void CopyFolder(string sourcePath, string destPath, string searchPattern)
    {
        if (Directory.Exists(sourcePath))
        {
            if (!Directory.Exists(destPath))
            {
                //目标目录不存在则创建
                try
                {
                    Directory.CreateDirectory(destPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("创建目标目录失败：" + ex.Message);
                }
            }
            //获得源文件下所有文件
            List<string> files = new List<string>(Directory.GetFiles(sourcePath, searchPattern));
            files.ForEach(c =>
            {
                string destFile = Path.Combine(destPath, Path.GetFileName(c));
                File.Copy(c, destFile, true);//覆盖模式
            });
            //获得源文件下所有目录文件
            List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
            folders.ForEach(c =>
            {
                string destDir = Path.Combine(destPath, Path.GetFileName(c));
                //采用递归的方法实现
                CopyFolder(c, destDir, searchPattern);
            });
        }
        else
        {
            throw new DirectoryNotFoundException("源目录不存在！");
        }
    }
}