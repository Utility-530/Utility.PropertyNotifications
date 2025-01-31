using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IDescriptor : IType, IParentGuid, IGetGuid, IGetName, IInitialise, IFinalise
    {
        Type ParentType { get; }

    }
}
