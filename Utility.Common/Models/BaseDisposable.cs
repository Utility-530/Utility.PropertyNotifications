using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace Utility.Common.Model
{
    public class BaseDisposable : IDisposable
    {
        protected CompositeDisposable CompositeDisposable = new();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
                CompositeDisposable.Dispose();
            }
            // get rid of unmanaged resources
        }
    }

    public class BaseReactiveDisposable : ReactiveObject, IDisposable
    {
        protected CompositeDisposable CompositeDisposable = new();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
                CompositeDisposable.Dispose();
            }
            // get rid of unmanaged resources
        }
    }
}
