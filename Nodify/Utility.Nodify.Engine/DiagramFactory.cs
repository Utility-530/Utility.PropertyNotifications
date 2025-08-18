using System;
using MoreLinq;
using Nodify.Playground;
using Utility.Nodify.Core;
using Utility.Nodify.Models;
using Utility.Reactives;

namespace Utility.Nodify.Engine
{
    public class DiagramFactory : IDiagramFactory
    {
        GraphSchema schema = new();
        NodesGenerator<NodeViewModel> gen = new();
        NodesGeneratorTree<NodeViewModel> genTree = new();
        NodesGeneratorMaster<NodeViewModel> genMaster = new();

        public void Build(IDiagramViewModel diagram)
        {
            if(diagram.Key == "Master")
            {
                genMaster.GenerateNodes(new NodesGeneratorSettings(1000)).SelfAndAdditions().Subscribe(node =>
                {
                    diagram.Nodes.Add(node);
                });
                ConnectNodesMaster(diagram);
            }
            else if(diagram.Key == "Tree")
            {
                genTree.GenerateNodes(new NodesGeneratorSettings(1000)).SelfAndAdditions().Subscribe(node =>
                {
                    diagram.Nodes.Add(node);
                });

                ConnectNodesTree(diagram);
            }
            else if(diagram.Key == "Diagram")
            {
                gen.GenerateNodes(new NodesGeneratorSettings(1000)).SelfAndAdditions().Subscribe(node =>
                {
                    diagram.Nodes.Add(node);
                });

                ConnectNodes(diagram);
            }             
        }

        private void ConnectNodes(IDiagramViewModel diagram)
        {

            var connections = gen.GenerateConnections();
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    var con = connections[i];
                    if (schema.TryAddConnection(con.Input, con.Output, out var connection))
                    {
                        connection.Data = con.Data;
                        diagram.Connections.Add(connection);
                    }
                    else
                    {

                    }
                }
            }
        }

        private void ConnectNodesTree(IDiagramViewModel diagram)
        {

            var connections = genTree.GenerateConnections();
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    var con = connections[i];
                    if (schema.TryAddConnection(con.Input, con.Output, out var connection))
                    {
                        connection.Data = con.Data;
                        diagram.Connections.Add(connection);
                    }
                }
            }
        }

        private void ConnectNodesMaster(IDiagramViewModel diagram)
        {

            var connections = genMaster.GenerateConnections();
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    var con = connections[i];
                    if (schema.TryAddConnection(con.Input, con.Output, out var connection))
                    {
                        connection.Data = con.Data;
                        diagram.Connections.Add(connection);
                    }
                }
            }
        }

    }
}

