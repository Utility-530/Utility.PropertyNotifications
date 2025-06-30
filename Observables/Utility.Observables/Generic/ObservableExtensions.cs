// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

//using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;

namespace Utility.Observables.Generic
{

    internal static class Stubs<T>
    {
        public static readonly Action<T> Ignore = static _ => { };
        public static readonly Func<T, T> I = static _ => _;

        public static readonly Action Nop = static () => { };
        public static readonly Action<int, int> IgnoreProgress = static (a, b) => { };
        public static readonly Action<Exception> Throw = static exception => { ExceptionDispatchInfo.Capture(exception).Throw(); };
    }

#if !NO_THREAD
    internal static class TimerStubs
    {
#if NETSTANDARD1_3
        public static readonly System.Threading.Timer Never = new System.Threading.Timer(static _ => { }, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
#else
        public static readonly Timer Never = new Timer(static _ => { });
#endif
    }
#endif

    /// <summary>
    /// Provides a set of static methods for subscribing delegates to observables.
    /// </summary>
    public static class ObservableExtensions
    {
        #region Subscribe delegate-based overloads

        /// <summary>
        /// Subscribes to the observable sequence without specifying any handlers.
        /// This method can be used to evaluate the observable sequence for its side-effects only.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this Interfaces.Reactive.IObservable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            //
            // [OK] Use of unsafe Subscribe: non-pretentious constructor for an observer; this overload is not to be used internally.
            //
            return source.Subscribe/*Unsafe*/(new Observer<T>(Stubs<T>.Ignore, Stubs<T>.Throw, Stubs<T>.Nop, Stubs<T>.IgnoreProgress));
        }

        /// <summary>
        /// Subscribes an element handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this Interfaces.Reactive.IObservable<T> source, Action<T> onNext)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (onNext == null)
            {
                throw new ArgumentNullException(nameof(onNext));
            }

            //
            // [OK] Use of unsafe Subscribe: non-pretentious constructor for an observer; this overload is not to be used internally.
            //
            return source.Subscribe/*Unsafe*/(new Observer<T>(onNext, Stubs<T>.Throw, Stubs<T>.Nop, Stubs<T>.IgnoreProgress));
        }

        /// <summary>
        /// Subscribes an element handler and an exception handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> or <paramref name="onError"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this Interfaces.Reactive.IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (onNext == null)
            {
                throw new ArgumentNullException(nameof(onNext));
            }

            if (onError == null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            //
            // [OK] Use of unsafe Subscribe: non-pretentious constructor for an observer; this overload is not to be used internally.
            //
            return source.Subscribe/*Unsafe*/(new Observer<T>(onNext, onError, Stubs<T>.Nop, Stubs<T>.IgnoreProgress));
        }

        /// <summary>
        /// Subscribes an element handler and a completion handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> or <paramref name="onCompleted"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this Interfaces.Reactive.IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (onNext == null)
            {
                throw new ArgumentNullException(nameof(onNext));
            }

            if (onCompleted == null)
            {
                throw new ArgumentNullException(nameof(onCompleted));
            }

            //
            // [OK] Use of unsafe Subscribe: non-pretentious constructor for an observer; this overload is not to be used internally.
            //
            return source.Subscribe/*Unsafe*/(new Observer<T>(onNext, Stubs<T>.Throw, onCompleted, Stubs<T>.IgnoreProgress));
        }

        /// <summary>
        /// Subscribes an element handler, an exception handler, and a completion handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
        /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> or <paramref name="onError"/> or <paramref name="onCompleted"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this Interfaces.Reactive.IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (onNext == null)
            {
                throw new ArgumentNullException(nameof(onNext));
            }

            if (onError == null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            if (onCompleted == null)
            {
                throw new ArgumentNullException(nameof(onCompleted));
            }

            //
            // [OK] Use of unsafe Subscribe: non-pretentious constructor for an observer; this overload is not to be used internally.
            //
            return source.Subscribe/*Unsafe*/(new Observer<T>(onNext, onError, onCompleted, Stubs<T>.IgnoreProgress));
        }


        /// <summary>
        /// Subscribes an element handler, an exception handler, and a completion handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
        /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> or <paramref name="onError"/> or <paramref name="onCompleted"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this Interfaces.Reactive.IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted, Action<int, int> progress)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (onNext == null)
            {
                throw new ArgumentNullException(nameof(onNext));
            }

            if (onError == null)
            {
                throw new ArgumentNullException(nameof(onError));
            }

            if (onCompleted == null)
            {
                throw new ArgumentNullException(nameof(onCompleted));
            }

            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            //
            // [OK] Use of unsafe Subscribe: non-pretentious constructor for an observer; this overload is not to be used internally.
            //
            return source.Subscribe/*Unsafe*/(new Observer<T>(onNext, onError, onCompleted, progress));
        }

        #endregion Subscribe delegate-based overloads


        public static Interfaces.Reactive.IObservable<T> Create<T>(Func<Interfaces.Reactive.IObserver<T>, IDisposable> subscribe)
        {
            var subject = new Subject<T>();

            var disposable = subscribe(subject);

            subject.Add(disposable);

            return subject;
        }


        public static Interfaces.Reactive.IObservable<T> Empty<T>()
        {
            var subject = new Subject<T>(); 
            subject.OnCompleted();
            return subject;
        }



        public static Interfaces.Reactive.IObservable<TOut> Select<TIn, TOut>(this Interfaces.Reactive.IObservable<TIn> observable, Func<TIn, TOut> func)
        {
            var subject = new Subject<TIn, TOut>(func);
            var disposable = observable.Subscribe(subject);
            subject.Add(disposable);
            return subject; 
        }

        public static Interfaces.Reactive.IObservable<TOutput> CastAs<TInput, TOutput>(this Interfaces.Reactive.IObservable<TInput> observable) where TOutput : class
        {
            return observable.Select(a=>(a as TOutput ?? throw new Exception("dfs w2121")));
        }      
 
    }
}