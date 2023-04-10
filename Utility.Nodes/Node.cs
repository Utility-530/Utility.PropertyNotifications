
using System.Collections;
using Utility.Collections;
using Utility.Nodes.Abstractions;
using Utility.Observables;

namespace Utility.Nodes
{
    public abstract class Node : INode
    {
        private bool _isRefreshing;
        protected Collection _children = new();
        protected Collection _leaves = new();
        protected Collection _branches = new();
        public abstract Task<object?> GetChildren();
        public virtual Task<object?> GetLeaves() => throw new NotImplementedException();
        public virtual Task<object?> GetBranches() => throw new NotImplementedException();

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

        public virtual IObservable Branches
        {
            get
            {
                _ = RefreshBranchesAsync();
                return _branches;
            }
        }

        public virtual IObservable Leaves
        {
            get
            {
                _ = RefreshChildrenAsync();
                return _leaves;
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
        protected virtual Task<bool> RefreshBranchesAsync()
        {
            return Task.FromResult(true);
        }
        protected virtual Task<bool> RefreshLeavesAsync()
        {
            return Task.FromResult(true);
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
