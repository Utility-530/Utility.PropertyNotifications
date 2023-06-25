using Utility.PropertyTrees.Infrastructure;
using System.Collections.Specialized;
using Utility.Collections;
using Utility.Nodes.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using System.Collections;
using System.ComponentModel;

namespace Utility.Nodes
{
    // property node
    public abstract class ValueNode : AutoObject, INode, INotifyCollectionChanged
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
        IEquatable INode.Key => this.Key;      

        public abstract string Name { get; }

        public INode Parent { get; set; }

        public virtual IEnumerable Ancestors => GetAncestors();

        public virtual IObservable Children
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

        private IEnumerable GetAncestors()
        {
            INode parent = this.Parent;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
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


        protected virtual void PropertyChanged_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Item[]")
                return;
            if (e.PropertyName == "Count")
                return;
            foreach (var child in Children)
            {
                if (child is ValueNode { Key: Key { Name: var name } } valueNode)
                    if (e.PropertyName == name)
                    {
                        valueNode.OnPropertyChanged(nameof(ValueNode.Value));
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
    }

}