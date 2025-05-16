using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Reactives
{
    public static class WhereHelper
    {

        public static IObservable<T> Where<T>(this IObservable<T> source, T value)
        {
            return source.Where(a => a.Equals(value));
        }

        public static IObservable<bool> WhereTrue(this IObservable<bool> source)
        {
            return source.Where(a => a.Equals(true));
        }

        public static IObservable<bool> WhereFalse(this IObservable<bool> source)
        {
            return source.Where(a => a.Equals(false));
        }

        public static IObservable<T> WhereNotDefault<T>(this IObservable<T> observable)
        {
            return observable.Where(a => !(a?.Equals(default(T)) ?? false));
        }

        public static IObservable<T> WhereIsNotNull<T>(this IObservable<T> observable) where T : class
        {
            return observable.Where(a => a != null);
        }    
                
        public static IObservable<T> WhereDefault<T>(this IObservable<T> observable)
        {
            return observable.Where(a => (a?.Equals(default(T)) ?? false));
        }

        public static IObservable<T> WhereIsNull<T>(this IObservable<T> observable) where T : class
        {
            return observable.Where(a => a == null);
        }
        public static IObservable<IEnumerable<T>> WhereAny<T>(this IObservable<IEnumerable<T>> observable)
        {
            return observable.Where(a => a.Any());
        }
        public static IObservable<IEnumerable<T>> WhereEmpty<T>(this IObservable<IEnumerable<T>> observable)
        {
            return observable.Where(a => a.Any() == false);
        }
    }
}
