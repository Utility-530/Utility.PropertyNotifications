using System.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes;
using Utility.Trees.Abstractions;
using Utility.Properties;


namespace Utility.PropertyTrees
{

    public record ActivationRequest(Guid? Key, PropertyDescriptor Descriptor, object Data, PropertyType PropertyType) : Request;
    public record ActivationResponse(ValueNode PropertyNode) : Response(PropertyNode);

    public record GetViewModelResponse(IReadOnlyCollection<IViewModel> ViewModels) : Response(ViewModels);
    public record GetViewModelRequest(IEquatable Key) : Request();

    public record SetViewModelResponse(IViewModel ViewModel) : Response(ViewModel);
    public record SetViewModelRequest(IEquatable Key, IViewModel ViewModel) : Request();

    public record DescriptorFilterResponse(PropertyDescriptor PropertyDescriptor, bool Include) : Response(Include);

    public record DescriptorFilterRequest(PropertyDescriptor PropertyDescriptor) : Request();


    public record MethodInfoFilterResponse(MethodInfo MethodInfo, bool Include) : Response(Include);

    public record MethodInfoFilterRequest(MethodInfo MethodInfo) : Request();

    public record RepositorySwitchResponse(IEquatable RepositoryKey) : Response(RepositoryKey);

    public record RepositorySwitchRequest(IEquatable ItemKey) : Request();

    public record MethodActivationRequest(Guid? Key, MethodInfo MethodInfo, object Data) : Request;
    public record MethodActivationResponse(MethodNode Node) : Response(Node);


    public record MethodsRequest(Guid Guid, object Data, object? ParentData, PropertyDescriptor Descriptor) : Request();

    public record MethodsResponse(IReadOnlyTree? Node, MethodInfo Source, ChangeType ChangeType = ChangeType.Add) : Response(Node);

    public record ChildrenRequest(Guid Guid, object Data, PropertyDescriptor Descriptor) : Request();

    public record ChildrenResponse(Change<IReadOnlyTree> NodeChange, Change<PropertyDescriptor> SourceChange) : Response(NodeChange);


}
