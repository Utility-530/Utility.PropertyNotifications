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

namespace Utility.Nodes
{
    public abstract class ValueNode : AutoObject, INode, INotifyCollectionChanged
    {
        protected Collection _children = new();
        protected Collection _branches = new();
        protected Collection _leaves = new();
        private bool flag = false;
        protected Filters predicates;
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

        public object Data { get; set; }

        protected virtual async Task<bool> RefreshAsync()
        {
            if (flag == true)
            {
                await Task.Delay(1000);
                flag = false;
                return await Task.FromResult(true);
            }
            flag = true;

            _ = Observe<ChildrenResponse, ChildrenRequest>(new ChildrenRequest(Data, Guid, Predicates))
                .Subscribe(a => _children.Add(a.PropertyNode), () => _children.Complete());

            return await Task.FromResult(true);
        }

        public Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public virtual Filters Predicates { get => predicates; set => predicates = value; }

        public override string ToString()
        {
            return Data.GetType().Name;
        }
    }


    public record ChildrenRequest(object Data, Guid Guid, Filters Filters) : Request;

    public record ChildrenResponse(ValueNode PropertyNode, double Completed, double Total) : Response(PropertyNode);

}