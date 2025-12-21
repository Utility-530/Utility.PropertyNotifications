using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using Utility.Enums;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Nodes;

namespace Nodify.Playground
{
    public class GraphSchema
    {
        #region Add Connection

        public bool CanAddConnection(IConnectorViewModel source, object target)
        {
            if (target is ConnectorViewModel con)
            {
                if (source != con)
                    if (source.Node != con.Node)
                        //if (source.Node.Graph == con.Node.Graph)
                            if (source.Shape == con.Shape)
                                if (con.AllowsNewConnections())
                                    if (con.AllowsNewConnections())
                                        if (source.Flow != con.Flow)
                                            return !con.IsConnectedTo(con);

            }
            //else if (con.AllowsNewConnections() && target is NodeViewModel node)
            //{
            //    var allConnectors = source.Flow == ConnectorFlow.Input ? node.Output : node.Input;
            //    return allConnectors.Any(c => (c as ConnectorViewModel)?.AllowsNewConnections() == true);
            //}

            return false;
        }

        public bool TryAddConnection(IConnectorViewModel source, object? target,out ConnectionViewModel? connection)
        {
            if (target != null && CanAddConnection(source, target))
            {
                if (target is ConnectorViewModel connector)
                {
                    connection = AddConnection(source, connector);
                    return true;
                }
                else if (target is NodeViewModel node)
                {
                    var allConnectors = source.Flow == IO.Input ? node.Outputs : node.Inputs;
                    var _connector = allConnectors.First(c => (c as ConnectorViewModel)?.AllowsNewConnections() == true);
                    connection = AddConnection(source, _connector);
                    return true;
                }
            }
            connection = null;
            return false;
        }

        private ConnectionViewModel AddConnection(IConnectorViewModel source, IConnectorViewModel target)
        {
            var sourceIsInput = source.Flow == IO.Input;

            return new ConnectionViewModel
            {
                Input = sourceIsInput ? source : target,
                Output = sourceIsInput ? target : source,
                Key = Guid.NewGuid().ToString()
            };
        }

        //private ConnectionViewModel AddConnection(IConnectorViewModel source, INodeViewModel target)
        //{
        //    var allConnectors = source.Flow == IO.Input ? target.Outputs : target.Inputs;
        //    var connector = allConnectors.First(c => (c as ConnectorViewModel)?.AllowsNewConnections() == true);

        //    return AddConnection(source, connector);
        //}

        #endregion

        public void DisconnectConnector(ConnectorViewModel connector)
        {
            var graph = connector.Node.Diagram;
            var connections = connector.Connections.ToList();
            connections.ForEach(c => graph.Connections.Remove(c));
        }

        public void SplitConnection(ConnectionViewModel connection, PointF location)
        {
            //var knot = new NodeViewModel()
            //{
            //    Location = location,
            //    Orientation = connection.Output.Node.Orientation,
            //    Flow = connection.Output.Flow,
            //    Connector = new ConnectorViewModel
            //    {
            //        MaxConnections = connection.Output.MaxConnections + connection.Input.MaxConnections,
            //        Shape = connection.Input.Shape
            //    }
            //};
            //connection.Graph.Nodes.Add(knot);

            //AddConnection(connection.Output, knot.Connector);
            //AddConnection(knot.Connector, connection.Input);

            //connection.Remove();
        }

        public void AddCommentAroundNodes(IList<NodeViewModel> nodes, string? text = default)
        {
            //var rect = nodes.GetBoundingBox(50);
            //var comment = new CommentNodeViewModel
            //{
            //    Location = rect.Location,
            //    Size = rect.Size,
            //    Title = text ?? "New comment"
            //};

            //nodes[0].Graph.Nodes.Add(comment);
        }

        /// <summary>
        /// Rewires all connections from the source connector to the target connector if possible.
        /// </summary>
        /// <remarks>The source must be an input connector.</remarks>
        public void Rewire(ConnectorViewModel source, ConnectorViewModel target)
        {
            if (source == target || source.Flow != IO.Input)
                return;

            var connectionsToRewire = source.Connections.ToList();
            foreach (var connection in connectionsToRewire)
            {
                if (CanAddConnection(connection.Output, target))
                {
                    source.Node.Diagram.Connections.Remove(connection);
                    AddConnection(connection.Output, target);
                }
            }
        }
    }
}
