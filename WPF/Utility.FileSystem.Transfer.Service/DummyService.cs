using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Abstract;

namespace Utility.FileSystem.Transfer.Service
{
    public class DummyService : ITransferer
    {
        public IObservable<Progress> Transfer(params string[] args) => 
            Task.Delay(TimeSpan.FromSeconds(2.0))
                .ToObservable()
                .Select(_ => DateTime.Now)
                .StartWith(DateTime.Now)
                .Select(a => new Progress(a, 50L, 100L))
            .Concat(Task
                .Delay(TimeSpan.FromSeconds(2.0))
                .ToObservable().Select(_ => DateTime.Now)
                .StartWith(DateTime.Now)
                .Select(a => new Progress(a, 100L, 100L)));
    }
}