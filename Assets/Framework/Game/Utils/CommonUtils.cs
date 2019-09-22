using System.IO;

namespace U3dClient
{
    public class CommonUtils
    {
        public static string NormalPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string CombinePath(string path1, string path2)
        {
            return NormalPath(Path.Combine(path1, path2));
        }
    }
}