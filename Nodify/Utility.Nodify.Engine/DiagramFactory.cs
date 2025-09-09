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
        NodesGeneratorTree2<NodeViewModel> gen2Tree = new();

        public void Build(IDiagramViewModel diagram)
        {
            gen2Tree.GenerateNodes(new NodesGeneratorSettings(1000)).SelfAndAdditions().Subscribe(node =>
            {
                diagram.Nodes.Add(node);

            });

            //ConnectNodesTree2(diagram);    
        }


        private void ConnectNodesTree2(IDiagramViewModel diagram)
        {

            var connections = gen2Tree.GenerateConnections().AndAdditions<ConnectionViewModel>().Subscribe(con =>
            {
                if (schema.TryAddConnection(con.Input, con.Output, out var connection))
                {
                    connection.Data = con.Data;
                    diagram.Connections.Add(connection);
                }

            });
        }
    }
}

