namespace Utility.Descriptors
{
    public interface ICollectionItemDescriptor : IDescriptor
    {
        int Index { get; }
    }

    public interface ICollectionDescriptor : IDescriptor, ICount
    {
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
    }

    public interface IMethodsDescriptor : IDescriptor
    {
    }

    public interface IPropertiesDescriptor : IDescriptor
    {
    }
    public interface IReferenceDescriptor : IDescriptor, IChildren
    {

    }
}
