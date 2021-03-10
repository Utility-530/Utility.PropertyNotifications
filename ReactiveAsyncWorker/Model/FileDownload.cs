using UtilityInterface.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveAsyncWorker.Model
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
    }
}
