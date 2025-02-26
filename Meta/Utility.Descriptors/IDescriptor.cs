namespace Utility.Descriptors
{
    public interface IDescriptor : IType, IParentGuid, IGetGuid, IGetName, IInitialise, IFinalise
    {
        Type ParentType { get; }

    }
}


