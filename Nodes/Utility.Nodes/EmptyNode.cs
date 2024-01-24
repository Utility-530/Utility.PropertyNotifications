using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class EmptyNode : Node
    {
        public override async Task<object?> GetChildren()
        {
            return await Task.FromResult(Array.Empty<object>());
        }

        public override async Task<bool> HasMoreChildren()
        {
            return await Task.FromResult(false);
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            throw new NotImplementedException();
        }
    }
}