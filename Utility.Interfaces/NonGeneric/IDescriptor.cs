using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IDescriptor : IType, IValue, ISetValue, IParentGuid, IGuid, IName, IGet, ISet, IInitialise, IFinalise
    {
        Type ParentType { get; }

    }
}
