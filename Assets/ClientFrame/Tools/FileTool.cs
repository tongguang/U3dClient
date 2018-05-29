using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace U3dClient
{
    public static class FileTool
    {
        public static string DataPath { private set; get; }
        public static string StreamingAssetsPath { private set; get; }
        public static string PersistentDataPath { private set; get; }

        public static string WWWDataPath { private set; get; }
        public static string WWWStreamingAssetsPath { private set; get; }
        public static string WWWPersistentDataPath { private set; get; }

        static FileTool()
        {
            StreamingAssetsPath = Application.streamingAssetsPath;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            DataPath = Application.dataPath;
            PersistentDataPath = Path.Combine(DataPath, "./ResCache");
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
