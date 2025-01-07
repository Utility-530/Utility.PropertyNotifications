using System.Reactive.Linq;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class EmptyNode : Node
    {
        public override IObservable<object?> GetChildren()
        {
            return Observable.Empty<object>();
        }

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