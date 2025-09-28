using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IDescriptor : IType, IGetName, IInitialise, IFinalise , IYieldItems
    {
        Type ParentType { get; }

    }
}
