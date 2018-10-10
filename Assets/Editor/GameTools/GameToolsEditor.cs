using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace U3dClient.GameTools
{
    public static class GameToolsEditor
    {
        [MenuItem("GameTools/OpenBuildAssetBundleWin")]
        public static void OpenBuildAssetBundleWin()
        {
            BuildAssetBundleWin.OpenWindow();
        }

        [MenuItem("GameTools/Test")]
        public static void Test()
        {
            Debug.Log(Path.GetPathRoot("test"));
        }
    }
}