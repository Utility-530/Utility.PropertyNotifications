using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Utility.Helpers;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;

namespace Utility.Objects
{
    public record PropertyData<T>(PropertyDescriptor Descriptor, object Instance) : PropertyData(Descriptor, Instance), IValue<T>
    {
        public T Value
        {
            get => (T)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }
        object IValue.Value => Value;
    }

    public record PropertyData(PropertyDescriptor Descriptor, object Instance) : MemberData(Descriptor, Instance), IIsReadOnly
    {
        public string Name => Descriptor?.Name ?? "Descriptor not set";

        public string DisplayName => Descriptor.DisplayName;

        public virtual bool IsReadOnly => Descriptor.IsReadOnly;

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        //public virtual TypeConverter Converter => Descriptor.Converter;

        public Type PropertyType => Descriptor.PropertyType;

        // collection
        public virtual Type? CollectionItemPropertyType => PropertyType.IsArray ? PropertyType.GetElementType() : IsCollection ? PropertyType.GenericTypeArguments().SingleOrDefault() : null;

        public virtual int CollectionCount => Instance is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;

        public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

        public virtual TypeConverter Converter => Descriptor.Converter;

        public bool IsCollection => PropertyType != null && PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType);

        public bool IsObservableCollection => PropertyType != null && typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyType);

        public bool IsFlagsEnum => PropertyType.IsFlagsEnum();

        public bool IsValueType => PropertyType.IsValueType;


    }
}
