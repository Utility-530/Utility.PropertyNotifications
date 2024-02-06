using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Changes;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyDescriptors
{
    //public enum DescriptorType
    //{
    //    CollectionItem,
    //    Property,
    //}

    public interface IMemberDescriptor: IIsReadOnly, IType
    {
        Guid Guid { get;  }
        string? Name { get; }
        bool IsReadOnly { get; }
        string Category { get; }
        System.Type Type { get; }
        bool IsValueOrStringProperty { get; }
        System.Type ComponentType { get; }

        IObservable<Change<IMemberDescriptor>> GetChildren();
        object GetValue();
        void SetValue(object value);

    }

    public interface ICollectionItemDescriptor: IMemberDescriptor
    {
        int Index { get; }
    }



    public interface IMethodDescriptor : IMemberDescriptor
    {
        void Invoke();
    }    

    public interface IMethodsDescriptor : IMemberDescriptor
    {

    }


    public interface IPropertiesDescriptor : IMemberDescriptor
    {

    }


}
