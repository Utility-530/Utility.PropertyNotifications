using ReactiveAsyncWorker.Model;
using System;
using System.IO;
using System.Net;
using System.Reactive.Linq;

namespace ReactiveAsyncWorker.ViewModel
{
    public class FileDownloadWorkerItem : ProgressWorkerItem<object>
    {
        //private Uri _uri;
        private readonly string _path;

        public string Destination => _path;

        public FileDownloadWorkerItem(Uri uri, string path, WebClient client) :
            base(
                progress: client.GetProgress()
                .Select(a => new System.ComponentModel.Custom.Generic.ProgressChangedEventArgs<object>(a.ProgressPercentage, a.UserState)),
                completed: client.GetCompletion(),
                actn: () => client.DownloadFileAsync(uri,
                     Path.Combine(Directory.GetParent(path).CreateDir().FullName, Path.GetFileName(path))),

                key: uri.AbsolutePath + DateTime.Now.ToString())

        {
            _path = path;
        }
    }
}
