using System;

namespace Utility.Reactives.Helpers
{
    public static class SubcribeHelper
    {
        public static T SubscribeTo<T, TValue>(this IObserver<TValue> observer, Func<T> func, out IDisposable disposable) where T : IObservable<TValue>
        {
            var xx = func();
            disposable = xx.Subscribe(observer);
            return xx;
        }

        public static T SubscribeTo<T, TValue>(this IObservable<TValue> observable, Func<T> func, out IDisposable disposable) where T : IObserver<TValue>
        {
            var xx = func();
            disposable = observable.Subscribe(xx);
            return xx;
        }
    }
}