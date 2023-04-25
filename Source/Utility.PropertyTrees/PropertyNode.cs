using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using Utility.Collections;
using Utility.Nodes.Abstractions;
using Utility.Interfaces.NonGeneric;
using System.Collections.ObjectModel;
using Utility.Helpers;

namespace Utility.PropertyTrees
{
    public class PropertyNode : AutoObject, INode, IPropertyNode, INotifyCollectionChanged
    {
        protected Collection _children = new();
        protected Collection _branches = new();
        protected Collection _leaves = new();
        private bool flag = false;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private Lazy<DescriptorFilters> lazyPredicates;
        private DescriptorFilters predicates;

        public PropertyNode(Guid guid) : base(guid)
        {
            lazyPredicates = new(() => new DefaultFilter(Data));
            _children.CollectionChanged += (s, e) => CollectionChanged?.Invoke(this, e);
        }

        public INode Parent { get; set; }

        public virtual IEnumerable Ancestors
        {
            get
            {
                return GetAncestors();
            }
        }

        public virtual IObservable Children
        {
            get
            {
                _ = RefreshAsync();
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

        public override string ToString()
        {
            return Data.GetType().Name;
        }

        public int Count => _children.Count;

        public virtual object Content => Data.GetType().Name;

        public object Data { get; set; }

        IEnumerable IPropertyNode.Children => Children;

        protected virtual async Task<bool> RefreshAsync()
        {
            if (flag == true)
                return await Task.FromResult(true);

            flag = true;


            PropertyFilter
                .Instance
                .FilterProperties(Data, Guid, Predicates)
                .Subscribe(prop =>
                {
                    //if (prop.IsValueType || prop.IsString)
                    //{
                    //    _leaves.Add(prop);
                    //}
                    //else
                    //    _branches.Add(prop);
                    _children.Add(prop);
                },
                e =>
                {
                    Errors.Add(ExceptionHelper.CombineMessages(e));                   
                },
                () =>
                {

                });

            return await Task.FromResult(true);
        }

        public Collection Errors { get; } = new();

        public Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public DescriptorFilters Predicates { get => predicates ?? lazyPredicates.Value; set => predicates = value; }
    }

    public class DefaultFilter : DescriptorFilters
    {
        private List<Predicate<PropertyDescriptor>> predicates;

        public DefaultFilter(object data)
        {
            var type = data.GetType();
            predicates = new(){
                new Predicate<PropertyDescriptor>(descriptor=>
            {
                      int level = descriptor.ComponentType.InheritanceLevel(type);

                   return level == 0 /*<= options.InheritanceLevel*/ && descriptor.IsBrowsable;
            }) };
        }

        public override IEnumerator<Predicate<PropertyDescriptor>> GetEnumerator()
        {
            return predicates.GetEnumerator();
        }
    }
}