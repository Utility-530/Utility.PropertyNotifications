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