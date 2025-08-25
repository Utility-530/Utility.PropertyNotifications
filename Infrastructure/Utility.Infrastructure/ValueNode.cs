using System.Collections.Specialized;
using Utility.Collections;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using System.Collections;
using System.ComponentModel;
using Utility.Infrastructure;
using Utility.Trees.Abstractions;
using Utility.Interfaces.Generic;

namespace Utility.Nodes
{
    public abstract class ValueNode : AutoObject, IReadOnlyTree, INotifyCollectionChanged
    {
        protected Collection _children = new();
        protected Collection _branches = new();
        protected Collection _leaves = new();
        protected bool flag = false;
        protected IDisposable? disposable = null;
        private object data;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected ValueNode(Guid guid) : base(guid)
        {
            _children.CollectionChanged += (s, e) => CollectionChanged?.Invoke(this, e);
        }
        string IGetKey.Key { get; }
        string ISetKey.Key { set => throw new NotImplementedException(); }

        public IReadOnlyTree Parent { get; set; }

        public abstract string Name { get; }

        public virtual IEnumerable Items
        {
            get
            {
                if (HasChildren)
                    _ = RefreshAsync();
                else
                    _children.Complete();
                return _children;
            }
        }

        public virtual object? Value
        {
            get
            {
                return GetProperty(Key);
            }
            set
            {
                var changedValue = ChangeType(value);
                SetProperty(Key, changedValue);
            }
        }

        public abstract bool HasChildren { get; }

        protected abstract object ChangeType(object value);

        public int Count => _children.Count;

        public virtual object Content => Data.GetType().Name;

        public object Data
        {
            get => data; set
            {
                data = value;
                if (data is INotifyPropertyChanged propertyChanged)
                {
                    propertyChanged.PropertyChanged += PropertyChanged_PropertyChanged;
                }
            }
        }

        Type IType.Type => this.Data.GetType();

        public int Depth
        {
            get
            {
                IReadOnlyTree parent = this;
                int depth = 0;
                while ((parent as IGetParent<IReadOnlyTree>).Parent != null)
                {
                    depth++;
                    parent = (parent as IGetParent<IReadOnlyTree>).Parent;
                }
                return depth;
            }
        }

        protected virtual void PropertyChanged_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Item[]")
                return;
            if (e.PropertyName == "Count")
                return;
            foreach (var child in Items)
            {
                if (child is ValueNode { Key: Key { Name: var name } } valueNode)
                    if (e.PropertyName == name)
                    {
                        valueNode.RaisePropertyChanged(nameof(ValueNode.Value));
                    }
            }
        }

        protected abstract Task<bool> RefreshAsync();

        public Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public override string ToString()
        {
            return Data.GetType().Name;
        }

        public abstract IEnumerator<ITree> GetEnumerator();

        public bool Equals(IReadOnlyTree? other)
        {
            return Key.Equals((other as IGetKey)?.Key);
        }

        public bool Equals(IKey<IEquatable>? other)
        {
            return Key.Equals(other?.Key);
        }

        public Task<object> AsyncClone()
        {
            throw new NotImplementedException();
        }
    }
}