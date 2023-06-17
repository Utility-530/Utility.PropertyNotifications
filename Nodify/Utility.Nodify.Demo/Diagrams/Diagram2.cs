using DryIoc;
using Utility.Nodify.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Nodify.Demo.Infrastructure
{
    //public class Diagram2 : Diagram
    //{

    //    public Diagram2(IContainer container)
    //    {
    //        Name = "Two";

    //        var source = new OperationNodeViewModel { Title = CustomOperationsFactory.Source, Location = new System.Windows.Point(100, 300) };
    //        var input1 = Globals.ViewModelInputConnector;
    //        var output1 = new ConnectorViewModel() { Title = "Output1", Node = source };
    //        source.Input.Add(input1);
    //        source.Output.Add(output1);
    //        source.OnInputValueChanged();


    //        var target = new OperationNodeViewModel { Title = CustomOperationsFactory.Target, Location = new System.Windows.Point(600, 300) };
    //        var input2 = new ConnectorViewModel() { Title = "Input2", Value = "Default" };
    //        var output2 = Globals.ViewModelOutputConnector;
    //        target.Input.Add(input2);
    //        target.Output.Add(output2);
    //        target.OnInputValueChanged();
    //        var connection = new OperationConnectionViewModel() { Input = input2, Output = output1 };
    //        Nodes = new[] { source, target };
    //        Connections = new[] { connection };
    //    }
    //    public override OperationConnectionViewModel[] Connections { get; }
    //    public override NodeViewModel[] Nodes { get; }
    //}

}
