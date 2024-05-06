using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IDescriptor : IType, IParentGuid, IGuid, IName, IInitialise, IFinalise
    {
        Type ParentType { get; }

    }
}
