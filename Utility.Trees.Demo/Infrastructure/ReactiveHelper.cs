using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Reactive.Linq
{

    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT License.
    // See the LICENSE file in the project root for more information. 

    public static class ReactiveHelper
    {
        /// <summary>
        /// Subscribes an element handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
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
            return source.Subscribe/*Unsafe*/(new AnonymousObserver<T>(onNext, Stubs.Throw, Stubs.Nop));
        }
    }

    /// <summary>
    /// Class to create an <see cref="IObserver{T}"/> instance from delegate-based implementations of the On* methods.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public sealed class AnonymousObserver<T> : ObserverBase<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        /// <summary>
        /// Creates an observer from the specified <see cref="IObserver{T}.OnNext(T)"/>, <see cref="IObserver{T}.OnError(Exception)"/>, and <see cref="IObserver{T}.OnCompleted()"/> actions.
        /// </summary>
        /// <param name="onNext">Observer's <see cref="IObserver{T}.OnNext(T)"/> action implementation.</param>
        /// <param name="onError">Observer's <see cref="IObserver{T}.OnError(Exception)"/> action implementation.</param>
        /// <param name="onCompleted">Observer's <see cref="IObserver{T}.OnCompleted()"/> action implementation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="onNext"/> or <paramref name="onError"/> or <paramref name="onCompleted"/> is <c>null</c>.</exception>
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
            _onError = onError ?? throw new ArgumentNullException(nameof(onError));
            _onCompleted = onCompleted ?? throw new ArgumentNullException(nameof(onCompleted));
        }

        /// <summary>
        /// Creates an observer from the specified <see cref="IObserver{T}.OnNext(T)"/> action.
        /// </summary>
        /// <param name="onNext">Observer's <see cref="IObserver{T}.OnNext(T)"/> action implementation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="onNext"/> is <c>null</c>.</exception>
        public AnonymousObserver(Action<T> onNext)
            : this(onNext, Stubs.Throw, Stubs.Nop)
        {
        }

        /// <summary>
        /// Creates an observer from the specified <see cref="IObserver{T}.OnNext(T)"/> and <see cref="IObserver{T}.OnError(Exception)"/> actions.
        /// </summary>
        /// <param name="onNext">Observer's <see cref="IObserver{T}.OnNext(T)"/> action implementation.</param>
        /// <param name="onError">Observer's <see cref="IObserver{T}.OnError(Exception)"/> action implementation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="onNext"/> or <paramref name="onError"/> is <c>null</c>.</exception>
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError)
            : this(onNext, onError, Stubs.Nop)
        {
        }

        /// <summary>
        /// Creates an observer from the specified <see cref="IObserver{T}.OnNext(T)"/> and <see cref="IObserver{T}.OnCompleted()"/> actions.
        /// </summary>
        /// <param name="onNext">Observer's <see cref="IObserver{T}.OnNext(T)"/> action implementation.</param>
        /// <param name="onCompleted">Observer's <see cref="IObserver{T}.OnCompleted()"/> action implementation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="onNext"/> or <paramref name="onCompleted"/> is <c>null</c>.</exception>
        public AnonymousObserver(Action<T> onNext, Action onCompleted)
            : this(onNext, Stubs.Throw, onCompleted)
        {
        }

        /// <summary>
        /// Calls the action implementing <see cref="IObserver{T}.OnNext(T)"/>.
        /// </summary>
        /// <param name="value">Next element in the sequence.</param>
        protected override void OnNextCore(T value) => _onNext(value);

        /// <summary>
        /// Calls the action implementing <see cref="IObserver{T}.OnError(Exception)"/>.
        /// </summary>
        /// <param name="error">The error that has occurred.</param>
        protected override void OnErrorCore(Exception error) => _onError(error);

        /// <summary>
        /// Calls the action implementing <see cref="IObserver{T}.OnCompleted()"/>.
        /// </summary>
        protected override void OnCompletedCore() => _onCompleted();

        //internal ISafeObserver<T> MakeSafe() => new AnonymousSafeObserver<T>(_onNext, _onError, _onCompleted);
    }




    /// <summary>
    /// Abstract base class for implementations of the <see cref="IObserver{T}"/> interface.
    /// </summary>
    /// <remarks>This base class enforces the grammar of observers where <see cref="IObserver{T}.OnError(Exception)"/> and <see cref="IObserver{T}.OnCompleted()"/> are terminal messages.</remarks>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public abstract class ObserverBase<T> : IObserver<T>, IDisposable
    {
        private int _isStopped;

        /// <summary>
        /// Creates a new observer in a non-stopped state.
        /// </summary>
        protected ObserverBase()
        {
            _isStopped = 0;
        }

        /// <summary>
        /// Notifies the observer of a new element in the sequence.
        /// </summary>
        /// <param name="value">Next element in the sequence.</param>
        public void OnNext(T value)
        {
            if (Volatile.Read(ref _isStopped) == 0)
            {
                OnNextCore(value);
            }
        }

        /// <summary>
        /// Implement this method to react to the receival of a new element in the sequence.
        /// </summary>
        /// <param name="value">Next element in the sequence.</param>
        /// <remarks>This method only gets called when the observer hasn't stopped yet.</remarks>
        protected abstract void OnNextCore(T value);

        /// <summary>
        /// Notifies the observer that an exception has occurred.
        /// </summary>
        /// <param name="error">The error that has occurred.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error"/> is <c>null</c>.</exception>
        public void OnError(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            if (Interlocked.Exchange(ref _isStopped, 1) == 0)
            {
                OnErrorCore(error);
            }
        }

        /// <summary>
        /// Implement this method to react to the occurrence of an exception.
        /// </summary>
        /// <param name="error">The error that has occurred.</param>
        /// <remarks>This method only gets called when the observer hasn't stopped yet, and causes the observer to stop.</remarks>
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Justification = "Same name as in the IObserver<T> definition of OnError in the BCL.")]
        protected abstract void OnErrorCore(Exception error);

        /// <summary>
        /// Notifies the observer of the end of the sequence.
        /// </summary>
        public void OnCompleted()
        {
            if (Interlocked.Exchange(ref _isStopped, 1) == 0)
            {
                OnCompletedCore();
            }
        }

        /// <summary>
        /// Implement this method to react to the end of the sequence.
        /// </summary>
        /// <remarks>This method only gets called when the observer hasn't stopped yet, and causes the observer to stop.</remarks>
        protected abstract void OnCompletedCore();

        /// <summary>
        /// Disposes the observer, causing it to transition to the stopped state.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Core implementation of <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> if the Dispose call was triggered by the <see cref="IDisposable.Dispose"/> method; <c>false</c> if it was triggered by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Volatile.Write(ref _isStopped, 1);
            }
        }

        internal bool Fail(Exception error)
        {
            if (Interlocked.Exchange(ref _isStopped, 1) == 0)
            {
                OnErrorCore(error);
                return true;
            }

            return false;
        }
    }

    internal static class Stubs<T>
    {
        public static readonly Action<T> Ignore = static _ => { };
        public static readonly Func<T, T> I = static _ => _;
    }

    internal static class Stubs
    {
        public static readonly Action Nop = static () => { };
        public static readonly Action<Exception> Throw = (e) => { throw e; };
    }

#if !NO_THREAD
    internal static class TimerStubs
    {
#if NETSTANDARD1_3
        public static readonly System.Threading.Timer Never = new System.Threading.Timer(static _ => { }, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
#else
        public static readonly System.Threading.Timer Never = new System.Threading.Timer(static _ => { });
#endif
    }
#endif
}

