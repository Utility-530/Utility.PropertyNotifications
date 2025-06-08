using Splat;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Utility.Interfaces.Exs;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;
using Utility.Helpers;

namespace Utility.Nodes.WPF
{
    public class ViewModel : NotifyPropertyClass, IDisposable
    {
        protected Lazy<INodeSource> source = new(() => Locator.Current.GetService<INodeSource>());
        private readonly Dictionary<string, ObservableCollection<IReadOnlyTree>> dictionary = [];
        private ObservableCollection<IReadOnlyTree> this[string key] => dictionary.GetValueOrNew(key);

        public virtual string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public bool IsSelected { get => isSelected; set => RaisePropertyChanged(ref isSelected, value); }
        protected IEnumerable<IReadOnlyTree> get(string name, [CallerMemberName] string? propertyName = default)
        {
            if (propertyName == null)
                throw new Exception("sd78__22");
            if (dictionary.TryGetValue(propertyName, out var value) == false)
            {
                source
                    .Value
                    .Single(name)
                    .Subscribe(a =>
                    {
                        this[propertyName].Add(a);
                    }).DisposeWith(disposables);
            }
            return this[propertyName];
        }

        #region dispose

        protected System.Reactive.Disposables.CompositeDisposable disposables = new();
        private bool _disposed;
        private bool isSelected;

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        //The Dispose method performs all object cleanup, so the garbage collector no longer needs to call the objects' Object.Finalize override. Therefore, the call to the SuppressFinalize method prevents the garbage collector from running the finalizer. If the type has no finalizer, the call to GC.SuppressFinalize has no effect. The actual cleanup is performed by the Dispose(bool) method overload.
        //In the overload, the disposing parameter is a Boolean that indicates whether the method call comes from a Dispose method(its value is true) or from a finalizer(its value is false).

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                disposables.Dispose();
            }

            // Free unmanaged resources.
            _disposed = true;
        }
        #endregion
    }
}
