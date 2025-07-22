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

            ConnectNodes();
        }
        private void ConnectNodes()
        {

            var connections = gen.GenerateConnections();
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    var con = connections[i];
                    schema.TryAddConnection(con.Input, con.Output);
                }
            }
        }

    }
}

