using Utility.Helpers;
using Utility.Nodes;

namespace Nodify.Playground
{
    public static class NodeGroupHelper
    {
        public class UnionFind
        {
            private Dictionary<string, string> parent = new Dictionary<string, string>();
            private Dictionary<string, int> rank = new Dictionary<string, int>();

            public void MakeSet(string x)
            {
                if (!parent.ContainsKey(x))
                {
                    parent[x] = x;
                    rank[x] = 0;
                }
            }

            public string Find(string x)
            {
                if (!parent.ContainsKey(x))
                    MakeSet(x);

                if (parent[x] != x)
                    parent[x] = Find(parent[x]);
                return parent[x];
            }

            public void Union(string x, string y)
            {
                string rootX = Find(x);
                string rootY = Find(y);

                if (rootX != rootY)
                {
                    if (rank[rootX] < rank[rootY])
                        parent[rootX] = rootY;
                    else if (rank[rootX] > rank[rootY])
                        parent[rootY] = rootX;
                    else
                    {
                        parent[rootY] = rootX;
                        rank[rootX]++;
                    }
                }
            }

            public Dictionary<string, List<string>> GetGroups()
            {
                return parent.Keys
                    .GroupBy(Find)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
        }

        public static List<List<NodeViewModel>> GroupConnectedNodesWithUnionFind(
            IEnumerable<ConnectionViewModel> connections, UnionFind existingUnionFind)
        {

            var uf = existingUnionFind;

            // Initialize all nodes
            //foreach (var node in nodeList)
            //{
            //    uf.MakeSet(node.Key);
            //}
            List<NodeViewModel> nodeList = new();
            // Union connected nodes
            //foreach (var node in nodeList)
            //{
            //    var connections = node.Output.SelectMany(a => a.Connections.Select(c => c.Input.Node.Key));
            foreach (var connection in connections)
            {
                if (connection.Input.Node is NodeViewModel node && connection.Output.Node is NodeViewModel _node)
                {
                    uf.MakeSet(node.Key);
                    uf.MakeSet(_node.Key);
                    uf.Union(node.Key, _node.Key);
                    nodeList.Add(node);
                    nodeList.Add(_node);
                }
            }
            //}

            // Create groups and assign GroupKey
            var groupDict = uf.GetGroups();
            var groups = new List<List<NodeViewModel>>();
            int groupIndex = 0;

            foreach (var group in groupDict.Values)
            {
                var groupKey = groupIndex++.ToString();
                var nodeGroup = new List<NodeViewModel>();

                foreach (var nodeKey in group)
                {
                    var node = nodeList.First(n => n.Key == nodeKey);
                    node.GroupKey = groupKey;
                    nodeGroup.Add(node);
                }
                groups.Add(nodeGroup);
            }

            return groups;
        }
    }
}