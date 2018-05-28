using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Text;

public static class GenABFile
{
    private static string _ResPath = "Res";
    private static string _ScriptPath = "Script/Lua";
    private static string _ResouceRootPath = Application.dataPath + "/" + "Resource";
    private static string _AssetBundleDirectory = "Assets/AssetBundles";

    [MenuItem("AB/Test")]
    public static void Test()
    {
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.temporaryCachePath);
    }

    [MenuItem("AB/GenAllAB")]
    public static void GenAllABFiles()
    {
        GenResABName();
        BuildAllAssetBundles();
    }

    [MenuItem("AB/GenResAB")]
    public static void GenResABFiles()
    {
        GenResABName();
        BuildAllAssetBundles();
    }

    public static void GenScriptABFiles()
    {
        GenScriptABName();
        BuildAllAssetBundles();
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
}