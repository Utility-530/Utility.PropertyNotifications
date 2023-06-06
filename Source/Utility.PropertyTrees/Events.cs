using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes;
using Utility.PropertyTrees.Abstractions;

namespace Utility.PropertyTrees
{
    public class Events
    {
        public record ActivationRequest(Guid? Key, PropertyDescriptor Descriptor, object Data, PropertyType PropertyType) : Request;
        public record ActivationResponse(ValueNode PropertyNode) : Response(PropertyNode);

        public record GetViewModelResponse(IViewModel ViewModel) : Response(ViewModel);
        public record GetViewModelRequest(IEquatable Key) : Request();
    }
}
