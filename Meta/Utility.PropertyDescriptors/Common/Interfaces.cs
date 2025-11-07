namespace Utility.PropertyDescriptors
{
    public interface ICollectionDescriptor : IDescriptor, IProliferation, ICount, IObserver<RefreshEventArgs>
    {
        Type ElementType { get; }
    }

    public interface ICollectionHeadersDescriptor : IDescriptor, ICount
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

    public interface IReferenceDescriptor : IDescriptor, ICount
    {
        object Instance { get; }
    }
}