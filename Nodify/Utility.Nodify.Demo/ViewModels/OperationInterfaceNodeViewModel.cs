//using Autofac;
using DryIoc;
using Utility.Nodify.Core;
using Utility.Nodify.Demo.Infrastructure;
using Utility.Nodify.Operations;
using System;
using System.Linq;
using System.Reactive.Linq;
using IContainer = DryIoc.IContainer;

namespace Utility.Nodify.Demo
{
    //public class OperationInterfaceNodeViewModel : OperationNodeViewModel
    //{
    //    private readonly IContainer container;


    //    public const string Input0 = "Dinosaur";
    //    public const string Input1 = "Giraffe" ;
    //    public const string Output0 = "Hippo";
    //    public const string Output1 = "Ant";


    //    IObservable<PropertyChange> observable => container.Resolve<IObservable<PropertyChange>>(Keys.Pipe);

    //    public OperationInterfaceNodeViewModel(IContainer container)
    //    {
    //        Title = CustomOperationsFactory.Interface;
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
