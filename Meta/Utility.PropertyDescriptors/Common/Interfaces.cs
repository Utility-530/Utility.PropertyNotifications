namespace Utility.Descriptors
{
    public interface ICollectionItemDescriptor : IDescriptor, IItem
    {
        int Index { get; }

        DateTime? Removed { get;  }
  
    }   
       
    public interface ICollectionItemReferenceDescriptor : IReferenceDescriptor, ICollectionItemDescriptor
    {
   
    }   
    

    public interface ICollectionDescriptor : IDescriptor, ICount
    {
        IEnumerable Collection { get; }

        public Type ElementType { get; }
    }

    public interface ICollectionHeadersDescriptor : ICollectionItemDescriptor
    {
    }

    public interface IHeaderDescriptor : IDescriptor
    {
    }

    public interface IMethodDescriptor : IDescriptor
    {
        object? this[int key] { get; set; }

        void Invoke();

        ICommand Command { get; }
    }

    public interface IMethodsDescriptor : IDescriptor
    {
    }

    public interface IPropertiesDescriptor : IDescriptor
    {
    }
    public interface IReferenceDescriptor : IDescriptor, IChildren
    {
        object Instance { get; }
    }
}
