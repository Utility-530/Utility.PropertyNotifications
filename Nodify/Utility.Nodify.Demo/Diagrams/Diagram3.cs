//using DryIoc;
//using Utility.Nodify.Core;
//using Utility.Nodify.Operations;
//using IContainer = DryIoc.IContainer;

//namespace Utility.Nodify.Demo.Infrastructure
//{
//    public class Diagram3 : Diagram
//    {

//        public Diagram3(IContainer container)
//        {
//            Name = "Three";

//            var @interface = container.Resolve<OperationInterfaceNodeViewModel>();
//            var input3 = new ConnectorViewModel() { Title = "Input3" };
//            var input4 = new ConnectorViewModel() { Title = "Input4" };
//            var output3 = new ConnectorViewModel() { Title = "Output1", };
//            var output4 = new ConnectorViewModel() { Title = "Output2", };
//            @interface.Input.Add(input3);
//            @interface.Input.Add(input4);
//            @interface.Output.Add(output3);
//            @interface.Output.Add(output4);
//            @interface.Initialise();

//            var source = new OperationNodeViewModel { Title = CustomOperationsFactory.Source, Location = new System.Windows.Point(100, 300) };
//            var input1 = new ConnectorViewModel() { Title = "Input1", };
//            var output1 = new ConnectorViewModel() { Title = "Output1", };
//            source.Input.Add(input1);
//            source.Output.Add(output1);

//            var target = new OperationNodeViewModel { Title = CustomOperationsFactory.Target, Location = new System.Windows.Point(600, 300) };
//            var input2 = new ConnectorViewModel() { Title = "Input2" };
//            var output2 = new ConnectorViewModel() { Title = "Output2", };

//            target.Input.Add(input2);
//            target.Output.Add(output2);

//            var connection = new OperationConnectionViewModel { Output = output1, Input = input2 };
//            var connection1 = new OperationConnectionViewModel { Output = output3, Input = input1 };
//            var connection2 = new OperationConnectionViewModel { Output = output2, Input = input4 };

//            Nodes = new[] { source, target, @interface };
//            Connections = new[] { connection, connection1, connection2 };
//        }

//        //public override OperationConnectionViewModel[] Connections { get; }

//        //public override NodeViewModel[] Nodes { get; }
//    }
//}
