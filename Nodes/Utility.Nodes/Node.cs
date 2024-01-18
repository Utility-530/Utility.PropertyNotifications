using DynamicData;
using System.Collections;
using Utility.Collections;
using Utility.Interfaces.NonGeneric;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public abstract class Node : Tree
    {
        private bool _isRefreshing;
        protected Collection items = new();

        public abstract Task<object?> GetChildren();

        public abstract Task<bool> HasMoreChildren();

        public abstract IReadOnlyTree ToNode(object value);

        public override IEnumerable Items
        {
            get
            {
                _ = RefreshChildrenAsync();
                return items;
            }
        }


        protected virtual async Task<bool> RefreshChildrenAsync()
        {
            if (_isRefreshing)
                return false;

            if (await HasMoreChildren() == false)
                return false;

            _isRefreshing = true;

            List<IReadOnlyTree> nodes = new();
            try
            {
                {
                    var output = await GetChildren();
                    if (output is IEnumerable enumerable)
                    {
                        foreach (var node in ToNodes(enumerable))
                            nodes.Add(node);
                   
                    }
                }
                //{
                //    var output = await GetProperties();
                //    if (output is IEnumerable enumerable)
                //        SetPropertiesCache(ToNodes(enumerable).ToList());
                //}
                return true;
            }
            catch (Exception ex)
            {
                Error = ex;
                return false;
            }
            finally
            {
                SetChildrenCache(nodes);
                _isRefreshing = false;
            }
     
        }


        public Exception Error { get; set; }
        protected virtual void SetChildrenCache(List<IReadOnlyTree> childrenCache)
        {
            items.Clear();
            items.AddRange(childrenCache);
            items.Complete();
        }

        protected virtual IEnumerable<IReadOnlyTree> ToNodes(IEnumerable collection)
        {
            foreach (var item in collection)
            {
                var node = ToNode(item);
                node.Parent = this;
                yield return node;
            }
        }
    }
}