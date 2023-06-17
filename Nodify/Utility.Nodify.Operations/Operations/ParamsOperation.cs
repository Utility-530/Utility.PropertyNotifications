using System;
using System.Linq;
using static Utility.Conversions.ConversionHelper;

namespace Utility.Nodify.Operations
{
    public class ParamsOperation : IOperation
    {
        private readonly Func<double[], double> _func;

        public ParamsOperation(Func<double[], double> func) => _func = func;

        public IOValue[] Execute(params IOValue[] operands)
            => new[] { new IOValue(default, _func.Invoke(operands.Select(a => ChangeType<double>(a.Value)).ToArray())) };
    }
}
