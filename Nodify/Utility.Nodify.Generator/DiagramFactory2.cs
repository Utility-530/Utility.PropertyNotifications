//using System;
//using System.Threading.Tasks;
//using MoreLinq;
//using Nodify.Playground;
//using Utility.Interfaces.Exs.Diagrams;
//using Utility.Nodes;
//using Utility.Nodify.Base.Abstractions;
//using Utility.Reactives;

//namespace Utility.Nodify.Engine
//{
//    public class DiagramFactory2 : IDiagramFactory
//    {
//        GraphSchema schema = new();
//        private NodesGenerator gen = new();
//        NodesGeneratorTree genTree = new();
//        NodesGeneratorMaster genMaster = new();
//        ReactiveNodeGroupManager manager = new ReactiveNodeGroupManager();

//        public async Task Build(IDiagramViewModel diagram)
//        {

//            genTree
//                .GenerateNodes(new NodesGeneratorSettings(1000))
//                .SelfAndAdditions()
//                .Subscribe(node =>
//            {
//                diagram.Nodes.Add(node);

//            });

//            ConnectNodesTree(diagram);


//            //gen2Tree.GenerateNodes(new NodesGeneratorSettings(1000)).SelfAndAdditions().Subscribe(node =>
//            //{
//            //    diagram.Nodes.Add(node);

//            //});

//            //ConnectNodesTree2(diagram);

//            gen.GenerateNodes(new NodesGeneratorSettings(1000)).SelfAndAdditions().Subscribe(node =>
//            {
//                diagram.Nodes.Add(node);
//            });

//            ConnectNodes(diagram);
//            ConnectNodesMaster(diagram);
//        }

//        private void ConnectNodes(IDiagramViewModel diagram)
//        {

//            var connections = gen.GenerateConnections();
//            {
//                for (int i = 0; i < connections.Count; i++)
//                {
//                    var con = connections[i];
//                    if (schema.TryAddConnection(con.Input, con.Output, out var connection))
//                    {
//                        connection.Data = con.Data;
//                        manager.AddNode(connection);
//                        diagram.Connections.Add(connection);
//                    }
//                    else
//                    {

//                    }
//                }
//            }
//        }

//        private void ConnectNodesTree(IDiagramViewModel diagram)
//        {

//            var connections = genTree.GenerateConnections().AndAdditions<ConnectionViewModel>().Subscribe(con =>
//            {
//                if (schema.TryAddConnection(con.Input, con.Output, out var connection))
//                {
//                    connection.Data = con.Data;
//                    diagram.Connections.Add(connection);
//                }

//            });
//        }

//        private void ConnectNodesMaster(IDiagramViewModel diagram)
//        {

//            var connections = genMaster.GenerateConnections();
//            {
//                for (int i = 0; i < connections.Count; i++)
//                {
//                    var con = connections[i];
//                    if (schema.TryAddConnection(con.Input, con.Output, out var connection))
//                    {
//                        connection.Data = con.Data;
//                        manager.AddNode(connection);
//                        diagram.Connections.Add(connection);
//                    }
//                }
//            }
//        }

//    }
//}

