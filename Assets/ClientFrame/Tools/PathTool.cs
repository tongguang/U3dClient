using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace U3dClient
{
    public static class PathTool
    {
        public static string DataPath { private set; get; }
        public static string StreamingAssetsPath { private set; get; }
        public static string PersistentDataPath { private set; get; }

        static PathTool()
        {
            DataPath = Application.dataPath;
            StreamingAssetsPath = Application.streamingAssetsPath;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            PersistentDataPath = Path.Combine(DataPath, "./ResCache");
#else
            PersistentDataPath = Application.persistentDataPath;
#endif
        }

        public static string GetBundlePath(string bundleName)
        {
            if (bundleName == "")
            {
                return "";
            }

            {
                var bundlePath = Path.Combine(PersistentDataPath, bundleName);
                if (File.Exists(bundlePath))
                {
                    return bundlePath;
                }
            }

            {
                var bundlePath = Path.Combine(StreamingAssetsPath, bundleName);
                if (File.Exists(bundlePath))
                {
                    return bundlePath;
                }
            }

            return "";
        }
    }
}
