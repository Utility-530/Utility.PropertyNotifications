using Utility.Nodify.Models;

namespace Nodify.Playground
{
    public class ReactiveNodeGroupManager
    {
        private readonly Dictionary<string, ConnectionViewModel> _nodes = new Dictionary<string, ConnectionViewModel>();
        private List<List<NodeViewModel>> _currentGroups = new List<List<NodeViewModel>>();
        NodeGroupHelper.UnionFind unionFind = new();
        public event Action<List<List<NodeViewModel>>> GroupsChanged;

        public void AddNode(ConnectionViewModel node)
        {
            _nodes[node.Key] = node;
            RecalculateGroups();
        }

        public void AddNodes(IEnumerable<ConnectionViewModel> nodes)
        {
            foreach (var node in nodes)
            {
                _nodes[node.Key] = node;
            }
            RecalculateGroups();
        }

        public void RemoveNode(string nodeKey)
        {
            if (_nodes.Remove(nodeKey))
            {
                RecalculateGroups();
            }
        }

        private void RecalculateGroups()
        {
            _currentGroups = NodeGroupHelper.GroupConnectedNodesWithUnionFind(_nodes.Values, unionFind);
            GroupsChanged?.Invoke(_currentGroups);
        }

        public List<List<NodeViewModel>> GetCurrentGroups() => _currentGroups;
    }
}