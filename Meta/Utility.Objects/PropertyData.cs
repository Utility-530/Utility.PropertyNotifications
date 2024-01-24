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

    public record NullablePropertyData<T>(PropertyDescriptor Descriptor, object Instance) : PropertyData(Descriptor, Instance), IValue<T?>
    {
        public T? Value
        {
            get => Descriptor.GetValue(Instance) is T t ? t : default; set { Descriptor.SetValue(Instance, value); }
        }
        object IValue.Value => Value;
    }


    public record PropertyData(PropertyDescriptor Descriptor, object Instance) : IIsReadOnly, IType
    {
        public string Name => Descriptor?.Name ?? "Descriptor not set";

        public string DisplayName => Descriptor.DisplayName;

        public virtual bool IsReadOnly => Descriptor.IsReadOnly;

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        //public virtual TypeConverter Converter => Descriptor.Converter;

        public Type Type => Descriptor.PropertyType;

        // collection
        public virtual Type? CollectionItemPropertyType => Type.IsArray ? Type.GetElementType() : IsCollection ? Type.GenericTypeArguments().SingleOrDefault() : null;

        public virtual int CollectionCount => Instance is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;

        public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

        public virtual TypeConverter Converter => Descriptor.Converter;

        public bool IsCollection => Type != null && Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(Type);

        public bool IsObservableCollection => Type != null && typeof(INotifyCollectionChanged).IsAssignableFrom(Type);

        public bool IsFlagsEnum => Type.IsFlagsEnum();

        public bool IsValueType => Type.IsValueType;

    }
}
