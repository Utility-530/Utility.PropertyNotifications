using System;
using System.Reactive.Subjects;
using Utility.Observables;

namespace Utility.Common.Models
{
    public class OutputNode<TOut> : IObservable<TOut>, IDisposable
    {
        private ReplaySubject<TOut> outSubject = new(1);
        private readonly Disposable disposer;

        private OutputNode(Func<IObservable<TOut>> func)
        {
            disposer = new Disposable(func().Subscribe(outSubject));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException("fvr fesdf vree");
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException(". / / dfggdfgf fgfdf");
        }

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            return outSubject.Subscribe(observer);
        }

        public static OutputNode<TOut> Create(Func<IObservable<TOut>> func)
        {
            return new OutputNode<TOut>(func);
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