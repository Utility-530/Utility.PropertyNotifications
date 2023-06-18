using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Nodify.Core;
using Utility.Nodify.Operations;
using Utility.Nodify.Operations.Infrastructure;
using Utility.Simulation.WPF.UDP.Demo;

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
            yield return new OperationInfo
            {
                Title = GraphController.Graph,
                Type = OperationType.Normal,
                Operation = container.Resolve<GraphOperation>(),
                MinInput = 1,
                MaxInput = 1
            };                 
            yield return new OperationInfo
            {
                Title = UDPController.UDP,
                Type = OperationType.Normal,
                Operation = container.Resolve<UDPOperation>(),
                MinInput = 1,
                MaxInput = 1
            };
        }
    }

    //public class LambdaOperation : IOperation
    //{
    //    private readonly Func<IOValue[], IOValue[]> func;

    //    public LambdaOperation(Func<IOValue[], IOValue[]> func)
    //    {
    //        this.func = func;
    //    }

    //    public IOValue[] Execute(params IOValue[] operands)
    //    {
    //        return func(operands);
    //    }
    //}

    //public class OperationInterfaceNodeViewModel : OperationNodeViewModel
    //{
    //    private readonly IContainer container;


    //    public const string Input0 = "Dinosaur";
    //    public const string Input1 = "Giraffe";
    //    public const string Output0 = "Hippo";
    //    public const string Output1 = "Ant";
    //    public const string MyTitle = "MyTitle";

    //    IObservable<PropertyChange> observable => container.Resolve<IObservable<PropertyChange>>(Keys.Pipe);

    //    public OperationInterfaceNodeViewModel(IContainer container)
    //    {
    //        Title = MyTitle;
    //        Location = new System.Windows.Point(300, 100);
    //        this.container = container;
    //    }

    //    public void Initialise()
    //    {
    //        observable
    //            .Subscribe(a =>
    //            {
    //                if (a.Name == nameof(Size))
    //                    return;

    //                foreach (var input in Input.Where(a => a.Title == Input0))
    //                    input.Value = a;
    //            });
    //    }

    //    public override void OnInputValueChanged(ConnectorViewModel connectorViewModel)
    //    {
    //        if (connectorViewModel.Title == Input1)
    //        {
    //            if (connectorViewModel.Value is var value)
    //            {
    //                //container.Resolve<ViewModel>().Value = value;
    //            }
    //        }
    //        base.OnInputValueChanged(connectorViewModel);
    //    }
    //}

}
