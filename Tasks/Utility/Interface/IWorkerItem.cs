using Utility.Tasks.Model;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using UtilityInterface.Generic;

namespace Utility
{
    public interface IWorkerItem : IObservable<IProgressState>, IKey<string>
    {      
        void Start();

        void Pause();

        void Stop();
    }

    public interface IWorkerItem<T> : IObservable<IProgressState<T>>, IKey<string>
    {
        void Start();

        void Pause();

        void Stop();
    }
}
