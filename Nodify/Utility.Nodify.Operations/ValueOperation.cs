using System;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Core;

namespace Utility.Nodify.Operations
{
    public class ValueOperation : IOperation
    {
        private readonly Func<double> _func;

        public ValueOperation(Func<double> func) => _func = func;

        public IOValue Execute(params IOValue[] operands) =>new (default, _func());

        public ISerialise FromString(string str)
        {
            throw new NotImplementedException();
        }
    }
}
