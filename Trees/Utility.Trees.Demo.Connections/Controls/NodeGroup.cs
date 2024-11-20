using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Controls.Base;

namespace Utility.Trees.Demo.Connections
{
    public class NodeGroup : ListBox<Node>
    {
        public delegate void ConnectorChangedEventHandler(object source, ConnectorChangeRoutedEventArgs rangeChange);
        public delegate void NodeChangedEventHandler(object source, NodeChangeRoutedEventArgs rangeChange);

        public static readonly RoutedEvent ConnectorChangeEvent = EventManager.RegisterRoutedEvent("ConnectorChange", RoutingStrategy.Bubble, typeof(ConnectorChangedEventHandler), typeof(NodeGroup));
        public static readonly RoutedEvent NodeChangeEvent = EventManager.RegisterRoutedEvent("NodeChange", RoutingStrategy.Bubble, typeof(NodeChangedEventHandler), typeof(NodeGroup));


        public NodeGroup()
        {
            EventManager.RegisterClassHandler(typeof(Connector), Control.MouseDownEvent, new RoutedEventHandler(Button_Click));
            EventManager.RegisterClassHandler(typeof(Connector), Control.PreviewMouseUpEvent, new RoutedEventHandler(Button2_Click));
            EventManager.RegisterClassHandler(typeof(Node), Control.MouseDownEvent, new RoutedEventHandler(Node_Mouse_Down_Click));

            void Node_Mouse_Down_Click(object sender, RoutedEventArgs e)
            {
                RaiseNodeChangeEvent(sender as Node);
            }

            void Button2_Click(object sender, RoutedEventArgs e)
            {
                RaiseConnectorChangeEvent(sender as Connector);
            }

            void Button_Click(object sender, RoutedEventArgs e)
            {
                e.Handled = true;
                RaiseConnectorChangeEvent(sender as Connector);
            }

        }

        public event ConnectorChangedEventHandler ConnectorChange
        {
            add { AddHandler(ConnectorChangeEvent, value); }
            remove { RemoveHandler(ConnectorChangeEvent, value); }
        }

        public event NodeChangedEventHandler NodeChange
        {
            add { AddHandler(NodeChangeEvent, value); }
            remove { RemoveHandler(NodeChangeEvent, value); }
        }


        private void RaiseConnectorChangeEvent(Connector start)
        {
            ConnectorChangeRoutedEventArgs newEventArgs = new (ConnectorChangeEvent, start);
            RaiseEvent(newEventArgs);
        }

        private void RaiseNodeChangeEvent(Node start)
        {
            NodeChangeRoutedEventArgs newEventArgs = new (NodeChangeEvent, start);
            RaiseEvent(newEventArgs);
        }
    }

    public class ConnectorChangeRoutedEventArgs : RoutedEventArgs
    {
        public Connector Connector;

        public ConnectorChangeRoutedEventArgs(RoutedEvent @event, Connector start) : base(@event)
        {
            Connector = start;
        }
    }
    public class NodeChangeRoutedEventArgs : RoutedEventArgs
    {
        public Node Node;

        public NodeChangeRoutedEventArgs(RoutedEvent @event, Node start) : base(@event)
        {
            Node = start;
        }
    }
}
