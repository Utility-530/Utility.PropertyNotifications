using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Abstract;
using Utility.FileSystem.Transfer.Service.Infrastructure;

namespace Utility.FileSystem.Transfer.Service
{
    public class CopyService : ITransferer
    {
        private readonly Subject<Progress> progress = new Subject<Progress>();

        public IObservable<Progress> Transfer(params string[] args)
        {
            string source = args[0];
            string destination = args[1];
            Init();
            return (IObservable<Progress>)this.progress;

            async void Init() => await FileTransferHelper.CopyWithProgressAsync(source, destination, (Action<Progress>)(a => this.progress.OnNext(a)), true).ContinueWith((Action<Task<OperationResult>>)(a => { }));
        }
    }
}