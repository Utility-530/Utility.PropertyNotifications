using System;
using System.Collections.Generic;
using System.Linq;
//using Utility.Nodify.Operations;
//using Utility.Nodify.Operations.Infrastructure;
//using static Utility.Conversions.ConversionHelper;

namespace Utility.Nodify.Demo.Infrastructure
{
    //public class CustomOperationsFactory : INodeFactory
    //{

    //    public const string Source = nameof(Source);
    //    public const string Target = nameof(Target);
    //    public const string Interface = nameof(Interface);

    //    public IEnumerable<OperationInfo> GetOperations()
    //    {
    //        yield return new OperationInfo
    //        {
    //            Title = Source,
    //            Type = OperationType.Normal,
    //            Operation = new SourceOperation(),
    //            MinInput = 1,
    //            MaxInput = 1,
    //        };
    //        yield return new OperationInfo
    //        {
    //            Title = Target,
    //            Type = OperationType.Normal,
    //            Operation = new TargetOperation(),
    //            MinInput = 1,
    //            MaxInput = 1
    //        };
    //    }


    //    public class SourceOperation : IOperation
    //    {
    //        private readonly Func<bool, string> _func = a => a ? "Hello" : "Goodbye";

    //        public SourceOperation() { }

    //        public IOValue[] Execute(params IOValue[] operands)
    //            => new[] { new IOValue(default, _func.Invoke(ChangeType<bool>(operands[0].Value))) };
    //    }

    //    public class TargetOperation : IOperation
    //    {
    //        private readonly Func<string, bool> _func = a => a switch { "Hello" => true, "Goodbye" => false, _ => false };

    //        public TargetOperation() { }

    //        public IOValue[] Execute(params IOValue[] operands)
    //            => new[] { new IOValue(default, _func.Invoke(ChangeType<string>(operands[0].Value))) };
    //    }

    //}

    //public class InterfaceOperationsFactory : INodeFactory
    //{
    //    public IEnumerable<OperationInfo> GetOperations()
    //    {
    //        yield return new OperationInfo
    //        {
    //            Title = "Interface",
    //            Type = OperationType.Normal,
    //            Operation = new Operation(),
    //            MinInput = 1,
    //            MaxInput = 1
    //        };
    //    }

    //}


    //public class Operation : IOperation
    //{
    //    public IOValue[] Execute(params IOValue[] operands)
    //    {
    //        if (operands.Single(a => a.Title == OperationInterfaceNodeViewModel.Input0 ).Value is PropertyChange { Source: BooleanViewModel { Guid: var guid, Value: bool value } })
    //        {
    //            if (guid == Guids.Boolean)
    //            {
    //                return new[] { new IOValue(OperationInterfaceNodeViewModel.Output0, value)/*, new IOValue { Title="Output2", Value = default} */};
    //            }
    //        }
    //        return new IOValue[0];
    //    }
    //}
}
