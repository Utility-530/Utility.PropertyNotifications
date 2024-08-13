using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Utility.Reactives
{
    public static class TaskHelper
    {

        public static Task<T> ToTask<T>(this IObservable<T> enumerable)
        {
            T t = default;
            enumerable.Subscribe(a =>
            {
                t = a;
            });
            while (t != null)
                Task.Delay(100);
            return Task.FromResult(t);
        }
    }
}
