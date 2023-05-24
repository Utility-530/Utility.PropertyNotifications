using Utility.Nodify.Operations;

namespace Utility.Nodify.Operations
{
    public interface IOperation
    {
        IOValue[] Execute(params IOValue[] operands);
    }

    public interface IFilter
    {
        bool Execute(object value);
    }
}
