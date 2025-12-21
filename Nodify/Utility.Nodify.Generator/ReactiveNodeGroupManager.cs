using Utility.Nodes;

namespace Nodify.Playground
{
    public class ReactiveNodeGroupManager
    {
        private readonly Dictionary<string, ConnectionViewModel> connections = new Dictionary<string, ConnectionViewModel>();
        private List<List<NodeViewModel>> _currentGroups = new List<List<NodeViewModel>>();
        NodeGroupHelper.UnionFind unionFind = new();
        public event Action<List<List<NodeViewModel>>> GroupsChanged;

        public void AddConnection(ConnectionViewModel connection)
        {
            connections[connection.Key] = connection;
            RecalculateGroups();
        }

        public void AddConnections(IEnumerable<ConnectionViewModel> nodes)
        {
            foreach (var node in nodes)
            {
                connections[node.Key] = node;
            }
            RecalculateGroups();
        }

        public void RemoveNode(string nodeKey)
        {
            if (connections.Remove(nodeKey))
            {
                RecalculateGroups();
            }
        }

        private void RecalculateGroups()
        {
            _currentGroups = NodeGroupHelper.GroupConnectedNodesWithUnionFind(connections.Values, unionFind);
            GroupsChanged?.Invoke(_currentGroups);
        }

        public List<List<NodeViewModel>> GetCurrentGroups() => _currentGroups;
    }
}