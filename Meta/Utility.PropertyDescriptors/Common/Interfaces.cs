namespace Utility.Descriptors
{
    //public enum DescriptorType
    //{
    //    CollectionItem,
    //    Property,
    //}



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
