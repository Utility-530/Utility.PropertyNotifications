using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utility.Interfaces.NonGeneric;

namespace Utility.Observables
{
    public class Disposable : IObservableCollection<IDisposable>, ICollection<IDisposable>, IDisposable
    {
        private bool disposed;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private Collection<IObserver<NotifyCollectionChangedEventArgs>> observers = new();

        public Disposable(IDisposable disposable) : this()
        {
            Disposables.Add(disposable);
        }

        public Disposable()
        {
            Disposables.CollectionChanged += Disposables_CollectionChanged;
        }

        private void Disposables_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var observer in Observers)
                observer.OnNext(e);
        }

        public ObservableCollection<IDisposable> Disposables { get; } = new();

        public IEnumerable<IObserver<NotifyCollectionChangedEventArgs>> Observers => observers;

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    foreach (var disposable in Disposables)
                        disposable.Dispose();
                }
            }
            disposed = true;
        }

        public int Count => Disposables.Count;

        public bool IsReadOnly => false;

        public IDisposable this[int index] { get => Disposables[index]; set => Disposables[index] = value; }

        public int IndexOf(IDisposable item)
        {
            return Disposables.IndexOf(item);
        }

        public void Insert(int index, IDisposable item)
        {
            Disposables.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Disposables.RemoveAt(index);
        }

        public void Add(IDisposable item)
        {
            Disposables.Add(item);
        }

        public void Clear()
        {
            Disposables.Clear();
        }

        public bool Contains(IDisposable item)
        {
            return Disposables.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            Disposables.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDisposable item)
        {
            return Disposables.Remove(item);
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return Disposables.GetEnumerator();
        }

        public IDisposable Subscribe(IObserver<NotifyCollectionChangedEventArgs> observer)
        {
            return new Disposer<NotifyCollectionChangedEventArgs>(observers, observer);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Disposables.GetEnumerator();
        }
    }

    public sealed class Disposer<T> : Generic.Disposer<IObserver<T>, T>
    {
        public Disposer(ICollection<IObserver<T>> observers, IObserver<T> observer) : base(observers, observer)
        {
        }
    }
}