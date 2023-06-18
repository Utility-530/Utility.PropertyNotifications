using DryIoc;
using Utility.Nodify.Core;
using Utility.Nodify.Operations;
using IContainer = DryIoc.IContainer;
using Utility.Simulation.WPF.UDP.Demo;

namespace Utility.Nodify.Demo.Infrastructure
{
    public class Diagram1 : Diagram
    {
        public Diagram1(IContainer container)
        {
            Name = "One";
            
            var graphController = container.Resolve<GraphController>();
            var udpController = container.Resolve<UDPController>();
            var resetController = container.Resolve<ResetController>();
            
            var connection = new OperationConnectionViewModel { Output = graphController.Output0, Input = udpController.Input0 };
            var connection1 = new OperationConnectionViewModel { Output = udpController.Output0, Input = graphController.Input0 };
            var connection2 = new OperationConnectionViewModel { Output = udpController.Output1, Input = graphController.Input1 };
            var connection3 = new OperationConnectionViewModel { Output = udpController.Output2, Input = graphController.Input2 };
            var connection4 = new OperationConnectionViewModel { Output = udpController.Output2, Input = resetController.Input0 };

            Nodes = new OperationNodeViewModel[] { graphController, udpController, resetController };
            Connections = new[] { connection, connection1, connection2, connection3, connection4 };
        }

        public override OperationConnectionViewModel[] Connections { get; }

        public override NodeViewModel[] Nodes { get; }
    }
}
