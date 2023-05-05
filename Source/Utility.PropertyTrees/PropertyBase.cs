using Utility.PropertyTrees.Abstractions;
using System.Collections;
using Utility.Conversions;
using Utility.Helpers;
using Utility.PropertyTrees.Infrastructure;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees
{
    public abstract class PropertyBase : PropertyNode, IProperty
    {
        public PropertyBase(Guid guid) : base(guid)
        {
        }

        public virtual string Name { get; }
        public bool IsCollection => PropertyType != null ? PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType) : false;
        public bool IsFlagsEnum => PropertyType.IsFlagsEnum();
        public bool IsValueType => PropertyType.IsValueType;
        public virtual int CollectionCount => Value is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;
        public virtual System.Type CollectionItemPropertyType => !IsCollection ? null : PropertyType.GetElementType();
        public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;
        public virtual bool IsError { get => this.GetProperty<bool>(); set => this.SetProperty(value); }

        //public bool IsValid => throw new NotImplementedException();

        public virtual System.Type PropertyType => Data.GetType();
        public abstract bool IsReadOnly { get; }
        public override object Content => Name;
        public IViewModel ViewModel { get; set; }

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

        protected virtual bool TryChangeType(object value, System.Type type, IFormatProvider provider, out object changedValue)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ConversionHelper.TryChangeType(value, type, provider, out changedValue);
        }
     
    }
}