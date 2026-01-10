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

        HashSet<INodeViewModel> _pending = new();

        public void Add(INodeViewModel node)
        {
            if (_pending.Contains(node))
                return;

            _pending.Add(node);
            Utility.Globals.UI.Post(a =>
            {
                _pending.Remove(node);
                _nodes.Add(node);
            }, node);

        }

        public void Remove(string key)
        {
            var node = Nodes.SingleOrDefault(a => a.Key() == key);
            _nodes.Remove(node);
        }


        public INodeViewModel? Find(string key)
        {
            return _pending.SingleOrDefault(a => a.Key() == key) ??
                _nodes.SingleOrDefault(a => a.Key() == key);
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

}
