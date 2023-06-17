using System;
using static Utility.Conversions.ConversionHelper;

namespace Utility.Nodify.Operations
{
    public class BinaryOperation : IOperation
    {
        private readonly Func<double, double, double> _func;

        public BinaryOperation(Func<double, double, double> func) => _func = func;

        public IOValue[] Execute(params IOValue[] operands)
            => new[] { new IOValue(default, _func.Invoke(ChangeType<double>(operands[0].Value), (double)ChangeType<double>(operands[1].Value)))};
    }
}
