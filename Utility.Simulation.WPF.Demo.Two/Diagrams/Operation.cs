using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Nodify.Operations;
using Utility.Nodify.Operations.Infrastructure;

namespace Utility.Nodify.Demo.Infrastructure
{

    public class DynamicOperationsFactory : IOperationsFactory
    {
        private readonly IContainer container;

        public DynamicOperationsFactory(IContainer container)
        {
            this.container = container;
        }

        public IEnumerable<OperationInfo> GetOperations()
        {
            return container.Resolve<OperationInfo[]>();
     
        }
    }

    public class LambdaOperation : IOperation
    {
        private readonly Func<IOValue[], IOValue[]> func;

        public LambdaOperation(Func<IOValue[], IOValue[]> func)
        {
            this.func = func;
        }

        public IOValue[] Execute(params IOValue[] operands)
        {
            return func(operands);
        }
    }

}
