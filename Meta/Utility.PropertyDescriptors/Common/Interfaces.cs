namespace Utility.Descriptors
{
    public interface ICollectionItemDescriptor: IDescriptor
    {
        int Index { get; }
    }



    public interface IMethodDescriptor : IDescriptor
    {
        void Invoke();
    }    

    public interface IMethodsDescriptor : IDescriptor
    {

    }


    public interface IPropertiesDescriptor : IDescriptor
    {

    }
}
