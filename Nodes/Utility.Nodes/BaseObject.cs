using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class BaseObject: NotifyPropertyChangedBase, IIsReadOnly
    {


        public BaseObject(PropertyDescriptor Descriptor, object Instance)
        {
            this.Descriptor = Descriptor;
            this.Instance = Instance;
        }
        public PropertyDescriptor Descriptor { get;  }

        public object Instance { get;  }

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
