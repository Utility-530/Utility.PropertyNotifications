using System;
using System.ComponentModel;
using System.Net;
using System.Reactive.Linq;


namespace Utility.Tasks
{
    public static class WebClientHelper
    {
        public static IObservable<DownloadProgressChangedEventArgs> GetProgress(this WebClient client) =>
            Observable.FromEventPattern<DownloadProgressChangedEventHandler, DownloadProgressChangedEventArgs>(h => client.DownloadProgressChanged += h, h => client.DownloadProgressChanged -= h)
          .Select(e => e.EventArgs);

        public static IObservable<AsyncCompletedEventArgs> GetCompletion(this WebClient client) =>
             Observable.FromEventPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(h => client.DownloadFileCompleted += h, h => client.DownloadFileCompleted -= h)
                .Select(_ =>
                _.EventArgs);
    }
}

