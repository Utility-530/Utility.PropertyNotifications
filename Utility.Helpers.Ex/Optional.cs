using Optional;
using Optional.Unsafe;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace Utility.Helpers.Ex
{
    public static class OptionalHelper
    {
        public static Option<T, Exception> SomeEx<T>(this T value)
        {
            return Option.Some<T, Exception>(value);
        }

        public static Option<T, Exception> NoneEx<T>(this Exception ex)
        {
            return Option.None<T, Exception>(ex);
        }

        public static IObservable<T> WhereIsNotNull<T>(this IObservable<Option<T, Exception>> ex)
        {
            return ex.Select(a => a.ValueOrDefault()).WhereNotNull();
        }
    }
}