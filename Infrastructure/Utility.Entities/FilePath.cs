using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Entities
{
    public readonly record struct FilePath(string Directory, string FileName)
    {
        public string Full => Path.Combine(Directory, FileName);
        public static FilePath FromFilePath(string filePath) => new(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
    }
    public readonly record struct Instance(object Value)
    {
    }

    public readonly record struct RazorEngineOutput(string Output, string Template, object Instance);
}
