using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Core;
using Utility.Nodify.Operations;

namespace Utility.Nodify.Operations
{
    public interface IOperation : ICore
    {
        IOValue Execute(params IOValue[] operands);
    }

    public interface IFilter
    {
        bool Execute(object value);
    }
}
