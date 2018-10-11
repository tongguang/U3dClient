using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace U3dClient.GameTools
{
    public class BuildAssetBundleWin : OdinEditorWindow
    {
        public static void OpenWindow()
        {
            var window = GetWindow<BuildAssetBundleWin>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        protected override void Initialize()
        {
            ProjectRootPath = CommonHelper.NormalPath(System.Environment.CurrentDirectory);
            RawResRootPath = CommonHelper.CombinePath("Assets", "Resource");
            TempAssetBundleDirectory = CommonHelper.CombinePath("TempAssetBundles", FileTool.AssetBundlesName);
#if UNITY_STANDALONE
            ReleaseAssetBundleDirectory = "ReleaseAssetBundles/Win";
#elif UNITY_ANDROID
            ReleaseAssetBundleDirectory = "ReleaseAssetBundles/Android";
#else
            ReleaseAssetBundleDirectory = "ReleaseAssetBundles/Ios";
#endif
        }

        [ReadOnly] public string ProjectRootPath = "";
        [ReadOnly] public string RawResRootPath = "";
        [ReadOnly] public string TempAssetBundleDirectory = "";
        [ReadOnly] public string ReleaseAssetBundleDirectory = "";

        private void GenResABName()
        {
            var filePaths = Directory.GetFiles(RawResRootPath,
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
                var abName = dirName.Replace(RawResRootPath + "/", "").ToLower();
                var asset = AssetImporter.GetAtPath(assetPath);
                if (asset)
                {
                    asset.SetAssetBundleNameAndVariant(abName, GlobalDefine.s_BundleSuffixName);
                }
            }
            AssetDatabase.Refresh();
        }

        private void ClearResABName()
        {
            var filePaths = Directory.GetFiles(RawResRootPath,
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

        private void BuildAssetBundles()
        {
            string assetBundleDirectory = TempAssetBundleDirectory;
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }

        [Button("BuildResAssetBundles")]
        public void BuildResAssetBundles()
        {
            GenResABName();
            BuildAssetBundles();
        }

        [Button("CopyAssetBundlesToReleaseDir")]
        public void CopyAssetBundlesToReleaseDir()
        {
        }
    }
}