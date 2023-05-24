using Utility.Nodify.Operations;
using Utility.Nodify.Operations.Infrastructure;
using System;
using System.Collections.Generic;


namespace Utility.Nodify.Demo.Infrastructure
{
    public class StandardOperationsFactory : IOperationsFactory
    {
        public IEnumerable<OperationInfo> GetOperations()
        {
            yield break;
            //yield return new OperationInfo
            //{
            //    Type = OperationType.Graph,
            //    Title = "(New) Operation Graph",
            //};
            //yield return new OperationInfo
            //{
            //    Type = OperationType.Calculator,
            //    Title = "Calculator"
            //};
            //yield return new OperationInfo
            //{
            //    Type = OperationType.Expression,
            //    Title = "Custom",
            //};
        }
    }

    public class MethodsOperationsFactory : IOperationsFactory
    {
        public IEnumerable<OperationInfo> GetOperations()
        {
            return OperationFactory.GetOperationsInfo(typeof(Methods));
        }

    }

}

