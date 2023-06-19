using System;
using System.Reactive.Linq;
using Utility.Enums;
using System.Net;
using Utility.Tasks.Model;

namespace Utility.Tasks
{

    public record FileDownloadTaskOutput(string Key, object Value) : TaskOutput(Key, Value);
    public class FactoryFileDownloadInput
    {
        public FactoryFileDownloadInput(WebClient client, FileDownload fileDownload)
        {
            Client = client;
            FileDownload = fileDownload;
        }

        public FileDownload FileDownload { get; }

        public WebClient Client { get; }
    }




    public class FileDownloadQueue : AsyncWorkerQueue /*: INotifyPropertyChanged*/ //where T : new()
    {
        static readonly WebClient client = new();

        public FileDownloadQueue(string key, IObservable<FileDownload> mainMethod, IFactory<IWorkerItem, FactoryFileDownloadInput> factory) : base(client.GetCompletion().Where(a => a.Error == null).Select(arg => new FileDownloadTaskOutput(key, arg)))
        {
            client
                .GetCompletion()
                .Where(a => a.Error != null)
                .Subscribe(a =>
                {

                });


            mainMethod
                .Subscribe(f =>
                {
                    // var item = new FileDownloadWorkerItem(f.Uri, f.Path, client);
                    Enqueue(factory.Create(new FactoryFileDownloadInput(client, f)));
                }, e =>
                 Console.WriteLine(e.Message), () => { });

            Init(commands);

            void Init(IObservable<ProcessState> commands)
            {
                commands
                .Subscribe(command =>
                {
                    //if (command == Utility.Enums.ProcessState.Blocked)

                    //else if (command == Utility.Enums.ProcessState.Ready)
                    //{
                    //    _backgroundWorker.RunWorkerAsync(wa);
                    //}
                    //else if (command == Utility.Enums.ProcessState.Running)

                    if (command == ProcessState.Terminated)
                        Cancel();
                    else
                        throw new ArgumentOutOfRangeException("argument should be nullable bool");
                });
            }
        }





        public void Cancel()
        {
            if (client.IsBusy)
                client.CancelAsync();
        }


    }

    //public class FileDownloaderCommandQueue : AsyncWorkerQueue<CompletedState>,IPlayer /*: INotifyPropertyChanged*/ //where T : new()
    //{

    //    public ISubject<Utility.Enums.ProcessState> commands { get; } = new Subject<Utility.Enums.ProcessState>();


    //    static IFileDownloader fileDownloader = new FileDownloader.FileDownloader(new FileDownloader.DownloadCacheImplementation());

    //    Tuple<Uri, string> tus = default(Tuple<Uri, string>);

    //    public FileDownloaderCommandQueue(IObservable<Tuple<Uri, string>> mainMethod) : base(fileDownloader.GetCompletion().Select(_=>_.State))
    //    {

    //        int i = 0;

    //        mainMethod.Subscribe(_ =>
    //        {
    //            tus = _;
    //            var x = new FileDownloadWorkerItem<CompletedState>(_.Item1, _.Item2, fileDownloader);
    //            Run(x);
    //        });

    //        React(commands);
    //    }



    //    private void React(IObservable<Utility.Enums.ProcessState> commands)
    //    {
    //        commands
    //        .Subscribe(command =>
    //        {
    //            if (command == Utility.Enums.ProcessState.Blocked)
    //                Pause();
    //            //else if (command == Utility.Enums.ProcessState.Ready)
    //            //{
    //            //    _backgroundWorker.RunWorkerAsync(wa);
    //            //}
    //            else if (command == Utility.Enums.ProcessState.Running)
    //                Resume();
    //            else if (command == Utility.Enums.ProcessState.Terminated)
    //                Cancel();
    //            else
    //                throw new ArgumentOutOfRangeException("argument should be nullable bool");
    //        });
    //    }



    //    public void Cancel()
    //    {

    //        fileDownloader.CancelDownloadAsync();
    //    }

    //    public void Pause()
    //    {
    //        fileDownloader.CancelDownloadAsync();
    //    }

    //    public void Resume()
    //    {
    //        fileDownloader.DownloadFileAsync(tus.Item1,tus.Item2);
    //    }
    //}


}
