
using Utility.Nodify.Core;
using Utility.Nodify.Operations.Infrastructure;
using DryIoc;
using System.Collections.ObjectModel;
using DynamicData;
using Utility.Helpers;
using Utility.Collections;
using Utility.Helpers.Generic;
using Utility.Nodify.Operations.Operations;
using Newtonsoft.Json;
using System.Reflection;

namespace Utility.Nodify.Engine.Infrastructure
{
    public class Converter : IConverter
    {
        private readonly IContainer container;

        public Converter(IContainer container)
        {
            this.container = container;
        }

        public IDiagramViewModel Convert(Diagram diagram)
        {
            var diagramViewModel = container.Resolve<IDiagramViewModel>();
            List<IConnectorViewModel> connectors = new();

            foreach (var node in diagram.Nodes)
            {
                INodeViewModel nodeViewModel = Convert(node);

                connectors.AddRange(nodeViewModel.Input);
                connectors.AddRange(nodeViewModel.Output);
                diagramViewModel.Nodes.Add(nodeViewModel);
            }

            foreach (var connection in diagram.Connections)
            {
                var connectionViewModel = new ConnectionViewModel
                {
                    Input = connectors.SingleOrDefault(a => a.Title == connection.Input),
                    Output = connectors.SingleOrDefault(a => a.Title == connection.Output)
                };
                container.RegisterInstanceMany<IConnectionViewModel>(connectionViewModel);
                diagramViewModel.Connections.Add(connectionViewModel);
            }

            return diagramViewModel;
        }

        public INodeViewModel Convert(Node node)
        {
            var nodeViewModel = new NodeViewModel
            {
                Title = node.Name,
                Location = node.Location
            };
            container.RegisterInstanceMany<INodeViewModel>(nodeViewModel);
            if (node.Content.DeserialiseMethod() is MethodInfo methodInfo)
            {
                nodeViewModel.Core = new MethodOperation(methodInfo);
            }
            else
            {
                var proto = JsonConvert.DeserializeObject<ObjectInfo>(node.Content);
                nodeViewModel.Core = new ObjectOperation(proto);
            }

            var inputs = node.Inputs.Select(a => new ConnectorViewModel() { Title = a.Key, IsInput = true, Node = nodeViewModel, Type = a.Content.FromString() }).ToArray();
            var outputs = node.Outputs.Select(a => new ConnectorViewModel() { Title = a.Key, Node = nodeViewModel, Type = a.Content.FromString() }).ToArray();
            nodeViewModel.Input.AddRange(inputs);
            nodeViewModel.Output.AddRange(outputs);

            return nodeViewModel;
        }


        public Diagram ConvertBack(IDiagramViewModel diagramViewModel)
        {
            var diagram = container.Resolve<Diagram>();
     
            List<Connector> connectors = new();
            //Diagram diagram = new();
            foreach (var nodeViewModel in diagramViewModel.Nodes)
            {
                var node = ConvertBack(nodeViewModel);

                connectors.AddRange(node.Inputs);
                connectors.AddRange(node.Outputs);

                diagram.Nodes.AddOrReplaceBy(a => a.Name, node);



            }

            foreach (var connectionViewModel in diagramViewModel.Connections)
            {
                var connection = new Connection
                {
                    Input = connectionViewModel.Input?.Title,
                    Output = connectionViewModel.Output?.Title
                };
                //diagram.Connections.Add(connection);
                diagram.Connections.AddOrReplaceBy(a => a.Input + a.Output, connection);
            }

            return diagram;
        }

        private static Node ConvertBack(INodeViewModel nodeViewModel)
        {
            Node node = new ()
            {
                Name = nodeViewModel.Title,
                Location = nodeViewModel.Location,
                Inputs = new Collection<Connector>(),
                Outputs = new Collection<Connector>()
            };
            if (nodeViewModel is NodeViewModel { Core: Interfaces.NonGeneric.ISerialise serialise } operationNode)
            {
                node.Content = serialise.ToString();
            }
            var inputs = nodeViewModel.Input.Select(a => new Connector() { Key = a.Title, IsInput = a.IsInput, Content = a.Type.AsString() }).ToArray();
            var outputs = nodeViewModel.Output.Select(a => new Connector() { Key = a.Title, IsInput = a.IsInput, Content = a.Type.AsString() }).ToArray();
            node.Inputs.AddRange(inputs);
            node.Outputs.AddRange(outputs);
            return node;
        }
    }
}
