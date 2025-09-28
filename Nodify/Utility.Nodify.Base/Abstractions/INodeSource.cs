using Splat;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodify.Operations.Infrastructure
{
    public interface INodeSource
    {
        IEnumerable<MenuItem> Filter(object viewModel);

        INodeViewModel Find(object guid);
    }

    public record MenuItem(string Key, Guid Guid, INodeSource NodeSource) : IReference
    {
        public IEnumerable<MenuItem> Children => NodeSource.Filter(this);

        public object Reference { get; set; }
    };
}
