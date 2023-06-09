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

        public record GetViewModelResponse(IReadOnlyCollection<IViewModel> ViewModels) : Response(ViewModels);
        public record GetViewModelRequest(IEquatable Key) : Request();    
        
        public record SetViewModelResponse(IViewModel ViewModel) : Response(ViewModel);
        public record SetViewModelRequest(IEquatable Key, IViewModel ViewModel) : Request();

        public record DescriptorFilterResponse(PropertyDescriptor PropertyDescriptor, bool Include) : Response(Include);

        public record DescriptorFilterRequest(PropertyDescriptor PropertyDescriptor) : Request();

        public record RepositorySwitchResponse(IEquatable RepositoryKey) : Response(RepositoryKey);

        public record RepositorySwitchRequest(IEquatable ItemKey) : Request();
    }
}
