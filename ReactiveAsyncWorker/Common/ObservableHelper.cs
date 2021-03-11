using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace ReactiveAsyncWorker
{
    public static class ObservableHelper
    {

        public static IObservable<int> SelectSubtractions(this IObservable<int> observable)
        {
               return SelectChanges(observable)
                        .Where(a => a.Item1==false)
                        .Select(a => a.Item2);
        }

        public static IObservable<int> SelectAdditions(this IObservable<int> observable)
        {
            return SelectChanges(observable)
                     .Where(a => a.Item1)
                     .Select(a => a.Item2);
        }

        public static IObservable<(bool, int)> SelectChanges(this IObservable<int> observable)
        {
            return observable
                     .Scan((false, 0), (a, b) => ((b - a.Item2) > 0, b));
              
        }
    }
}
