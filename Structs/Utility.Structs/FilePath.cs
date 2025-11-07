using System.IO;

namespace Utility.Structs
{
    public readonly record struct FilePath(string Directory, string FileName)
    {
        public string Full => Path.Combine(Directory, FileName);
        public static FilePath FromFilePath(string filePath) => new(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
    }
}