using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace Utility.Tasks
{
    //public  class HtmlDocumentDownloaderQueue: AsyncWorkerQueue<object> /*: INotifyPropertyChanged*/ //where T : new()
    //{

    //    public ISubject<Utility.Enums.ProcessState> commands { get; } = new Subject<Utility.Enums.ProcessState>();


    //    public HtmlDocumentDownloaderQueue(IObservable<Action<Uri>> mainMethod) : base(_client.GetCompletion().Select(_ => _.UserState))
    //    {
    //        mainMethod.Subscribe(_ =>
    //        {
    //            var x = new FileDownloadWorkerItem<object>(_.Item1, _.Item2, _client);
    //            Enqueue(x);
    //        }, e =>
    //         Console.WriteLine(e.Message), () => { });

    //        React(commands);
    //    }

    //    private void React(IObservable<Utility.Enums.ProcessState> commands)
    //    {
    //        commands
    //        .Subscribe(command =>
    //        {
       
    //            if (command == Utility.Enums.ProcessState.Terminated)
    //                Cancel();
    //            else
    //                throw new ArgumentOutOfRangeException("argument should be nullable bool");
    //        });
    //    }

    //    public void Cancel()
    //    {
    //        if (_client.IsBusy)
    //            _client.CancelAsync();
    //    }


    //}
}