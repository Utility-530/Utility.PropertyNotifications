using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class EmptyNode : NodeViewModel<object>
    {
        public override async Task<bool> HasMoreChildren()
        {
            return await Task.FromResult(false);
        }

        public override Task<ITree> ToTree(object value)
        {
            throw new NotImplementedException();
        }
    }
}