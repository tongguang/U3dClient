using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace U3dClient
{
    public static class FileTool
    {
        public static string s_DataPath;
        public static string s_StreamingAssetsPath;
        public static string s_PersistentDataPath;

        public static string s_WWWDataPath;
        public static string s_WWWStreamingAssetsPath;
        public static string s_WWWPersistentDataPath;

        static FileTool()
        {
            
            s_StreamingAssetsPath = Application.streamingAssetsPath;
#if UNITY_STANDALONE
            s_DataPath = Application.dataPath;
            s_PersistentDataPath = Path.Combine(s_DataPath, "../ResCache");
            s_WWWDataPath = "file://" + s_DataPath;
            s_WWWStreamingAssetsPath = "file://" + s_StreamingAssetsPath;
            s_WWWPersistentDataPath = "file://" + s_PersistentDataPath;
#elif UNITY_ANDROID
            s_DataPath = Application.dataPath;
            s_PersistentDataPath = Application.persistentDataPath;
			s_WWWDataPath = "jar:file://" + s_DataPath;
			s_WWWStreamingAssetsPath = s_StreamingAssetsPath;
			s_WWWPersistentDataPath = "jar:file://" + s_PersistentDataPath;
#else
            s_DataPath = Application.dataPath;
            s_PersistentDataPath = Application.persistentDataPath;
            s_WWWDataPath = "file://" + s_DataPath;
            s_WWWStreamingAssetsPath = "file://" + s_StreamingAssetsPath;
            s_WWWPersistentDataPath = "file://" + s_PersistentDataPath;
#endif
        }

        public static string GetBundlePath(string bundleName)
        {
            if (bundleName == "")
            {
                return "";
            }

            {
                var bundlePath = Path.Combine(s_PersistentDataPath, bundleName);
                if (File.Exists(bundlePath))
                {
                    return bundlePath;
                }
            }

            {
                var bundlePath = Path.Combine(s_StreamingAssetsPath, bundleName);
                return bundlePath;
            }
        }
    }
}
