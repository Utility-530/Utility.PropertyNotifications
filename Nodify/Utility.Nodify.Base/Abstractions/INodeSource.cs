using Splat;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Core;

namespace Utility.Nodify.Operations.Infrastructure
{
    public interface INodeSource
    {
        IEnumerable<MenuItem> Filter(object viewModel);

        INodeViewModel Find(object guid);
    }

    public record MenuItem(string Key, Guid Guid): IReference
    {
        public IEnumerable<MenuItem> Children => Locator.Current.GetService<INodeSource>().Filter(this);

        public object Reference { get; set; }
    };
}
