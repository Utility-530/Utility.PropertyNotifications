using Utility.PropertyTrees.Infrastructure;
using System.Collections.Specialized;
using System.Reactive.Linq;
using Utility.Collections;
using Utility.Nodes.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using System.Globalization;
using System.Collections;
using Utility.Conversions;
using Utility.Observables.Generic;
using System.ComponentModel;

namespace Utility.Nodes
{
    public abstract class ValueNode : AutoObject, INode, INotifyCollectionChanged
    {
        protected Collection _children = new();
        protected Collection _branches = new();
        protected Collection _leaves = new();
        private bool flag = false;
        IDisposable? disposable = null;
        private object data;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected ValueNode(Guid guid) : base(guid)
        {
            _children.CollectionChanged += (s, e) => CollectionChanged?.Invoke(this, e);
        }

        public override Key Key => new(Guid, Name, PropertyType);

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
            INode parent = this;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public virtual Type PropertyType => Data.GetType();

        public virtual object? Value
        {
            get
            {
                return GetProperty(Key);
            }
            set
            {
                if (!TryChangeType(value, PropertyType, CultureInfo.CurrentCulture, out object changedValue))
                {
                    throw new ArgumentException("Cannot convert value {" + value + "} to type '" + PropertyType.FullName + "'.");
                }

                SetProperty(Key, changedValue);
            }
        }

        public abstract bool HasChildren { get; }

        protected virtual bool TryChangeType(object value, Type type, IFormatProvider provider, out object changedValue)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ConversionHelper.TryChangeType(value, type, provider, out changedValue);
        }

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

        private void PropertyChanged_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            foreach (var child in Children)
            {
                if (child is ValueNode { Key: Key { Name: var name } } valueNode)
                    if (e.PropertyName == name)
                    {
                        valueNode.OnPropertyChanged(nameof(ValueNode.Value));
                    }
            }
        }

        protected virtual async Task<bool> RefreshAsync()
        {
            if (flag == true)
            {
                await Task.Delay(10000);
                disposable?.Dispose();
                flag = false;
                return await Task.FromResult(true);
            }
            flag = true;

            disposable = Observe<ChildrenResponse, ChildrenRequest>(new ChildrenRequest(Guid, Data))
                .Subscribe(a =>
                {
                    if(a.PropertyNode==null)
                    {
                        throw new Exception("dsv2s331hj f");
                    }
                    if (_children.Any(ass => a.PropertyNode.Key.Guid == (ass as ValueNode)?.Key.Guid) == false)
                        _children.Add(a.PropertyNode);
                }, () => _children.Complete());

            return await Task.FromResult(true);
        }

        public Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public override string ToString()
        {
            return Data.GetType().Name;
        }
    }


    public record ChildrenRequest(Guid Guid, object Data) : Request();

    public record ChildrenResponse(ValueNode PropertyNode, double Completed, double Total) : Response(PropertyNode);
}