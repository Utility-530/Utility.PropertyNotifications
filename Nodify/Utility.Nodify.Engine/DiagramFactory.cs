using MoreLinq;
using Nodify.Playground;
using Utility.Nodify.Core;
using Utility.Nodify.Models;

namespace Utility.Nodify.Engine
{
    public class DiagramFactory : IDiagramFactory
    {
        GraphSchema schema = new();
        NodesGenerator<NodeViewModel> gen = new();

        public void Build(IDiagramViewModel diagram)
        {
            gen.GenerateNodes(new NodesGeneratorSettings(1000)).ForEach(node =>
            {
                diagram.Nodes.Add(node);
            });

            ConnectNodes(diagram);
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
                }
            }
        }

    }
}

