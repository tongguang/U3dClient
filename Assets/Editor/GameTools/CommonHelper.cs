using System;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace U3dClient.GameTools
{
    public static class CommonHelper
    {
        public static string NormalPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string CombinePath(string path1, string path2)
        {
            return NormalPath(Path.Combine(path1, path2));
        }

        public static string GetFileMD5(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException(String.Format("<{0}>, 不存在", path));
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] buffer = md5Provider.ComputeHash(fs);
            string resule = BitConverter.ToString(buffer);
            resule = resule.Replace("-", "");
            md5Provider.Clear();
            fs.Close();
            return resule;
        }

        public static void CopyFile(string sourcePath, string destPath)
        {
            var dirName = Path.GetDirectoryName(destPath);
            if (dirName == null)
            {
                Debug.LogWarning(string.Format("复制{0}到{1}出错", sourcePath, destPath));
                return;
            }
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            File.Copy(sourcePath, destPath, true);
        }
    }
}