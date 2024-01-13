using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Reactive.Helpers
{
    public static class OnNextAwaiterHelper
    {
        public static NotifyOnCompletion<T> GetAwaiter<T>(this IObservable<T> lazy) => new (lazy);
    }


    public class NotifyOnCompletion<T> : INotifyCompletion
    {
        private IDisposable disposable;
        private bool isCompleted;
        private readonly List<T> values = new();

        public NotifyOnCompletion(IObservable<T> observable)
        {
            disposable =
                observable.Subscribe(a =>
                {
                    values.Add(a);
                }, () => isCompleted = true);
        }

        public void OnCompleted(Action continuation)
        {
            disposable.Dispose();
            continuation();
        }

        public List<T> GetResult() => values;

        public bool IsCompleted => isCompleted;
    }
}



