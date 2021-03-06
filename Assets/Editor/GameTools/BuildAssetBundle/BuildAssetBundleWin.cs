﻿using System.Collections;
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
            AbsProjectPath = BuildAssetBundleProcess.s_AbsProjectPath;
            RelativeNormalResReleasePath = BuildAssetBundleProcess.s_RelativeNormalResReleasePath;
            RelativeTempAssetBundlesPath = BuildAssetBundleProcess.s_RelativeTempAssetBundlesPath;
            AssetBundlesName = BuildAssetBundleProcess.s_AssetBundlesName;
            VersionFileName = BuildAssetBundleProcess.s_VersionFileName;
        }

        [ReadOnly] public string AbsProjectPath = "";
        [ReadOnly] public string RelativeNormalResReleasePath = "";
        [ReadOnly] public string RelativeTempAssetBundlesPath = "";
        [ReadOnly] public string AssetBundlesName = "";
        [ReadOnly] public string VersionFileName = "";
        

        [Button("GeneratePackDataToTempPath")]
        public void GeneratePackDataToTempPath()
        {
            EditorApplication.delayCall += BuildAssetBundleProcess.GeneratePackDataToTempPath;
        }

        [Button("CopyPackDataFromTempToReleasePath")]
        public void CopyPackDataFromTempToReleasePath()
        {
            EditorApplication.delayCall += BuildAssetBundleProcess.CopyPackDataFromTempToReleasePath;
        }

        [Button("CopyPackDataFromReleaseToStreamAssetPath")]
        public void CopyPackDataFromReleaseToStreamAssetPath()
        {
            EditorApplication.delayCall += BuildAssetBundleProcess.CopyPackDataFromReleaseToStreamAssetPath;
        }

        [Button("Test")]
        public void Test()
        {
            EditorApplication.delayCall += BuildAssetBundleProcess.GenerateScriptResAssetBundleName;
        }
    }
}