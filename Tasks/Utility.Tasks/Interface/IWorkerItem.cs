using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using Utility.Interfaces.Generic;
using Utility.Progressions;

namespace Utility
{
    public interface IWorkerItem : System.IObservable<IProgressState>, IKey<string>
    {      
        void Start();

        void Pause();

        void Stop();
    }

    public interface IWorkerItem<T> : System.IObservable<IProgressState<T>>, IKey<string>
    {
        void Start();

        void Pause();

        void Stop();
    }
}
