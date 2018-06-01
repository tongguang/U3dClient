using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace U3dClient
{
    public static class FileTool
    {
        public static string DataPath;
        public static string StreamingAssetsPath;
        public static string PersistentDataPath;

        public static string WWWDataPath;
        public static string WWWStreamingAssetsPath;
        public static string WWWPersistentDataPath;

        public static string AssetBundlesName;
        public static string VersionFileName;
        public static string ResInfoFileExtension;

        static FileTool()
        {
            AssetBundlesName = "AssetBundles";
            VersionFileName = "Version.txt";
            ResInfoFileExtension = ".ex";

            StreamingAssetsPath = Application.streamingAssetsPath;
#if UNITY_STANDALONE
            DataPath = Application.dataPath;
            PersistentDataPath = Path.Combine(DataPath, "../ResCache");
            WWWDataPath = "file://" + DataPath;
            WWWStreamingAssetsPath = "file://" + StreamingAssetsPath;
            WWWPersistentDataPath = "file://" + PersistentDataPath;
#elif UNITY_ANDROID
            DataPath = "jar:file://" + Application.dataPath;
            PersistentDataPath = "jar:file://" + Application.persistentDataPath;
            WWWDataPath = DataPath;
            WWWStreamingAssetsPath = StreamingAssetsPath;
            WWWPersistentDataPath = PersistentDataPath;
#else
            DataPath = Application.dataPath;
            PersistentDataPath = Application.persistentDataPath;
            WWWDataPath = "file://" + DataPath;
            WWWStreamingAssetsPath = "file://" + StreamingAssetsPath;
            WWWPersistentDataPath = "file://" + PersistentDataPath;
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
                return bundlePath;
            }
        }
    }
}
