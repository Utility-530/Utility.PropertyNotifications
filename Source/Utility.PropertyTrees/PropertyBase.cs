using Utility.PropertyTrees.Abstractions;
using Utility.Helpers;
using Utility.PropertyTrees.Infrastructure;
using Utility.Nodes;
using System.Collections.Specialized;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees
{
    public abstract class PropertyBase : ValueNode, IProperty
    {
        private Lazy<Filters> lazyPredicates => new(() => new DefaultFilter(Data));

        Command<object> command;
        public PropertyBase(Guid guid) : base(guid)
        {
            command = new Command<object>(a =>
            {
                if (a is IGuid guid)
                    this.Send(guid);
                else
                    throw new Exception("sd sss");
            });
        }

        //public abstract string Name { get; }
        public bool IsException => PropertyType == typeof(Exception);
        public bool IsCollection => PropertyType != null && PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType);
        public bool IsObservableCollection => PropertyType != null && typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyType);
        public bool IsFlagsEnum => PropertyType.IsFlagsEnum();
        public bool IsValueType => PropertyType.IsValueType;
        public virtual int CollectionCount => Value is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;
        public virtual System.Type CollectionItemPropertyType => !IsCollection ? null : PropertyType.GetElementType();
        public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;
        public virtual bool IsError { get => this.GetProperty<bool>(); set => this.SetProperty(value); }
        public abstract bool IsReadOnly { get; }
        public override object Content => Name;
        //public IViewModel ViewModel { get; set; }

        public ICommand Command => command;
        public string? DataTemplateKey { get; set; }

        public virtual Type Type { get; set; }

        public virtual PropertyDescriptor Descriptor { get; set; }

        public override Filters Predicates => predicates ?? lazyPredicates.Value;

        protected override async Task<bool> RefreshAsync()
        {
            if ((PropertyType.IsValueType || PropertyType == typeof(string)) != true)
                return await base.RefreshAsync();

            _children.Complete();
            return await Task.FromResult(false);
        }

        public bool IsString => PropertyType == typeof(string);

        public override string ToString()
        {
            return Name;
        }
    }

    public static class PropertyTypeHelper
    {
        public static bool IsCollection(this Type propertyType) => propertyType != null && propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType);
        public static bool IsException(this PropertyDescriptor descriptor) => typeof(Exception).IsAssignableFrom(descriptor.PropertyType);
    }

    public class DefaultFilter : Filters
    {
        private List<Predicate<object>> predicates;

        public DefaultFilter(object data)
        {
            var type = data.GetType();
            predicates = new(){
                    new Predicate<object>(value=>
                {

                    if(value is CollectionItemDescriptor collectionItemDescriptor)
                        return true;
                    if(value is PropertyDescriptor descriptor)
                    {
                        if(descriptor.IsException())
                            return false;
                        if(type.IsCollection())
                        {
                            return false;
                        }
                        int level = descriptor.ComponentType.InheritanceLevel(type);

                        return level == 0 /*<= options.InheritanceLevel*/ && descriptor.IsBrowsable;
                    }

                    return true;
                }) };
        }

        public override IEnumerator<Predicate<object>> GetEnumerator()
        {
            return predicates.GetEnumerator();
        }
    }
}