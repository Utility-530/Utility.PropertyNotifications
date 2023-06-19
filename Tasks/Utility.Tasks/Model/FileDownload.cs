using Utility.Interfaces.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.NonGeneric;

namespace Utility.Tasks.Model
{
    public struct FileDownload :  IKey<string>
    {
        public FileDownload(Uri uri, string path, string key) : this()
        {
            Uri = uri;
            Path = path;
            Key = key;
        }

        public Uri Uri { get; }

        public string Path { get; }

        public string Key { get; }

        public bool Equals(IKey<string> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable other)
        {
            throw new NotImplementedException();
        }
    }
}
