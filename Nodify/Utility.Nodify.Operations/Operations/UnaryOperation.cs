using System;
using static Utility.Conversions.ConversionHelper;

namespace Utility.Nodify.Operations
{
    public class UnaryOperation : IOperation
    {
        private readonly Func<double, double> _func;

        public UnaryOperation(Func<double, double> func) => _func = func;

        public IOValue[] Execute(params IOValue[] operands)
            => new[] { new IOValue(default, _func.Invoke(ChangeType<double>(operands[0].Value))) };
    }
}
