using System;
using System.Reactive.Subjects;
using Utility.Observables;

namespace Utility.Common.Models
{
    public class FunctionNode<TIn, TOut> : IObserver<TIn>, IObservable<TOut>, IDisposable
    {
        private readonly ReplaySubject<TIn> inSubject = new(1);
        private readonly ReplaySubject<TOut> outSubject = new(1);
        private readonly Disposable disposer;

        private FunctionNode(Func<IObservable<TIn>, IObservable<TOut>> func)
        {
            disposer = new Disposable(func(inSubject).Subscribe(outSubject));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException(" ffff. / / dfggdfgf fgfdf");
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException("uiytyt. / / dfggdfgf fgfdf");
        }

        public void OnNext(TIn value)
        {
            inSubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            return outSubject.Subscribe(observer);
        }

        public static FunctionNode<TIn, TOut> Create(Func<IObservable<TIn>, IObservable<TOut>> func)
        {
            return new FunctionNode<TIn, TOut>(func);
        }

        #region dispose

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            disposer.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion dispose
    }
}