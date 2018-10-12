using System;
using System.IO;
using System.Security.Cryptography;

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
    }
}