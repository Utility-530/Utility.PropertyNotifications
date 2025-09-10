
//using Utility.Nodify.Core;
//using Utility.Nodify.Operations.Infrastructure;
//using DryIoc;
//using System.Collections.ObjectModel;
//using DynamicData;
//using Utility.Helpers;
//using Utility.Helpers.Reflection;
//using Utility.Helpers.Generic;
//using Utility.Nodify.Operations.Operations;
//using Newtonsoft.Json;
//using System.Reflection;
//using System.Drawing;
//using Utility.Nodify.Entities;
//using System.Collections.Generic;
//using System.Linq;
//using Utility.Nodify.Models;
//using Utility.Nodify.Operations;
//using Utility.Interfaces.NonGeneric;
//using System;

//namespace Utility.Nodify.Engine.Infrastructure
//{
//    public class Converter : IConverter
//    {
//        private readonly IContainer container;

//        public Converter(IContainer container)
//        {
//            this.container = container;
//        }

//        public IDiagramViewModel Convert(Diagram diagram)
//        {
//            var diagramViewModel = container.Resolve<IDiagramViewModel>();
//            List<IConnectorViewModel> connectors = new();

//            foreach (var node in diagram.Nodes)
//            {
//                INodeViewModel nodeViewModel = Convert(node);

//                connectors.AddRange(nodeViewModel.Input);
//                connectors.AddRange(nodeViewModel.Output);
//                diagramViewModel.Nodes.Add(nodeViewModel);
//            }

//            foreach (var connection in diagram.Connections)
//            {
//                var connectionViewModel = new ConnectionViewModel
//                {
//                    Input = connectors.SingleOrDefault(a => a.Key == connection.Input),
//                    Output = connectors.SingleOrDefault(a => a.Key == connection.Output)
//                };
//                container.RegisterInstanceMany<IConnectionViewModel>(connectionViewModel);
//                diagramViewModel.Connections.Add(connectionViewModel);
//            }

//            return diagramViewModel;
//        }

//        public INodeViewModel Convert(Node node)
//        {
//            var nodeViewModel = new NodeViewModel
//            {
//                Key = node.Name,
//                Location = new PointF((float)node.Location.X, (float)node.Location.Y)
//            };
//            container.RegisterInstanceMany<INodeViewModel>(nodeViewModel);
//            if (node.Content.DeserialiseMethod() is MethodInfo methodInfo)
//            {
//                nodeViewModel.Data = new MethodOperation(methodInfo);
//            }
//            else
//            {
//                var proto = JsonConvert.DeserializeObject<ObjectInfo>(node.Content);
//                nodeViewModel.Data = new ObjectOperation(proto);
//            }

//            var inputs = node.Inputs.Select(a => new ConnectorViewModel() { Key = a.Key, IsInput = true, Node = nodeViewModel, Data = a.Content.FromString() }).ToArray();
//            var outputs = node.Outputs.Select(a => new ConnectorViewModel() { Key = a.Key, Node = nodeViewModel, Data = a.Content.FromString() }).ToArray();
//            nodeViewModel.Input.AddRange(inputs);
//            nodeViewModel.Output.AddRange(outputs);

//            return nodeViewModel;
//        }


//        public Diagram ConvertBack(IDiagramViewModel diagramViewModel)
//        {
//            var diagram = container.Resolve<Diagram>();
     
//            List<Connector> connectors = new();
//            //Diagram diagram = new();
//            foreach (var nodeViewModel in diagramViewModel.Nodes)
//            {
//                var node = ConvertBack(nodeViewModel);

//                connectors.AddRange(node.Inputs);
//                connectors.AddRange(node.Outputs);

//                diagram.Nodes.AddOrReplaceBy(a => a.Name, node);



//            }

//            foreach (var connectionViewModel in diagramViewModel.Connections)
//            {
//                var connection = new Connection
//                {
//                    Input = connectionViewModel.Input?.Key,
//                    Output = connectionViewModel.Output?.Key
//                };
//                //diagram.Connections.Add(connection);
//                diagram.Connections.AddOrReplaceBy(a => a.Input + a.Output, connection);
//            }

//            return diagram;
//        }

//        private static Node ConvertBack(INodeViewModel nodeViewModel)
//        {
//            Node node = new ()
//            {
//                Name = (nodeViewModel as IGetKey).Key,
//                Location = new Utility.Structs.Point(nodeViewModel.Location.X, nodeViewModel.Location.Y),
//                Inputs = new Collection<Connector>(),
//                Outputs = new Collection<Connector>()
//            };
//            if (nodeViewModel is NodeViewModel { Data: Interfaces.NonGeneric.ISerialise serialise } operationNode)
//            {
//                node.Content = serialise.ToString();
//            }
//            var inputs = nodeViewModel.Input.Select(a => new Connector() { Key = a.Key, IsInput = a.IsInput, Content = a is Type type ? type.AsString() : null }).ToArray();
//            var outputs = nodeViewModel.Output.Select(a => new Connector() { Key = a.Key, IsInput = a.IsInput, Content = a is Type type ? type.AsString() : null }).ToArray();
//            node.Inputs.AddRange(inputs);
//            node.Outputs.AddRange(outputs);
//            return node;
//        }
//    }
//}
