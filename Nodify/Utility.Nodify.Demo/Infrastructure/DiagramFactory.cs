using System;
using System.Threading.Tasks;
using MoreLinq;
using Nodify.Playground;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Nodes;
using Utility.Nodify.Base.Abstractions;
using Utility.Reactives;

namespace Utility.Nodify.Demo.Infrastructure
{
    public class DiagramFactory : IDiagramFactory
    {
        private GraphSchema schema = new();
        private NodesGenerator gen = new();
        private NodesGeneratorTree genTree = new();
        private NodesGeneratorMaster genMaster = new();
        private ReactiveNodeGroupManager manager = new ReactiveNodeGroupManager();

        public async Task Build(IDiagramViewModel diagram)
        {
            genTree
                .GenerateNodes(new NodesGeneratorSettings(1000))
                .SelfAndAdditions()
                .Subscribe(node =>
                {
                    diagram.Nodes.Add(node);
                });

            ConnectNodesTree(diagram);

            gen.GenerateNodes(new NodesGeneratorSettings(1000)).SelfAndAdditions().Subscribe(node =>
            {
                diagram.Nodes.Add(node);
            });

            ConnectNodes(diagram);
            ConnectNodesMaster(diagram);

            void ConnectNodes(IDiagramViewModel diagram)
            {
                var connections = gen.GenerateConnections().SelfAndAdditions().Subscribe(con =>
                {
                    if (schema.TryAddConnection(con.Input, con.Output, out var connection))
                    {
                        connection.Data = con.Data;
                        manager.AddConnection(connection);
                        diagram.Connections.Add(connection);
                    }
                    else
                    {
                        throw new Exception("SF£dsss");
                    }
                });
            }

            void ConnectNodesTree(IDiagramViewModel diagram)
            {
                var connections = genTree.GenerateConnections().AndAdditions<ConnectionViewModel>().Subscribe(con =>
                {
                    if (schema.TryAddConnection(con.Input, con.Output, out var connection))
                    {
                        connection.Data = con.Data;
                        diagram.Connections.Add(connection);
                    }
                    else
                    {
                        throw new Exception("S8 8F£dsss");
                    }
                });
            }

            void ConnectNodesMaster(IDiagramViewModel diagram)
            {
                var connections = genMaster.GenerateConnections();
                {
                    for (int i = 0; i < connections.Count; i++)
                    {
                        var con = connections[i];
                        if (schema.TryAddConnection(con.Input, con.Output, out var connection))
                        {
                            connection.Data = con.Data;
                            manager.AddConnection(connection);
                            diagram.Connections.Add(connection);
                        }
                    }
                }
            }
        }
    }
}

