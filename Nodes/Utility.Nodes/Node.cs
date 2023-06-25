using System.Collections;
using Utility.Collections;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Abstractions;

namespace Utility.Nodes
{
    public abstract class Node : INode
    {
        private bool _isRefreshing;
        protected Collection _children = new();

        public abstract IEquatable Key { get; }
        public abstract Task<object?> GetChildren();

        public abstract Task<bool> HasMoreChildren();

        public abstract INode ToNode(object value);

        public abstract object Content { get; }

        public INode Parent { get; set; }

        public virtual IEnumerable Ancestors => GetAncestors();

        public virtual IObservable Children
        {
            get
            {
                _ = RefreshChildrenAsync();
                return _children;
            }
        }

        private IEnumerable GetAncestors()
        {
            INode parent = this;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }


        protected virtual async Task<bool> RefreshChildrenAsync()
        {
            if (_isRefreshing)
                return false;

            if (await HasMoreChildren() == false)
                return false;

            _isRefreshing = true;

            try
            {
                {
                    var output = await GetChildren();
                    if (output is IEnumerable enumerable)
                        SetChildrenCache(ToNodes(enumerable).ToList());
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
                return false;
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        protected virtual void SetChildrenCache(List<INode> childrenCache)
        {
            _children.Clear();
            _children.AddRange(childrenCache);
            _children.Complete();
        }

        protected virtual IEnumerable<INode> ToNodes(IEnumerable collection)
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