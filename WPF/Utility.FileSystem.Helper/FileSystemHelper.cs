using System;
using System.IO;


#nullable enable
namespace Utility.FileSystem.Helper
{
    public static class FileSystemHelper
    {
        public static string CorrectFileDestinationPath(string source, string destination)
        {
            string str = destination;
            bool? nullable = destination.IsPathFile();
            bool flag = true;
            if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
                str = Path.Combine(destination, Path.GetFileName(source));
            return str;
        }

        public static bool? IsPathFile(this string path)
        {
            bool? nullable = new bool?();
            if (Directory.Exists(path) || File.Exists(path))
                nullable = !File.GetAttributes(path).HasFlag((Enum)FileAttributes.Directory) ? new bool?(false) : new bool?(true);
            return nullable;
        }
    }
}