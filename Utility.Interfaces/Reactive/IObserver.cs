using System;

namespace Utility.Interfaces.Reactive
{
    public interface IObserver<T> : System.IObserver<T>
    {
        void OnProgress(int complete, int total);
    }
}
