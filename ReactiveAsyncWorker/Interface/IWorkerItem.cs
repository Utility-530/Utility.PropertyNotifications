using ReactiveAsyncWorker.Model;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace ReactiveAsyncWorker
{


    public interface IWorkerItem<TOutput> : IObservable<ProgressState>
    {
        string Key { get; }

        void Start();

        TOutput State { get; }

        UtilityEnum.ProcessState Status { get; }
        IObservable<TOutput> StateChanges { get; }
    }
}
