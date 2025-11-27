using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Models;
using Utility.Nodify.Operations;
using Utility.Observables;
using Utility.Observables.Generic;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Structs.Repos;
using CompositeDisposable = Utility.Observables.CompositeDisposable;
using Disposable = System.Reactive.Disposables.Disposable;

namespace Utility.Nodes.Meta
{

    public partial class NodesStore
    {
        private bool _disposed;
        private const string ERROR_INVALID_GUID = "Invalid GUID format";
        private readonly ObservableCollection<INodeViewModel> _nodes = [];

        public IReadOnlyCollection<INodeViewModel> Nodes => _nodes;



        private IObservable<INodeViewModel> many(string key)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(NodeEngine));

            return Observable.Create<INodeViewModel>(observer =>
            {
                if (!Guid.TryParse(key, out var _))
                    throw new Exception(ERROR_INVALID_GUID);

                return _nodes
                    .AndAdditions<INodeViewModel>()
                    .Where(node => node.Key().Equals(key))
                    .Take(1)
                    .Subscribe(node =>
                    {
                        observer.OnNext(node);
                        observer.OnCompleted();
                    });
            });
        }
        #region disposepattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _nodes?.Clear();
                //_dirty?.Dispose();
            }

            _disposed = true;
        }
        #endregion disposepattern
    }

    public static class NodesStoreHelper
    {
        public static void Remove(this INodeSource nodeSource, string key)
        {
            var node = nodeSource.Nodes.SingleOrDefault(a => a.Key() == key);
            if(nodeSource.Nodes is IList<INodeViewModel> nodes)
                nodes.Remove(node);
            else
                throw new InvalidOperationException("Nodes collection is not mutable.");
        }
    }
    

}
