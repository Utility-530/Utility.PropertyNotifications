using System;
using System.Reactive.Linq;
using UtilityEnum;
using System.Net;
using ReactiveAsyncWorker.Model;

namespace ReactiveAsyncWorker
{
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


    //public interface IFileDownloadWorkerFactory:  IFactory<IFileDownloadWorkerItem, FileDownload>
    //{
    //    public IFileDownloadWorkerItem Create(FactoryArguments2 args);
    //}

    public class FileDownloadQueue : AsyncWorkerQueue<object> /*: INotifyPropertyChanged*/ //where T : new()
    {
        static WebClient client = new WebClient();

        public FileDownloadQueue(IObservable<FileDownload> mainMethod, IFactory<IWorkerItem<object>, FactoryFileDownloadInput> factory) : base(client.GetCompletion().Where(a => a.Error == null).Select(arg => arg.UserState))
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
                    Enqueue(factory.Create( new FactoryFileDownloadInput(client, f)));
                }, e =>
                 Console.WriteLine(e.Message), () => { });

            Init(commands);

            void Init(IObservable<ProcessState> commands)
            {
                commands
                .Subscribe(command =>
                {
                    //if (command == UtilityEnum.ProcessState.Blocked)

                    //else if (command == UtilityEnum.ProcessState.Ready)
                    //{
                    //    _backgroundWorker.RunWorkerAsync(wa);
                    //}
                    //else if (command == UtilityEnum.ProcessState.Running)

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

    //    public ISubject<UtilityEnum.ProcessState> commands { get; } = new Subject<UtilityEnum.ProcessState>();


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



    //    private void React(IObservable<UtilityEnum.ProcessState> commands)
    //    {
    //        commands
    //        .Subscribe(command =>
    //        {
    //            if (command == UtilityEnum.ProcessState.Blocked)
    //                Pause();
    //            //else if (command == UtilityEnum.ProcessState.Ready)
    //            //{
    //            //    _backgroundWorker.RunWorkerAsync(wa);
    //            //}
    //            else if (command == UtilityEnum.ProcessState.Running)
    //                Resume();
    //            else if (command == UtilityEnum.ProcessState.Terminated)
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
