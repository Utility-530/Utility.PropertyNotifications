using System;

namespace Utility.Interfaces.Generic
{
    public interface IObserver<T> : System.IObserver<T>
    {
        void OnProgress(int complete, int total);
    }
}
